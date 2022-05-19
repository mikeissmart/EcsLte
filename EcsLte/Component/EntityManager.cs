using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace EcsLte
{
    public unsafe class EntityManager
    {
        private Entity[] _entities;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData[] _entityDatas;
        private int _nextId;
        private int _entityCount;
        private Stack<Entity> _reusableEntities;
        private Entity[] _uniqueComponentEntities;
        private ISharedComponentIndexDictionary[] _sharedComponentIndexes;
        private readonly IManagedComponentPool[] _managedComponentPools;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }
        internal EntityData[] EntityDatas => _entityDatas;

        internal EntityManager(EcsContext context)
        {
            _entities = new Entity[1];
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _entityDatas = new EntityData[1];
            _nextId = 1;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.AllUniqueCount];
            _sharedComponentIndexes = SharedComponentIndexDictionary.CreateSharedComponentIndexDictionaries();
            _managedComponentPools = ManagedComponentPool.CreateManagedComponentPools();
            _lockObj = new object();

            Context = context;
        }

        public int EntityCount()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _entityCount;
        }

        public int EntityCount(EntityArcheType entityArcheType)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
            return archeTypeData->EntityCount;
        }

        public int EntityCount(EntityQuery entityQuery)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));

            Context.QueryManager.UpdateEntityQuery(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            var entityCount = 0;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                entityCount += ((ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr)->EntityCount;

            return entityCount;
        }

        public bool HasEntity(Entity entity)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return entity.Id > 0 &&
                entity.Id < _entities.Length &&
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

        public bool HasEntity(Entity entity, EntityQuery entityQuery)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));

            if (!HasEntity(entity))
                return false;

            Context.QueryManager.UpdateEntityQuery(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            var archeTypeData = _entityDatas[entity.Id].ArcheTypeData;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
            {
                if (archeTypeData == (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr)
                    return true;
            }

            return false;
        }

        public Entity[] GetEntities()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (_isCachedEntitiesDirty)
            {
                if (_cachedEntities.Length != EntityCount())
                    _cachedEntities = new Entity[EntityCount()];
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

        public Entity[] GetEntities(EntityQuery entityQuery)
        {
            var entityCount = EntityCount(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            var entities = new Entity[entityCount];
            var entitiesOffset = 0;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
            {
                var archeTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr;
                archeTypeData->CopyEntities(ref entities, entitiesOffset);
                entitiesOffset += archeTypeData->EntityCount;
            }

            return entities;
        }

        public Entity CreateEntity(EntityBlueprint blueprint)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllBlueprintComponents.Length == 0)
                throw new EntityBlueprintNoComponentsException();

            lock (_lockObj)
            {
                CheckCapacity(1);

                var entity = AllocateEntity();
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(blueprint.GetEntityArcheType());

                CheckAndSetUniqueComponents(entity, archeTypeData->ArcheType);
                var tempComponentBuffer = stackalloc byte[archeTypeData->ComponentsSizeInBytes];
                archeTypeData->CopyComponentDatasToBuffer(blueprint.AllBlueprintComponents, tempComponentBuffer);
                _entityDatas[entity.Id] = archeTypeData->AddEntity(entity, tempComponentBuffer);

                return entity;
            }
        }

        public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllBlueprintComponents.Length == 0)
                throw new EntityBlueprintNoComponentsException();
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 1)
                return new Entity[] { CreateEntity(blueprint) };
            if (count == 0)
                return new Entity[count];

            lock (_lockObj)
            {
                CheckCapacity(count);

                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(blueprint.GetEntityArcheType());
                CheckUniqueComponents(archeTypeData->ArcheType);
                var tempComponentBuffer = stackalloc byte[archeTypeData->ComponentsSizeInBytes];
                archeTypeData->CopyComponentDatasToBuffer(blueprint.AllBlueprintComponents, tempComponentBuffer);
                archeTypeData->PreCheckEntityAllocation(count);

                var entities = new Entity[count];
                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity();
                    entities[i] = entity;
                    _entityDatas[entity.Id] = archeTypeData->AddEntity(entity, tempComponentBuffer);
                }

                return entities;
            }
        }

        public void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            lock (_lockObj)
            {
                DestroyEntityNoCheck(entity);
            }
        }

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            lock (_lockObj)
            {
                foreach (var entity in entities)
                {
                    if (!HasEntity(entity))
                        throw new EntityDoesNotExistException(entity);

                    DestroyEntityNoCheck(entity);
                }
            }
        }

        public void DestroyEntities(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
                var archeType = archeTypeData->ArcheType;
                for (var i = 0; i < archeType.ComponentConfigLength; i++)
                {
                    var config = archeType.ComponentConfigs[i];
                    if (config.IsUnique)
                        _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                }
                for (var i = 0; i < archeTypeData->EntityCount; i++)
                    DeallocateEntity(archeTypeData->GetEntity(i));
                archeTypeData->ClearAllEntities();
            }
        }

        public void DestroyEntities(EntityQuery entityQuery)
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            Context.QueryManager.UpdateEntityQuery(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            lock (_lockObj)
            {
                for (var entityQueryIndex = 0; entityQueryIndex < contextQueryData.ArcheTypeDatas.Length; entityQueryIndex++)
                {
                    var archeTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[entityQueryIndex].Ptr;
                    var archeType = archeTypeData->ArcheType;
                    for (var i = 0; i < archeType.ComponentConfigLength; i++)
                    {
                        var config = archeType.ComponentConfigs[i];
                        if (config.IsUnique)
                            _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                    }
                    for (var i = 0; i < archeTypeData->EntityCount; i++)
                        DeallocateEntity(archeTypeData->GetEntity(i));
                    archeTypeData->ClearAllEntities();
                }
            }
        }

        public Entity TransferEntity(EcsContext sourceContext, Entity entity, bool destroyEntity)
        {
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (Context == sourceContext)
                throw new EntityTransferSameEcsContextException(Context);
            if (!sourceContext.EntityManager.HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            Entity nextEntity;
            lock (_lockObj)
            {
                CheckCapacity(1);

                var prevEntityData = sourceContext.EntityManager._entityDatas[entity.Id];
                var prevArcheTypeData = prevEntityData.ArcheTypeData;
                var archeType = prevArcheTypeData->ArcheType.Clone();
                nextEntity = AllocateEntity();

                CheckAndSetUniqueComponents(nextEntity, archeType);
                var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref archeType);
                _entityDatas[nextEntity.Id] = nextArcheTypeData->AddEntity(nextEntity, prevArcheTypeData->GetComponentsPtr(prevEntityData));
            }
            if (destroyEntity)
                sourceContext.EntityManager.DestroyEntity(entity);

            return nextEntity;
        }

        public Entity[] TransferEntities(EcsContext sourceContext, IEnumerable<Entity> entities, bool destroyEntities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (Context == sourceContext)
                throw new EntityTransferSameEcsContextException(Context);
            if (entities.Count() == 0)
                return new Entity[0];
            if (entities.Count() == 1)
                return new Entity[] { TransferEntity(sourceContext, entities.First(), destroyEntities) };

            var nextEntities = new Entity[entities.Count()];
            lock (_lockObj)
            {
                CheckCapacity(entities.Count());

                var nextEntityIndex = 0;
                foreach (var entity in entities)
                {
                    if (!sourceContext.EntityManager.HasEntity(entity))
                        throw new EntityDoesNotExistException(entity);

                    var prevEntityData = sourceContext.EntityManager._entityDatas[entity.Id];
                    var prevArcheTypeData = prevEntityData.ArcheTypeData;
                    var archeType = prevArcheTypeData->ArcheType.Clone();
                    var nextEntity = AllocateEntity();

                    CheckAndSetUniqueComponents(nextEntity, archeType);
                    var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref archeType);
                    nextEntities[nextEntityIndex++] = nextEntity;
                    _entityDatas[nextEntity.Id] = nextArcheTypeData->AddEntity(nextEntity, prevArcheTypeData->GetComponentsPtr(prevEntityData));
                }
            }
            if (destroyEntities)
                sourceContext.EntityManager.DestroyEntities(entities);

            return nextEntities;
        }

        public Entity[] TransferEntities(EcsContext sourceContext, EntityArcheType entityArcheType, bool destroyEntities)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (Context == sourceContext)
                throw new EntityTransferSameEcsContextException(Context);

            var prevArcheTypeData = sourceContext.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
            var entitiesLength = prevArcheTypeData->EntityCount;

            if (entitiesLength == 0)
                return new Entity[0];
            if (entitiesLength == 1)
                return new Entity[] { TransferEntity(sourceContext, prevArcheTypeData->GetEntity(0), destroyEntities) };

            var entities = new Entity[entitiesLength];
            lock (_lockObj)
            {
                CheckCapacity(entitiesLength);

                CheckUniqueComponents(prevArcheTypeData->ArcheType);
                var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);

                nextArcheTypeData->PreCheckEntityAllocation(entitiesLength);
                for (var i = 0; i < entitiesLength; i++)
                {
                    var prevEntity = prevArcheTypeData->GetEntity(i);
                    var prevComponentsPtr = prevArcheTypeData->GetComponentsPtr(sourceContext.EntityManager._entityDatas[prevEntity.Id]);
                    var nextEntity = AllocateEntity();
                    entities[i] = nextEntity;
                    _entityDatas[nextEntity.Id] = nextArcheTypeData->AddEntity(nextEntity, prevComponentsPtr);
                }
            }
            if (destroyEntities)
                sourceContext.EntityManager.DestroyEntities(entityArcheType);

            return entities;
        }

        public Entity[] TransferEntities(EcsContext sourceContext, EntityQuery entityQuery, bool destroyEntities)
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (Context == sourceContext)
                throw new EntityTransferSameEcsContextException(Context);

            var entitiesLength = sourceContext.EntityManager.EntityCount(entityQuery);
            if (entitiesLength == 0)
                return new Entity[0];
            if (entitiesLength == 1)
                return new Entity[] { TransferEntity(sourceContext, sourceContext.EntityManager.GetEntities(entityQuery)[0], destroyEntities) };

            sourceContext.QueryManager.UpdateEntityQuery(entityQuery);
            var sourceContextQueryData = entityQuery.QueryData.ContextQueryData[sourceContext];
            var entities = new Entity[entitiesLength];
            var entityIndex = 0;
            lock (_lockObj)
            {
                CheckCapacity(entitiesLength);

                for (var queryIndex = 0; queryIndex < sourceContextQueryData.ArcheTypeDatas.Length; queryIndex++)
                {
                    var prevArcheTypeData = (ArcheTypeData*)sourceContextQueryData.ArcheTypeDatas[queryIndex].Ptr;
                    var prevArcheType = prevArcheTypeData->ArcheType.Clone();
                    if (prevArcheTypeData->EntityCount > 0)
                    {
                        var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref prevArcheType);

                        nextArcheTypeData->PreCheckEntityAllocation(prevArcheTypeData->EntityCount);
                        if (prevArcheTypeData->EntityCount == 1)
                        {
                            var nextEntity = AllocateEntity();
                            CheckAndSetUniqueComponents(nextEntity, nextArcheTypeData->ArcheType);
                            var prevEntity = prevArcheTypeData->GetEntity(0);
                            var prevComponentsPtr = prevArcheTypeData->GetComponentsPtr(sourceContext.EntityManager._entityDatas[prevEntity.Id]);
                            entities[entityIndex++] = nextEntity;
                            _entityDatas[nextEntity.Id] = nextArcheTypeData->AddEntity(nextEntity, prevComponentsPtr);
                        }
                        else if (prevArcheTypeData->EntityCount > 1)
                        {
                            CheckUniqueComponents(prevArcheTypeData->ArcheType);
                            nextArcheTypeData->PreCheckEntityAllocation(prevArcheTypeData->EntityCount);
                            for (var i = 0; i < prevArcheTypeData->EntityCount; i++)
                            {
                                var prevEntity = prevArcheTypeData->GetEntity(i);
                                var prevComponentsPtr = prevArcheTypeData->GetComponentsPtr(sourceContext.EntityManager._entityDatas[prevEntity.Id]);
                                var nextEntity = AllocateEntity();
                                entities[entityIndex++] = nextEntity;
                                _entityDatas[nextEntity.Id] = nextArcheTypeData->AddEntity(nextEntity, prevComponentsPtr);
                            }
                        }
                    }
                }
            }
            if (destroyEntities)
                sourceContext.EntityManager.DestroyEntities(entityQuery);

            return entities;
        }

        public EntityArcheType GetEntityArcheType(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var archeTypeData = _entityDatas[entity.Id].ArcheTypeData;
            return new EntityArcheType(Context, archeTypeData);
        }

        public EntityArcheType[] GetEntityArcheTypes(EntityQuery entityQuery)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));

            Context.QueryManager.UpdateEntityQuery(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            var entityArcheTypes = new EntityArcheType[contextQueryData.ArcheTypeDatas.Length];
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                entityArcheTypes[i] = new EntityArcheType(Context, (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr);

            return entityArcheTypes;
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
            if (!entityArcheType.HasComponentType<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            return Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType)
                ->GetAllComponentTypes<TComponent>(ComponentConfig<TComponent>.Config);
        }

        public TComponent[] GetComponents<TComponent>(EntityQuery entityQuery) where TComponent : unmanaged, IComponent
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (!entityQuery.HasWhereAllOf<TComponent>())
                throw new EntityQueryNotHaveWhereOfAllException(typeof(TComponent));

            Context.QueryManager.UpdateEntityQuery(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            var components = new TComponent[EntityCount(entityQuery)];
            var config = ComponentConfig<TComponent>.Config;
            var entityIndex = 0;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
            {
                var archeTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr;
                archeTypeData->GetAllComponentTypes(ref components, entityIndex, config);
                entityIndex += archeTypeData->EntityCount;
            }

            return components;
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

            lock (_lockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                var entityData = _entityDatas[entity.Id];
                if (config.IsShared)
                {
                    var nextArcheType = entityData.ArcheTypeData->ArcheType.Clone();
                    nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                    {
                        SharedIndex = config.SharedIndex,
                        SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
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

            lock (_lockObj)
            {
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
                                SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
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
        }

        public void UpdateComponent<TComponent>(EntityArcheType entityArcheType, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (!entityArcheType.HasComponentType<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            lock (_lockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                if (config.IsShared)
                {
                    var prevArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromEntityArcheType(entityArcheType);
                    var nextArcheType = prevArcheTypeData->ArcheType.Clone();
                    nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                    {
                        SharedIndex = config.SharedIndex,
                        SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
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
        }

        public void UpdateComponent<TComponent>(EntityQuery entityQuery, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            if (!entityQuery.HasWhereAllOf<TComponent>())
                throw new EntityQueryNotHaveWhereOfAllException(typeof(TComponent));

            Context.QueryManager.UpdateEntityQuery(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[Context];
            lock (_lockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                if (config.IsShared)
                {
                    for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                    {
                        var prevArcheTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr;
                        var nextArcheType = prevArcheTypeData->ArcheType.Clone();
                        nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                        {
                            SharedIndex = config.SharedIndex,
                            SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component),
                        });
                        var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref nextArcheType);
                        if (nextArcheTypeData != prevArcheTypeData)
                            ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, ref _entityDatas);

                        nextArcheTypeData->SetAllComponents(&component, config);
                    }
                }
                else
                {
                    for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                    {
                        ((ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr)
                            ->SetAllComponents(&component, config);
                    }
                }
            }
        }

        internal void UpdateForEachComponents(Entity entity, EntityData entityData, ComponentConfigOffset[] configOffsets, IComponent[] components)
        {
            if (!HasEntity(entity))
                return;

            lock (_lockObj)
            {
                var largestSizeInBytes = configOffsets
                    .Take(components.Length)
                    .OrderByDescending(x => x.Config.UnmanagedSizeInBytes)
                    .First()
                    .Config.UnmanagedSizeInBytes;
                var componentPtr = stackalloc byte[largestSizeInBytes];
                for (var i = 0; i < components.Length; i++)
                {
                    var configOffset = configOffsets[i];
                    var component = components[i];
                    if (configOffset.Config.IsShared)
                    {
                        var nextArcheType = entityData.ArcheTypeData->ArcheType.Clone();
                        nextArcheType.ReplaceSharedComponentDataIndex(new SharedComponentDataIndex
                        {
                            SharedIndex = configOffset.Config.SharedIndex,
                            SharedDataIndex = _sharedComponentIndexes[configOffset.Config.SharedIndex].GetIndexObj(component),
                        });
                        var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeDataFromArcheType(ref nextArcheType);
                        if (nextArcheTypeData != entityData.ArcheTypeData)
                        {
                            ArcheTypeData.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);
                            entityData = _entityDatas[entity.Id];
                        }
                    }

                    Marshal.StructureToPtr(component, (IntPtr)componentPtr, false);
                    entityData.ArcheTypeData->SetComponent(entityData, componentPtr, configOffset);
                }
            }
        }

        internal SharedComponentDataIndex GetSharedComponentDataIndex<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            return new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component)
            };
        }

        internal SharedComponentDataIndex GetSharedComponentDataIndex(ISharedComponent component, ComponentConfig config) =>
            new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component)
            };

        internal IComponentData GetSharedComponentData(SharedComponentDataIndex dataIndex) => _sharedComponentIndexes[dataIndex.SharedIndex].GetComponentData(dataIndex);

        internal void InternalDestroy()
        {
            lock (_lockObj)
            {
                _entities = null;
                _cachedEntities = null;
                _isCachedEntitiesDirty = true;
                _entityDatas = null;
                _nextId = 0;
                _reusableEntities = null;
                _uniqueComponentEntities = null;
                _sharedComponentIndexes = null;
                _entityCount = 0;
            }
        }

        private void CheckCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = _entities.Length - ((_entityCount + 1) - _reusableEntities.Count);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_entities.Length + count, 2) + 1);
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
            _isCachedEntitiesDirty = true;
            _entityCount++;

            return entity;
        }

        private void DeallocateEntity(Entity entity)
        {
            _entityDatas[entity.Id] = new EntityData();
            _entities[entity.Id] = Entity.Null;
            _reusableEntities.Push(entity);
            _isCachedEntitiesDirty = true;
            _entityCount--;
        }

        private void DestroyEntityNoCheck(Entity entity)
        {
            var archeTypeData = _entityDatas[entity.Id].ArcheTypeData;
            var archeType = archeTypeData->ArcheType;
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                if (config.IsUnique)
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
            }
            archeTypeData->RemoveEntity(entity, ref _entityDatas);
            DeallocateEntity(entity);
        }

        private void CheckAndSetUniqueComponents(Entity entity, ArcheType archeType)
        {
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
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
            }
        }

        private void CheckUniqueComponents(ArcheType archeType)
        {
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                if (config.IsUnique)
                {
                    var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                    if (uniqueEntity == Entity.Null)
                        uniqueEntity = AllocateEntity();

                    throw new EntityAlreadyHasComponentException(
                        uniqueEntity,
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }
            }
        }
    }
}
