using EcsLte.Data.Unmanaged;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace EcsLte.Native
{
    public class ComponentEntityFactory_Native : IComponentEntityFactory
    {
        private static readonly int _initEntityLength = 4;

        private readonly unsafe ComponentConfigOffset_Native* _configs;
        private int _componentTotalSizeInBytes;
        private unsafe Entity* _entities;
        private unsafe EntityData_Native* _entityDatas;
        private int _entitiesCount;
        private int _entitiesLength;
        private readonly Queue<PtrWrapper> _componentsCache;
        private Entity[] _cachedEntites;
        private bool _cachedEntitiesDirty;
        private unsafe Entity* _reusableEntities;
        private int _reusableEntitiesCount;
        private int _reusableEntitiesLength;
        private unsafe Entity* _uniqueComponentEntities;
        private int _nextId;

        public int Count => _entitiesCount;
        public int Capacity => _entitiesLength;

        public unsafe ComponentEntityFactory_Native()
        {
            _configs = MemoryHelper.Alloc<ComponentConfigOffset_Native>(ComponentConfigs.Instance.AllComponentCount);
            var offsetInBytes = 0;
            for (var i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
            {
                var config = ComponentConfigs.Instance.AllComponentConfigs[i];
                _configs[i] = new ComponentConfigOffset_Native
                {
                    Config = config,
                    OffsetInBytes = offsetInBytes,
                };
                offsetInBytes += config.UnmanagedInBytesSize + 1;
            }
            _componentTotalSizeInBytes = offsetInBytes;

            _entities = MemoryHelper.Alloc<Entity>(_initEntityLength);
            _entityDatas = MemoryHelper.Alloc<EntityData_Native>(_initEntityLength);
            _entitiesCount = 0;
            _entitiesLength = _initEntityLength;

            _componentsCache = new Queue<PtrWrapper>();

            //_cachedEntites
            _cachedEntitiesDirty = true;

            _reusableEntities = MemoryHelper.Alloc<Entity>(_initEntityLength);
            _reusableEntitiesCount = 0;
            _reusableEntitiesLength = _initEntityLength;

            _uniqueComponentEntities = MemoryHelper.Alloc<Entity>(ComponentConfigs.Instance.UniqueComponentCount);

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
                foreach (var pair in ((EntityBlueprint_Native)blueprint).GetComponentConfigsAndDataOrdered())
                    AddComponentPostCheck(entity, entityData, pair.Value.GetData(), pair.Key);
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
                var componentConfigsAndDatas = ((EntityBlueprint_Native)blueprint).GetComponentConfigsAndDataOrdered();
                foreach (var pair in componentConfigsAndDatas)
                {
                    if (pair.Key.IsUnique)
                    {
                        var config = pair.Key;
                        var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                        if (uniqueEntity == Entity.Null)
                            uniqueEntity = AllocateEntity(out _);

                        throw new EntityAlreadyHasComponentException(uniqueEntity,
                            ComponentConfigs.Instance.AllComponentTypes[pair.Key.ComponentIndex]);
                    }
                }

                var components = (byte*)MemoryHelper.Alloc(_componentTotalSizeInBytes);
                for (var i = 0; i < componentConfigsAndDatas.Length; i++)
                {
                    var pair = componentConfigsAndDatas[i];
                    var configOffset = _configs[pair.Key.ComponentIndex];
                    var hasComPtr = components + configOffset.OffsetInBytes;
                    var comPtr = hasComPtr + 1;

                    MemoryHelper.Copy(
                        pair.Value.GetData(),
                        comPtr,
                        pair.Key.UnmanagedInBytesSize);
                    *hasComPtr = 1;
                }

                for (var i = 0; i < count; i++)
                {
                    entities[i] = AllocateEntity(out var entityData);
                    MemoryHelper.Copy(
                        components,
                        entityData->Components,
                        _componentTotalSizeInBytes);
                }
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

            var entityData = &_entityDatas[entity.Id];
            CacheComponents(entityData->Components);
            entityData->Components = null;

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

                var entityData = &_entityDatas[entity.Id];
                CacheComponents(entityData->Components);
                entityData->Components = null;

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

            return *(_entityDatas[entity.Id].Components + _configs[config.ComponentIndex].OffsetInBytes) != 0;
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

            var entityData = _entityDatas[entity.Id];
            var components = new IComponent[entityData.ComponentCount];
            var componentsIndex = 0;
            for (var i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
            {
                var configOffset = _configs[i];
                var hasComPtr = entityData.Components + configOffset.OffsetInBytes;
                var comPtr = hasComPtr + 1;
                if (*hasComPtr == 1)
                {
                    components[componentsIndex++] = (IComponent)Marshal.PtrToStructure(
                        (IntPtr)comPtr,
                        ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
                    if (componentsIndex == components.Length)
                        break;
                }
            }

            return components;
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

            return GetComponentPostCheck<TComponentUnique>(_uniqueComponentEntities[config.UniqueIndex], config);
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
            {
                AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config);
                return;
            }

            var configOffset = _configs[ComponentConfig<TComponent>.Config.ComponentIndex];

            *(TComponent*)(_entityDatas[entity.Id].Components + configOffset.OffsetInBytes + 1) = component;
        }

        public unsafe void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            var configOffset = _configs[config.ComponentIndex];
            var entityData = &_entityDatas[entity.Id];
            var hasComPtr = _entityDatas[entity.Id].Components + configOffset.OffsetInBytes;
            var comPtr = hasComPtr + 1;

            if (config.IsUnique)
                _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;

            MemoryHelper.Clear(comPtr, config.UnmanagedInBytesSize);
            *hasComPtr = 0;
            entityData->ComponentCount--;
        }

        public unsafe void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = &_entityDatas[entity.Id];
            MemoryHelper.Clear(entityData->Components, _componentTotalSizeInBytes);
            entityData->ComponentCount = 0;
        }

        public unsafe Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityAlreadyHasComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = CreateEntity(null);
            AddComponentPostCheck(entity, componentUnique, config);
            _uniqueComponentEntities[config.UniqueIndex] = entity;

            return entity;
        }

        public unsafe Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(newComponentUnique);

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            var configOffset = _configs[config.ComponentIndex];

            *(TComponentUnique*)(_entityDatas[entity.Id].Components + configOffset.OffsetInBytes) = newComponentUnique;

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
            var configOffset = _configs[config.ComponentIndex];
            var entityData = &_entityDatas[entity.Id];

            MemoryHelper.Clear(entityData->Components + configOffset.OffsetInBytes, config.UnmanagedInBytesSize + 1);
            entityData->ComponentCount--;
            _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
        }

        public unsafe void Dispose()
        {
            MemoryHelper.Free(_configs);
            _componentTotalSizeInBytes = 0;
            MemoryHelper.Free(_entities);
            for (var i = 0; i < _entitiesCount; i++)
                MemoryHelper.Free(_entityDatas[i].Components);
            MemoryHelper.Free(_entityDatas);
            _entitiesCount = 0;
            _entitiesLength = 0;
            foreach (var cache in _componentsCache)
                MemoryHelper.Free(cache.Ptr);
            _componentsCache.Clear();
            _cachedEntites = null;
            _cachedEntitiesDirty = true;
            MemoryHelper.Free(_reusableEntities);
            _reusableEntitiesCount = 0;
            _reusableEntitiesLength = 0;
            MemoryHelper.Free(_uniqueComponentEntities);
            _uniqueComponentEntities = null;
            _nextId = 0;
        }

        private unsafe void CheckUnusedCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = Capacity - ((Count + 1) - _reusableEntitiesCount);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(Capacity + count, 2) + 1);
                _entities = (Entity*)MemoryHelper.Realloc(
                    _entities,
                    _entitiesLength * TypeCache<Entity>.SizeInBytes,
                    newCapacity * TypeCache<Entity>.SizeInBytes);
                _entityDatas = (EntityData_Native*)MemoryHelper.Realloc(
                    _entityDatas,
                    _entitiesLength * TypeCache<EntityData_Native>.SizeInBytes,
                    newCapacity * TypeCache<EntityData_Native>.SizeInBytes);

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

        private unsafe Entity AllocateEntity(out EntityData_Native* entityData)
        {
            Entity entity;
            if (_reusableEntitiesCount > 0)
            {
                entity = _reusableEntities[--_reusableEntitiesCount];
                entity.Version++;
                _entityDatas[entity.Id] = new EntityData_Native();
            }
            else
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
            }
            entityData = &_entityDatas[entity.Id];

            _entities[entity.Id] = entity;
            entityData->Components = GetCacheComponents();
            _entitiesCount++;

            return entity;
        }

        private unsafe void AddComponentPostCheck<TComponent>(Entity entity, TComponent component, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        typeof(TComponent));
                }

                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }

            var configOffset = _configs[config.ComponentIndex];
            var entityData = &_entityDatas[entity.Id];
            var hasComPtr = entityData->Components + configOffset.OffsetInBytes;
            var comPtr = hasComPtr + 1;

            *(TComponent*)comPtr = component;
            *hasComPtr = 1;
            entityData->ComponentCount++;
        }

        private unsafe void AddComponentPostCheck(Entity entity, EntityData_Native* entityData, void* component, ComponentConfig config)
        {
            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }

                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }

            var hasComPtr = entityData->Components + _configs[config.ComponentIndex].OffsetInBytes;
            var comPtr = hasComPtr + 1;

            MemoryHelper.Copy(
                component,
                comPtr,
                config.UnmanagedInBytesSize);
            *hasComPtr = 1;
            entityData->ComponentCount++;
        }

        private unsafe TComponent GetComponentPostCheck<TComponent>(Entity entity, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            var configOffset = _configs[config.ComponentIndex];

            return *(TComponent*)(_entityDatas[entity.Id].Components + configOffset.OffsetInBytes + 1);
        }

        private unsafe byte* GetCacheComponents()
        {
            if (_componentsCache.Count > 0)
                return (byte*)_componentsCache.Dequeue().Ptr;
            else
                return (byte*)MemoryHelper.Alloc(_componentTotalSizeInBytes);
        }

        private unsafe void CacheComponents(byte* components)
        {
            MemoryHelper.Clear(components, _componentTotalSizeInBytes);
            _componentsCache.Enqueue(new PtrWrapper { Ptr = components });
        }
    }
}
