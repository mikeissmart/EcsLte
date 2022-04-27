using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte
{
    internal unsafe class ComponentEntityFactory : IDisposable
    {
        private Entity[] _entities;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData[] _entityDatas;
        private int _nextId;
        private Stack<Entity> _reusableEntities;
        private Entity[] _uniqueComponentEntities;
        private ArcheTypeFactory _archeTypeFactory;
        private IIndexDictionary[] _sharedComponentIndexes;

        internal int EntityCount { get; private set; }
        internal int EntityCapacity { get => _entities.Length; }
        internal EntityData[] EntityData { get => _entityDatas; }

        internal ComponentEntityFactory()
        {
            _entities = new Entity[1];
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _entityDatas = new EntityData[1];
            _nextId = 1;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.UniqueComponentCount];
            _sharedComponentIndexes = IndexDictionary.CreateSharedComponentIndexDictionaries();
        }

        internal void SetDependentFactories(ArcheTypeFactory archeTypeFactory)
        {
            _archeTypeFactory = archeTypeFactory;
        }

        internal bool HasEntity(Entity entity)
        {
            return entity.Id > 0 &&
                entity.Id < EntityCapacity &&
                _entities[entity.Id] == entity;
        }

        internal Entity[] GetEntities()
        {
            if (_isCachedEntitiesDirty)
            {
                if (_cachedEntities.Length != EntityCount)
                    _cachedEntities = new Entity[EntityCount];
                for (int i = 1, j = 0; i < _nextId; i++)
                {
                    var entity = _entities[i];
                    if (entity.Id > 0)
                        _cachedEntities[j++] = entity;
                }
                _isCachedEntitiesDirty = false;
            }

            return _cachedEntities;
        }

        internal Entity CreateEntity(EntityBlueprint blueprint)
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

            return entity;
        }

        internal Entity[] CreateEntities(int count, EntityBlueprint blueprint)
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
            _entities[entity.Id] = Entity.Null;
            _reusableEntities.Push(entity);
            _isCachedEntitiesDirty = true;
            EntityCount--;
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
                _entities[entity.Id] = Entity.Null;
                _reusableEntities.Push(entity);
            }
            _isCachedEntitiesDirty = true;
            EntityCount -= entities.Count();
        }

        internal bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return entityData.ArcheTypeData->ArcheType.HasComponentConfig(ComponentConfig<TComponent>.Config);
        }

        internal TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var entityData = _entityDatas[entity.Id];
            return *(TComponent*)entityData.ArcheTypeData->GetComponentPtr(entityData, ComponentConfig<TComponent>.Config);
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
            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex] != Entity.Null;
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
            return *(TComponentUnique*)entityData.ArcheTypeData->GetComponentPtr(entityData, config);
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

            UpdateComponentNoCheck(entity, _entityDatas[entity.Id], component);
        }

        internal void UpdateComponent<TComponent>(EntityQuery query, TComponent component) where TComponent : unmanaged, IComponent
        {
            query.CheckQueryData();
            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
            {
                for (var i = 0; i < query.QueryData.ArcheTypeDatas.Count; i++)
                {
                    var prevArcheTypeData = (ArcheTypeData*)query.QueryData.ArcheTypeDatas[i].Ptr;
                    var nextArcheType = prevArcheTypeData->ArcheType.Clone();
                    nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                    {
                        SharedIndex = config.SharedIndex,
                        SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                    });
                    var nextArcheTypeData = _archeTypeFactory.GetArcheTypeDataFromArcheType(ref nextArcheType);
                    if (nextArcheTypeData != prevArcheTypeData)
                        ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, ref _entityDatas);

                    nextArcheTypeData->SetAllComponents(&component, config);
                }
            }
            else
            {
                for (var i = 0; i < query.QueryData.ArcheTypeDatas.Count; i++)
                {
                    ((ArcheTypeData*)query.QueryData.ArcheTypeDatas[i].Ptr)
                        ->SetAllComponents(&component, config);
                }
            }
        }

        internal void UpdateComponentNoCheck<TComponent>(Entity entity, EntityData entityData, TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
            {
                var nextArcheType = entityData.ArcheTypeData->ArcheType.Clone();
                nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                {
                    SharedIndex = config.SharedIndex,
                    SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                });
                var nextArcheTypeData = _archeTypeFactory.GetArcheTypeDataFromArcheType(ref nextArcheType);
                if (nextArcheTypeData != entityData.ArcheTypeData)
                {
                    ArcheTypeData.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);
                    entityData = _entityDatas[entity.Id];
                }
            }

            entityData.ArcheTypeData->SetComponent(entityData, &component, ComponentConfig<TComponent>.Config);
        }

        public void Dispose()
        {
            _entities = null;
            _cachedEntities = null;
            _isCachedEntitiesDirty = true;
            _entityDatas = null;
            _nextId = 0;
            _reusableEntities = null;
            _uniqueComponentEntities = null;
            _archeTypeFactory = null;
            _sharedComponentIndexes = null;
        }

        private void CheckCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = EntityCapacity - ((EntityCount + 1) - _reusableEntities.Count);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(EntityCapacity + count, 2) + 1);
                Array.Resize(ref _entities, newCapacity);
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
            _entities[entity.Id] = entity;
            EntityCount++;

            return entity;
        }
    }
}
