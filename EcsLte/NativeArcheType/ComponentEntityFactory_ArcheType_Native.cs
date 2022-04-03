using EcsLte.Data;
using EcsLte.Data.Unmanaged;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.NativeArcheType
{
    public class ComponentEntityFactory_ArcheType_Native : IComponentEntityFactory
    {
        private static readonly int _entitiesInitCapacity = 4;

        private unsafe EntityData_ArcheType_Native* _entityDatas;
        private unsafe Entity* _entities;
        private int _entitiesCount;
        private int _entitiesLength;
        private Entity[] _cachedEntites;
        private bool _cachedEntitiesDirty;
        private unsafe Entity* _reusableEntities;
        private int _reusableEntitiesCount;
        private int _reusableEntitiesLength;
        private unsafe Entity* _uniqueComponentEntities;
        private unsafe ComponentConfigIndex_ArcheType_Native* _uniqueConfigs;
        private unsafe byte* _uniqueComponents;
        private int _uniqueComponentsLengthInBytes;
        private ArcheTypeFactory_ArcheType_Native _archeTypeFactory;
        private IIndexDictionary[] _sharedComponentIndexes;
        private int _nextId;

        public int Count => _entitiesCount;
        public int Capacity => _entitiesLength;

        public unsafe ComponentEntityFactory_ArcheType_Native()
        {
            _entityDatas = MemoryHelper.Alloc<EntityData_ArcheType_Native>(_entitiesInitCapacity);

            _entities = MemoryHelper.Alloc<Entity>(_entitiesInitCapacity);
            _entitiesCount = 0;
            _entitiesLength = _entitiesInitCapacity;

            //_cachedEntites
            _cachedEntitiesDirty = true;

            _reusableEntities = MemoryHelper.Alloc<Entity>(_entitiesInitCapacity);
            _reusableEntitiesCount = 0;
            _reusableEntitiesLength = _entitiesInitCapacity;

            var uniqueComponentCount = ComponentConfigs.Instance.UniqueComponentCount;
            _uniqueComponentEntities = MemoryHelper.Alloc<Entity>(uniqueComponentCount);
            _uniqueConfigs = MemoryHelper.Alloc<ComponentConfigIndex_ArcheType_Native>(uniqueComponentCount);
            for (var i = 0; i < uniqueComponentCount; i++)
            {
                var config = ComponentConfigs.Instance.AllUniqueConfigs[i];
                _uniqueConfigs[i] = new ComponentConfigIndex_ArcheType_Native
                {
                    Index = i,
                    OffsetInBytes = _uniqueComponentsLengthInBytes,
                    Config = config
                };
                _uniqueComponentsLengthInBytes += config.UnmanagedInBytesSize;
            }
            _uniqueComponents = (byte*)MemoryHelper.Alloc(_uniqueComponentsLengthInBytes);

            _archeTypeFactory = new ArcheTypeFactory_ArcheType_Native();

            _sharedComponentIndexes = IndexDictionary.CreateSharedComponentIndexDictionaries();

            _nextId = 1;
        }

        public unsafe Entity[] GetEntities()
        {
            if (_cachedEntitiesDirty)
            {
                if (_cachedEntites == null)
                    _cachedEntites = new Entity[Count];
                else if (_cachedEntites.Length != Count)
                    Array.Resize(ref _cachedEntites, Count);

                var cachedIndex = 0;
                for (var i = 1; i <= Count; i++)
                {
                    var entity = _entities[i];
                    if (entity.IsNotNull)
                        _cachedEntites[cachedIndex++] = entity;
                }
                _cachedEntitiesDirty = false;
            }

            return _cachedEntites;
        }

        public unsafe bool HasEntity(Entity entity) => entity.Id > 0 &&
                entity.Id < Capacity &&
                _entities[entity.Id] == entity;

        public unsafe Entity CreateEntity(IEntityBlueprint blueprint)
        {
            CheckUnusedCapacity(1);

            var entity = AllocateEntity(out var entityData);
            if (blueprint != null)
            {
                var archeTypeIndex = ((EntityBlueprint_ArcheType_Native)blueprint).GetArcheTypeIndex(
                    _archeTypeFactory,
                    _uniqueConfigs,
                    _sharedComponentIndexes,
                    out var blueprintData);
                var archeTypeData = _archeTypeFactory.GetArcheTypeData(archeTypeIndex);
                archeTypeData->AddEntity(entity, entityData);

                for (var i = 0; i < blueprintData.UniqueComponents.Length; i++)
                {
                    var config = blueprintData.UniqueConfigs[i];
                    var component = blueprintData.UniqueComponents[i];

                    if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                    {
                        throw new EntityAlreadyHasComponentException(_uniqueComponentEntities[config.UniqueIndex],
                            ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                    }

                    _uniqueComponentEntities[config.UniqueIndex] = entity;
                    archeTypeData->SetUniqueComponent(config, component.GetData(), _uniqueComponents);
                }

                if (blueprintData.Components.Length > 0)
                {
                    var componentsBuffer = blueprintData.CreateComponentsBuffer();
                    archeTypeData->SetEntityBlueprintData(entityData, componentsBuffer, blueprintData.ComponentsLengthInBytes);

                    MemoryHelper.Free(componentsBuffer);
                }
            }
            _cachedEntitiesDirty = true;

            return entity;
        }

        public unsafe Entity[] CreateEntities(int count, IEntityBlueprint blueprint)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count", "Must be greater than 0.");
            if (count == 1)
                return new Entity[] { CreateEntity(blueprint) };

            CheckUnusedCapacity(count);

            var entities = new Entity[count];
            if (blueprint != null)
            {
                var archeTypeIndex = ((EntityBlueprint_ArcheType_Native)blueprint).GetArcheTypeIndex(
                    _archeTypeFactory,
                    _uniqueConfigs,
                    _sharedComponentIndexes,
                    out var blueprintData);
                var archeTypeData = _archeTypeFactory.GetArcheTypeData(archeTypeIndex);

                if (blueprintData.UniqueComponents.Length > 0)
                {
                    var config = blueprintData.UniqueConfigs[0];
                    var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                    if (uniqueEntity == Entity.Null)
                        uniqueEntity = AllocateEntity(out _);

                    throw new EntityAlreadyHasComponentException(uniqueEntity,
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }

                var componentsBuffer = blueprintData.CreateComponentsBuffer();
                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity(out var entityData);
                    entities[i] = entity;

                    archeTypeData->AddEntity(entity, entityData);
                    archeTypeData->SetEntityBlueprintData(entityData, componentsBuffer, blueprintData.ComponentsLengthInBytes);
                }

                MemoryHelper.Free(componentsBuffer);
            }
            else
            {
                for (var i = 0; i < count; i++)
                    entities[i] = AllocateEntity(out _);
            }
            _cachedEntitiesDirty = true;

            return entities;
        }

        public unsafe void DestroyEntity(Entity entity)
        {
            // Remove components checks HasEntity
            RemoveAllComponents(entity);

            CheckReusedCapacity(1);

            _entities[entity.Id] = Entity.Null;
            _reusableEntities[_reusableEntitiesCount++] = entity;
            _entitiesCount--;
            _cachedEntitiesDirty = true;
        }

        public unsafe void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            CheckReusedCapacity(entities.Count());

            foreach (var entity in entities)
            {
                // Remove components checks HasEntity
                RemoveAllComponents(entity);

                _entities[entity.Id] = Entity.Null;
                _reusableEntities[_reusableEntitiesCount++] = entity;
            }
            _entitiesCount -= entities.Count();
            _cachedEntitiesDirty = true;
        }

        public unsafe bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var config = ComponentConfig<TComponent>.Config;
            var entityData = _entityDatas[entity.Id];

            return entityData.ComponentArcheTypeData != null &&
                entityData.ComponentArcheTypeData->ArcheType.HasComponentConfig(config);
        }

        public unsafe TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return GetComponentPostCheck<TComponent>(entity, ComponentConfig<TComponent>.Config);
        }

        public unsafe IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = &_entityDatas[entity.Id];
            if (entityData->ComponentArcheTypeData != null)
                return entityData->ComponentArcheTypeData->GetAllComponents(entityData, _uniqueComponents);
            return new IComponent[0];
        }

        public unsafe bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            var config = ComponentConfig<TComponentUnique>.Config;

            return _uniqueComponentEntities[config.UniqueIndex] != Entity.Null;
        }

        public unsafe TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;

            var entityData = _entityDatas[_uniqueComponentEntities[config.UniqueIndex].Id];
            return *(TComponentUnique*)entityData.ComponentArcheTypeData
                ->GetUniqueComponent(config, _uniqueComponents);
        }

        public unsafe Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        public unsafe void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config);
        }

        public unsafe void ReplaceComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config);
            else
                ReplaceComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config);
        }

        public unsafe void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            RemoveComponentPostCheck<TComponent>(entity, ComponentConfig<TComponent>.Config);
        }

        public unsafe void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = &_entityDatas[entity.Id];
            var prevArcheTypeData = entityData->ComponentArcheTypeData;
            if (prevArcheTypeData != null)
                prevArcheTypeData->RemoveEntity(entityData, _entityDatas);
        }

        public unsafe Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityAlreadyHasComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var entity = CreateEntity(null);
            AddComponentPostCheck(entity, componentUnique, ComponentConfig<TComponentUnique>.Config);

            return entity;
        }

        public unsafe Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(newComponentUnique);

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            ReplaceComponentPostCheck(entity, newComponentUnique, config);

            return entity;
        }

        public unsafe void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            RemoveComponentPostCheck<TComponentUnique>(entity, config);
        }

        public unsafe void Dispose()
        {
            MemoryHelper.Free(_entityDatas);
            _entityDatas = null;
            MemoryHelper.Free(_entities);
            _entities = null;
            _entitiesCount = 0;
            _entitiesLength = 0;
            _cachedEntites = null;
            _cachedEntitiesDirty = true;
            MemoryHelper.Free(_reusableEntities);
            _reusableEntitiesCount = 0;
            _reusableEntitiesLength = 0;
            MemoryHelper.Free(_uniqueComponentEntities);
            _uniqueComponentEntities = null;
            MemoryHelper.Free(_uniqueConfigs);
            _uniqueConfigs = null;
            MemoryHelper.Free(_uniqueComponents);
            _uniqueComponents = null;
            _uniqueComponentsLengthInBytes = 0;
            _archeTypeFactory.Dispose();
            _archeTypeFactory = null;
            foreach (var indexDic in _sharedComponentIndexes)
                indexDic.Clear();
            _sharedComponentIndexes = null;
            _nextId = 0;
        }

        private unsafe void CheckUnusedCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = Capacity - ((Count + 1) - _reusableEntitiesCount);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_entitiesLength + count, 2) + 1);
                _entities = (Entity*)MemoryHelper.Realloc(
                    _entities,
                    _entitiesLength * TypeCache<Entity>.SizeInBytes,
                    newCapacity * TypeCache<Entity>.SizeInBytes);
                _entityDatas = (EntityData_ArcheType_Native*)MemoryHelper.Realloc(
                    _entityDatas,
                    _entitiesLength * TypeCache<EntityData_ArcheType_Native>.SizeInBytes,
                    newCapacity * TypeCache<EntityData_ArcheType_Native>.SizeInBytes);

                _entitiesLength = newCapacity;
            }
        }

        private unsafe void CheckReusedCapacity(int count)
        {
            var unusedCount = _reusableEntitiesLength - _reusableEntitiesCount;
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_reusableEntitiesLength + count, 2) + 1);
                _reusableEntities = (Entity*)MemoryHelper.Realloc(
                    _reusableEntities,
                    _reusableEntitiesLength * TypeCache<Entity>.SizeInBytes,
                    newCapacity * TypeCache<Entity>.SizeInBytes);
                _reusableEntitiesLength = newCapacity;
            }
        }

        private unsafe Entity AllocateEntity(out EntityData_ArcheType_Native* entityData)
        {
            Entity entity;
            if (_reusableEntitiesCount > 0)
            {
                entity = _reusableEntities[--_reusableEntitiesCount];
                entity.Version++;
                entityData = &_entityDatas[entity.Id];
                entityData->Clear();
            }
            else
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
                entityData = &_entityDatas[entity.Id];
            }
            _entities[entity.Id] = entity;
            _entitiesCount++;

            return entity;
        }

        private unsafe void AddComponentPostCheck<TComponent>(Entity entity, TComponent component, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            var entityData = &_entityDatas[entity.Id];
            var prevArcheTypeData = entityData->ComponentArcheTypeData;

            Component_ArcheType_Native nextArcheType;
            if (prevArcheTypeData != null)
                nextArcheType = entityData->ComponentArcheTypeData->ArcheType;
            else
                nextArcheType = new Component_ArcheType_Native();

            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[ComponentConfig<TComponent>.Config.UniqueIndex],
                        typeof(TComponent));
                }

                _uniqueComponentEntities[config.UniqueIndex] = entity;
                nextArcheType = Component_ArcheType_Native.AppendComponent(nextArcheType, config);
            }
            else if (config.IsShared)
            {
                nextArcheType = Component_ArcheType_Native.AppendComponent(
                    nextArcheType,
                    config,
                    _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component));
            }
            else
            {
                nextArcheType = Component_ArcheType_Native.AppendComponent(nextArcheType, config);
            }

            if (!_archeTypeFactory.GetArcheTypeData(nextArcheType, _uniqueConfigs, out var nextArcheTypeData))
                nextArcheType.Dispose();

            if (prevArcheTypeData != null)
                nextArcheTypeData->TransferEntity(prevArcheTypeData, entity, entityData, _entityDatas);
            else
                nextArcheTypeData->AddEntity(entity, entityData);

            if (config.IsUnique)
                nextArcheTypeData->SetUniqueComponent(config, &component, _uniqueComponents);
            else
                nextArcheTypeData->SetComponent(entityData, config, &component);
        }

        private unsafe TComponent GetComponentPostCheck<TComponent>(Entity entity, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            var entityData = &_entityDatas[entity.Id];
            var archeTypeData = entityData->ComponentArcheTypeData;

            if (config.IsUnique)
                return *(TComponent*)archeTypeData->GetUniqueComponent(config, _uniqueComponents);
            return *(TComponent*)archeTypeData->GetComponent(entityData, config);
        }

        private unsafe void ReplaceComponentPostCheck<TComponent>(Entity entity, TComponent component, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            var entityData = &_entityDatas[entity.Id];
            var prevArcheTypeData = entityData->ComponentArcheTypeData;

            if (config.IsUnique)
            {
                prevArcheTypeData->SetUniqueComponent(config, &component, _uniqueComponents);
            }
            else if (config.IsShared)
            {
                var nextSharedIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component);
                if (!prevArcheTypeData->ArcheType.HasSharedIndex(config, nextSharedIndex))
                {
                    // Transfer entity since current archeTypeData does not have sharedComponent
                    var nextArcheType = Component_ArcheType_Native.ReplaceSharedComponent(
                        prevArcheTypeData->ArcheType,
                        config,
                        nextSharedIndex);

                    if (!_archeTypeFactory.GetArcheTypeData(nextArcheType, _uniqueConfigs, out var nextArcheTypeData))
                        nextArcheType.Dispose();

                    nextArcheTypeData->TransferEntity(prevArcheTypeData, entity, entityData, _entityDatas);
                    nextArcheTypeData->SetComponent(entityData, config, &component);
                }
                else
                {
                    prevArcheTypeData->SetComponent(entityData, config, &component);
                }
            }
            else
            {
                prevArcheTypeData->SetComponent(entityData, config, &component);
            }
        }

        private unsafe void RemoveComponentPostCheck<TComponent>(Entity entity, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            var entityData = &_entityDatas[entity.Id];
            var prevArcheTypeData = entityData->ComponentArcheTypeData;

            Component_ArcheType_Native nextArcheType;
            if (prevArcheTypeData->ArcheType.ComponentConfigLength == 1)
            {
                // Only has one component, no need to check other configs
                prevArcheTypeData->RemoveEntity(entityData, _entityDatas);
                if (config.IsUnique)
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
            }
            else
            {
                if (config.IsUnique)
                {
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                    nextArcheType = Component_ArcheType_Native.RemoveComponent(prevArcheTypeData->ArcheType, config);
                }
                else
                {
                    nextArcheType = config.IsShared
                        ? Component_ArcheType_Native.RemoveSharedComponent(prevArcheTypeData->ArcheType, config)
                        : Component_ArcheType_Native.RemoveComponent(prevArcheTypeData->ArcheType, config);
                }

                if (!_archeTypeFactory.GetArcheTypeData(nextArcheType, _uniqueConfigs, out var nextArcheTypeData))
                    nextArcheType.Dispose();
                nextArcheTypeData->TransferEntity(prevArcheTypeData, entity, entityData, _entityDatas);
            }
        }
    }
}
