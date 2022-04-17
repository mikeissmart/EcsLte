using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.HybridArcheType
{
    internal unsafe class ComponentEntityFactory_Hybrid : IDisposable
    {
        private static readonly int _entitiesInitCapacity = 4;

        private DataCache<Entity[], Entity[]> _entities;
        private EntityData_Hybrid[] _entityDatas;
        private int _entitiesCount;
        private int _nextId;
        private Stack<Entity> _reusableEntities;
        private Entity[] _uniqueComponentEntities;
        private ArcheTypeFactory_Hybrid _archeTypeFactory;
        private IIndexDictionary[] _sharedComponentIndexes;

        internal int Count => _entitiesCount;
        internal int Capacity => _entities.UncachedData.Length;

        internal ComponentEntityFactory_Hybrid()
        {
            _entities = new DataCache<Entity[], Entity[]>(UpdateCachedEntities,
                new Entity[_entitiesInitCapacity],
                null);
            _entityDatas = new EntityData_Hybrid[_entitiesInitCapacity];
            _entitiesCount = 0;
            _nextId = 1;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.UniqueComponentCount];
            _archeTypeFactory = new ArcheTypeFactory_Hybrid();
            _sharedComponentIndexes = IndexDictionary.CreateSharedComponentIndexDictionaries();
        }

        internal Entity[] GetEntities() => _entities.CachedData;

        internal bool HasEntity(Entity entity) => entity.Id > 0 &&
                entity.Id < Capacity &&
                _entities.UncachedData[entity.Id] == entity;

        internal Entity CreateEntity(EntityBlueprint_Hybrid blueprint)
        {
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllBlueprintComponents == null)
                throw new ArgumentNullException(nameof(blueprint.AllBlueprintComponents));

            CheckCapacity(1);

            var entity = AllocateEntity();
            if (blueprint.ArcheTypeData == null || blueprint.ComponentEntityFactory != this)
            {
                blueprint.ComponentEntityFactory = this;
                blueprint.ArcheTypeData = _archeTypeFactory.GetArcheTypeDataFromBlueprint(blueprint, _sharedComponentIndexes);
            }

            foreach (var blueprintComponent in blueprint.UniqueBlueprintComponents)
            {
                if (_uniqueComponentEntities[blueprintComponent.Config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[blueprintComponent.Config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[blueprintComponent.Config.ComponentIndex]);
                }

                _uniqueComponentEntities[blueprintComponent.Config.UniqueIndex] = entity;
            }

            var tempComponentBuffer = stackalloc byte[blueprint.ArcheTypeData->ComponentsSizeInBytes];
            blueprint.ArcheTypeData->CopyBlueprintComponentsBuffer(blueprint.AllBlueprintComponents, tempComponentBuffer);
            _entityDatas[entity.Id] = blueprint.ArcheTypeData->AddEntity(entity, tempComponentBuffer);
            _entities.SetDirty();

            return entity;
        }

        internal Entity[] CreateEntities(int count, EntityBlueprint_Hybrid blueprint)
        {
            if (count == 1)
                return new Entity[] { CreateEntity(blueprint) };
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllBlueprintComponents == null)
                throw new ArgumentNullException(nameof(blueprint.AllBlueprintComponents));
            if (count == 0)
                return new Entity[count];

            CheckCapacity(count);

            if (blueprint.ArcheTypeData == null || blueprint.ComponentEntityFactory != this)
            {
                blueprint.ComponentEntityFactory = this;
                blueprint.ArcheTypeData = _archeTypeFactory.GetArcheTypeDataFromBlueprint(blueprint, _sharedComponentIndexes);
            }

            if (blueprint.UniqueBlueprintComponents.Length > 0)
            {
                var config = blueprint.UniqueBlueprintComponents[0].Config;
                var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                if (uniqueEntity == Entity.Null)
                    uniqueEntity = AllocateEntity();

                throw new EntityAlreadyHasComponentException(
                    uniqueEntity,
                    ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
            }

            var tempComponentBuffer = stackalloc byte[blueprint.ArcheTypeData->ComponentsSizeInBytes];
            blueprint.ArcheTypeData->CopyBlueprintComponentsBuffer(blueprint.AllBlueprintComponents, tempComponentBuffer);
            blueprint.ArcheTypeData->PreCheckEntityStateAllocation(count);

            var entities = new Entity[count];
            for (var i = 0; i < count; i++)
            {
                var entity = AllocateEntity();
                entities[i] = entity;
                _entityDatas[entity.Id] = blueprint.ArcheTypeData->AddEntity(entity, tempComponentBuffer);
            }
            _entities.SetDirty();

            return entities;
        }

        internal void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var archeTypeData = _entityDatas[entity.Id].ArcheTypeData;
            var archeType = archeTypeData->ArcheType;
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                if (config.IsUnique)
                    _uniqueComponentEntities[entity.Id] = Entity.Null;
            }
            _entityDatas[entity.Id] = archeTypeData->RemoveEntity(entity, ref _entityDatas);
            _entities.UncachedData[entity.Id] = Entity.Null;
            _reusableEntities.Push(entity);

            _entitiesCount--;
            _entities.SetDirty();
        }

        internal void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var archeTypeData = _entityDatas[entity.Id].ArcheTypeData;
                var archeType = archeTypeData->ArcheType;
                for (var i = 0; i < archeType.ComponentConfigLength; i++)
                {
                    var config = archeType.ComponentConfigs[i];
                    if (config.IsUnique)
                        _uniqueComponentEntities[entity.Id] = Entity.Null;
                }
                _entityDatas[entity.Id] = archeTypeData->RemoveEntity(entity, ref _entityDatas);
                _entities.UncachedData[entity.Id] = Entity.Null;
                _reusableEntities.Push(entity);
            }
            _entitiesCount -= entities.Count();
            _entities.SetDirty();
        }

        internal bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var config = ComponentConfig<TComponent>.Config;
            var entityData = _entityDatas[entity.Id];

            return entityData.ArcheTypeData->ArcheType.HasComponentConfig(config);
        }

        internal TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var entityData = _entityDatas[entity.Id];
            return *(TComponent*)entityData.ArcheTypeData->GetComponent(entityData, ComponentConfig<TComponent>.Config);
        }

        internal IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return entityData.ArcheTypeData->GetAllComponents(entityData);
        }

        internal bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            var config = ComponentConfig<TComponentUnique>.Config;

            return _uniqueComponentEntities[config.UniqueIndex] != Entity.Null;
        }

        internal TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;

            var entityData = _entityDatas[_uniqueComponentEntities[config.UniqueIndex].Id];
            return *(TComponentUnique*)entityData.ArcheTypeData->GetComponent(entityData, config);
        }

        internal Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        internal void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            var entityData = _entityDatas[entity.Id];
            if (config.IsShared)
            {
                var prevArcheType = entityData.ArcheTypeData->ArcheType;
                var nextArcheType = prevArcheType.Clone();
                nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                {
                    SharedIndex = config.SharedIndex,
                    SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                });
                var nextArcheTypeData = _archeTypeFactory.GetArcheTypeDataFromArcheType(ref nextArcheType);
                if (nextArcheTypeData != entityData.ArcheTypeData)
                    ArcheTypeData_Hybrid.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);
            }

            entityData.ArcheTypeData->SetComponent(entityData, &component, ComponentConfig<TComponent>.Config);
        }

        public void Dispose()
        {
            _entities = null;
            _entityDatas = null;
            _entitiesCount = 0;
            _nextId = 0;
            _reusableEntities = null;
            _uniqueComponentEntities = null;
            _archeTypeFactory.Dispose();
            _archeTypeFactory = null;
            _sharedComponentIndexes = null;
        }

        private void CheckCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = Capacity - ((Count + 1) - _reusableEntities.Count);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(Capacity + count, 2) + 1);
                Array.Resize(ref _entities.UncachedData, newCapacity);
                Array.Resize(ref _entityDatas, newCapacity);
            }
        }

        private Entity AllocateEntity()
        {
            Entity entity;
            if (_reusableEntities.Count > 0)
            {
                entity = _reusableEntities.Pop();
                entity.Version++;
            }
            else
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
            }
            _entities.UncachedData[entity.Id] = entity;
            _entitiesCount++;

            return entity;
        }

        private Entity[] UpdateCachedEntities(Entity[] uncachedData)
            => uncachedData
                .Where(x => x.Id != 0)
                .ToArray();
    }
}
