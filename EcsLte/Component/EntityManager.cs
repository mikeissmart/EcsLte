using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public unsafe class EntityManager
    {
        private Entity[] _entities;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData[] _entityDatas;
        private int _nextId;
        private Stack<Entity> _reusableEntities;
        private Entity[] _uniqueComponentEntities;

        public EcsContext Context { get; private set; }
        public int EntityCount { get; private set; }
        public int EntityCapacity => _entities.Length;
        internal EntityData[] EntityData => _entityDatas;
        internal IIndexDictionary[] SharedComponentIndexes { get; private set; }

        internal EntityManager(EcsContext context)
        {
            _entities = new Entity[1];
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _entityDatas = new EntityData[1];
            _nextId = 1;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.AllUniqueComponentCount];

            Context = context;
            SharedComponentIndexes = IndexDictionary.CreateSharedComponentIndexDictionaries();
        }

        public bool HasEntity(Entity entity)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return entity.Id > 0 &&
                entity.Id < EntityCapacity &&
                _entities[entity.Id] == entity;
        }

        public bool HasEntity(Entity entity, EntityArcheType entityArcheType)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            if (!HasEntity(entity))
                return false;

            var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
            return _entityDatas[entity.Id].ArcheTypeData == archeTypeData;
        }

        public Entity[] GetEntities()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

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

        public Entity[] GetEntities(EntityArcheType entityArcheType)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
            var entities = new Entity[archeTypeData->EntityCount];
            archeTypeData->CopyEntities(ref entities, 0);

            return entities;
        }

        public Entity CreateEntity(EntityBlueprint blueprint)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllBlueprintComponents == null)
                throw new ArgumentNullException(nameof(blueprint.AllBlueprintComponents));

            CheckCapacity(1);

            var entity = AllocateEntity();
            var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromBlueprint(blueprint);

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

            var tempComponentBuffer = stackalloc byte[archeTypeData->ComponentsSizeInBytes];
            archeTypeData->CopyBlueprintComponentsBuffer(blueprint.AllBlueprintComponents, tempComponentBuffer);
            _entityDatas[entity.Id] = archeTypeData->AddEntity(entity, tempComponentBuffer);

            return entity;
        }

        public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
        {
            if (count == 1)
                return new Entity[] { CreateEntity(blueprint) };
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllBlueprintComponents == null)
                throw new ArgumentNullException(nameof(blueprint.AllBlueprintComponents));
            if (count == 0)
                return new Entity[count];

            CheckCapacity(count);

            var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromBlueprint(blueprint);

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

            var tempComponentBuffer = stackalloc byte[archeTypeData->ComponentsSizeInBytes];
            archeTypeData->CopyBlueprintComponentsBuffer(blueprint.AllBlueprintComponents, tempComponentBuffer);
            archeTypeData->PreCheckEntityStateAllocation(count);

            var entities = new Entity[count];
            for (var i = 0; i < count; i++)
            {
                var entity = AllocateEntity();
                entities[i] = entity;
                _entityDatas[entity.Id] = archeTypeData->AddEntity(entity, tempComponentBuffer);
            }

            return entities;
        }

        public void DestroyEntity(Entity entity)
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

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

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
                        _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                }
                _entityDatas[entity.Id] = archeTypeData->RemoveEntity(entity, ref _entityDatas);
                _entities[entity.Id] = Entity.Null;
                _reusableEntities.Push(entity);
            }
            _isCachedEntitiesDirty = true;
            EntityCount -= entities.Count();
        }

        public void DestroyEntities(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
            var archeType = archeTypeData->ArcheType;
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                if (config.IsUnique)
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
            }
            for (var i = 0; i < archeTypeData->EntityCount; i++)
            {
                var entity = archeTypeData->GetEntity(i);
                _entityDatas[entity.Id] = new EntityData();
                _entities[entity.Id] = Entity.Null;
                _reusableEntities.Push(entity);
            }
            _isCachedEntitiesDirty = true;
            EntityCount -= archeTypeData->EntityCount;
            archeTypeData->ClearAllEntities();
        }

        public bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return entityData.ArcheTypeData->ArcheType.HasComponentConfig(ComponentConfig<TComponent>.Config);
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var entityData = _entityDatas[entity.Id];
            return *(TComponent*)entityData.ArcheTypeData->GetComponentPtr(entityData, ComponentConfig<TComponent>.Config);
        }

        public TComponent[] GetComponents<TComponent>(EntityArcheType entityArcheType) where TComponent : unmanaged, IComponent
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (!entityArcheType.HasComponent<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            return Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType)
                ->GetAllComponentTypes<TComponent>(ComponentConfig<TComponent>.Config);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return entityData.ArcheTypeData->GetAllComponents(entityData);
        }

        public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex] != Entity.Null;
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
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

        public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        public void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            UpdateComponentNoCheck(entity, _entityDatas[entity.Id], component);
        }

        public void UpdateComponent<TComponent>(EntityQuery query, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

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
                        SharedDataIndex = SharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                    });
                    var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref nextArcheType);
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

        public void UpdateComponent<TComponent>(IEnumerable<Entity> entities, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (entities.Count() == 0)
                return;

            if (entities.Count() == 1)
            {
                UpdateComponent(entities.First(), component);
                return;
            }

            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
            {
                ArcheTypeData* prevArcheTypeData = null;
                ArcheTypeData* nextArcheTypeData = null;
                var nextConfigOffset = new ComponentConfigOffset();
                foreach (var entity in entities)
                {
                    if (!HasComponent<TComponent>(entity))
                        throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                    var entityData = _entityDatas[entity.Id];
                    if (prevArcheTypeData != entityData.ArcheTypeData)
                    {
                        prevArcheTypeData = entityData.ArcheTypeData;
                        var nextArcheType = prevArcheTypeData->ArcheType.Clone();
                        nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                        {
                            SharedIndex = config.SharedIndex,
                            SharedDataIndex = SharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                        });
                        nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref nextArcheType);
                        nextArcheTypeData->GetComponentOffset(config, out nextConfigOffset);
                    }

                    if (prevArcheTypeData != nextArcheTypeData)
                    {
                        ArcheTypeData.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);
                        entityData = _entityDatas[entity.Id];
                    }

                    nextArcheTypeData->SetComponent(entityData, &component, nextConfigOffset);
                }
            }
            else
            {
                ArcheTypeData* prevArcheTypeData = null;
                var configOffset = new ComponentConfigOffset();
                foreach (var entity in entities)
                {
                    if (!HasComponent<TComponent>(entity))
                        throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                    var entityData = _entityDatas[entity.Id];
                    if (prevArcheTypeData != entityData.ArcheTypeData)
                    {
                        prevArcheTypeData = entityData.ArcheTypeData;
                        prevArcheTypeData->GetComponentOffset(config, out configOffset);
                    }

                    prevArcheTypeData->SetComponent(entityData, &component, configOffset);
                }
            }
        }

        public void UpdateComponent<TComponent>(EntityArcheType entityArcheType, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
            {
                var prevArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
                var nextArcheType = prevArcheTypeData->ArcheType.Clone();
                nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                {
                    SharedIndex = config.SharedIndex,
                    SharedDataIndex = SharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                });
                var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref nextArcheType);
                if (nextArcheTypeData != prevArcheTypeData)
                    ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, ref _entityDatas);

                nextArcheTypeData->SetAllComponents(&component, config);
            }
            else
            {
                Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType)
                    ->SetAllComponents(&component, config);
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
                    SharedDataIndex = SharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                });
                var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref nextArcheType);
                if (nextArcheTypeData != entityData.ArcheTypeData)
                {
                    ArcheTypeData.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);
                    entityData = _entityDatas[entity.Id];
                }
            }

            entityData.ArcheTypeData->SetComponent(entityData, &component, ComponentConfig<TComponent>.Config);
        }

        internal SharedComponentDataIndex GetSharedComponentDataIndex<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            return new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = SharedComponentIndexes[config.SharedIndex].GetIndexObj(component)
            };
        }

        internal SharedComponentDataIndex GetSharedComponentDataIndex(ISharedComponent component, ComponentConfig config) => new SharedComponentDataIndex
        {
            SharedIndex = config.SharedIndex,
            SharedDataIndex = SharedComponentIndexes[config.SharedIndex].GetIndexObj(component)
        };

        internal void InternalDestroy()
        {
            _entities = null;
            _cachedEntities = null;
            _isCachedEntitiesDirty = true;
            _entityDatas = null;
            _nextId = 0;
            _reusableEntities = null;
            _uniqueComponentEntities = null;
            ;
            SharedComponentIndexes = null;
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
