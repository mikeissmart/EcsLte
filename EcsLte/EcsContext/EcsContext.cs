using EcsLte.Data;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public unsafe class EcsContext
    {
        private List<IndexDictionary<EntityQueryData>> _queryDataIndexes;
        private List<List<EntityQueryData>> _queryDatas;
        private Entity* _entities;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData* _entityDatas;
        private int _nextId;
        private int _entityCount;
        private int _entityLength;
        private Stack<Entity> _reusableEntities;
        private Entity* _uniqueComponentEntities;
        private readonly EntityCommandManager _commands;
        private readonly SystemManager _systems;

        public string Name { get; }
        public bool IsDestroyed { get; private set; }
        public ICommands Commands => _commands;
        public ISystems Systems => _systems;
        internal EntityData* EntityDatas => _entityDatas;
        internal SharedComponentIndexDictionaries SharedIndexDics { get; private set; }
        internal ManagedComponentPools ManagePools { get; private set; }
        internal ArcheTypeDataManager ArcheTypeManager { get; private set; }

        internal object LockObj;

        internal EcsContext(string name)
        {
            _queryDataIndexes = new List<IndexDictionary<EntityQueryData>>();
            _queryDatas = new List<List<EntityQueryData>>();
            _entities = MemoryHelper.Alloc<Entity>(1);
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _entityDatas = MemoryHelper.Alloc<EntityData>(1);
            _nextId = 1;
            _entityCount = 0;
            _entityLength = 0;
            _reusableEntities = new Stack<Entity>();
            if (ComponentConfigs.Instance.AllUniqueCount > 0)
                _uniqueComponentEntities = MemoryHelper.Alloc<Entity>(ComponentConfigs.Instance.AllUniqueCount);
            _commands = new EntityCommandManager(this);
            _systems = new SystemManager(this);

            Name = name;
            SharedIndexDics = new SharedComponentIndexDictionaries();
            ManagePools = new ManagedComponentPools();
            ArcheTypeManager = new ArcheTypeDataManager(this);
            LockObj = new object();
        }

        public int EntityCount()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _entityCount;
        }

        public int EntityCount(EntityArcheType entityArcheType)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            lock (LockObj)
            {
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                return archeTypeData->EntityCount;
            }
        }

        public int EntityCount(EntityQuery entityQuery)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));

            UpdateEntityQuery(entityQuery, false);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
            var entityCount = 0;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                entityCount += ((ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr)->EntityCount;

            return entityCount;
        }

        public bool HasEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return entity.Id > 0 &&
                entity.Id < _entityLength &&
                _entities[entity.Id] == entity;
        }

        public bool HasEntity(Entity entity, EntityArcheType entityArcheType)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            if (!HasEntity(entity))
                return false;

            lock (LockObj)
            {
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                return _entityDatas[entity.Id].ArcheTypeData == archeTypeData;
            }
        }

        public bool HasEntity(Entity entity, EntityQuery entityQuery)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));

            if (!HasEntity(entity))
                return false;

            UpdateEntityQuery(entityQuery, false);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
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
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

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
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            Entity[] entities;
            lock (LockObj)
            {
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                entities = new Entity[archeTypeData->EntityCount];
                archeTypeData->CopyEntities(ref entities, 0);
            }

            return entities;
        }

        public Entity[] GetEntities(EntityQuery entityQuery)
        {
            var entityCount = EntityCount(entityQuery);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
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
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllComponentDatas.Length == 0)
                throw new EntityBlueprintNoComponentsException();

            lock (LockObj)
            {
                CheckCapacity(1);

                var archeTypeData = ArcheTypeManager.GetArcheTypeData(blueprint.GetEntityArcheType());
                var entity = AllocateEntity();
                var entityData = archeTypeData->AddEntity(entity);
                _entityDatas[entity.Id] = entityData;
                CheckAndSetUniqueComponents(entity, archeTypeData);

                var tempComponentBuffer = stackalloc byte[archeTypeData->ComponentsSizeInBytes];
                archeTypeData->CopyBlittableComponentDatasToBuffer(blueprint.BlittableComponentDatas, tempComponentBuffer);

                if (blueprint.BlittableComponentDatas.Length > 0)
                    archeTypeData->SetComponentsBuffer(entityData, tempComponentBuffer);
                if (blueprint.ManagedComponentDatas.Length > 0)
                {
                    for (var j = 0; j < blueprint.ManagedComponentDatas.Length; j++)
                    {
                        var componentData = blueprint.ManagedComponentDatas[j];
                        var managedPool = ManagePools.GetPool(componentData.Config);

                        blueprint.ManagedComponentDatas[j].CopyManagedComponentData(
                            archeTypeData,
                            entityData,
                            managedPool.AllocateComponent(),
                            managedPool);
                    }
                }

                return entity;
            }
        }

        public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint.AllComponentDatas.Length == 0)
                throw new EntityBlueprintNoComponentsException();
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0)
                return new Entity[count];
            if (count == 1)
                return new Entity[] { CreateEntity(blueprint) };

            lock (LockObj)
            {
                CheckCapacity(count);

                var archeTypeData = ArcheTypeManager.GetArcheTypeData(blueprint.GetEntityArcheType());
                archeTypeData->PreCheckEntityAllocation(count);
                CheckUniqueComponents(archeTypeData);

                var tempComponentBuffer = stackalloc byte[archeTypeData->ComponentsSizeInBytes];
                archeTypeData->CopyBlittableComponentDatasToBuffer(blueprint.BlittableComponentDatas, tempComponentBuffer);

                var componentIndexes = new int[blueprint.ManagedComponentDatas.Length][];
                for (var i = 0; i < blueprint.ManagedComponentDatas.Length; i++)
                {
                    componentIndexes[i] = ManagePools.GetPool(blueprint.ManagedComponentDatas[i].Config)
                        .AllocateComponents(count);
                }

                var entities = new Entity[count];
                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity();
                    var entityData = archeTypeData->AddEntity(entity);
                    entities[i] = entity;
                    _entityDatas[entity.Id] = entityData;

                    if (blueprint.BlittableComponentDatas.Length > 0)
                        archeTypeData->SetComponentsBuffer(entityData, tempComponentBuffer);
                    if (blueprint.ManagedComponentDatas.Length > 0)
                    {
                        for (var j = 0; j < blueprint.ManagedComponentDatas.Length; j++)
                        {
                            var componentData = blueprint.ManagedComponentDatas[j];

                            blueprint.ManagedComponentDatas[j].CopyManagedComponentData(
                                archeTypeData,
                                entityData,
                                componentIndexes[j][i],
                                ManagePools.GetPool(componentData.Config));
                        }
                    }
                }

                return entities;
            }
        }

        public void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            lock (LockObj)
            {
                DestroyEntityNoCheck(entity);
            }
        }

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (LockObj)
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
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (LockObj)
            {
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                ClearUniqueComponents(archeTypeData);
                for (var i = 0; i < archeTypeData->EntityCount; i++)
                    DeallocateEntity(archeTypeData->GetEntity(i));
                archeTypeData->ClearAllEntities();
            }
        }

        public void DestroyEntities(EntityQuery entityQuery)
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (LockObj)
            {
                UpdateEntityQuery(entityQuery, true);
                var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
                for (var entityQueryIndex = 0; entityQueryIndex < contextQueryData.ArcheTypeDatas.Length; entityQueryIndex++)
                {
                    var archeTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[entityQueryIndex].Ptr;
                    ClearUniqueComponents(archeTypeData);
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
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (this == sourceContext)
                throw new EntityTransferSameEcsContextException(this);
            if (!sourceContext.HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            Entity nextEntity;
            lock (LockObj)
            {
                CheckCapacity(1);

                var prevEntityData = sourceContext._entityDatas[entity.Id];
                var prevArcheTypeData = prevEntityData.ArcheTypeData;
                ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);

                var nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();
                nextEntity = AllocateEntity();
                var nextEntityData = nextArcheTypeData->AddEntity(entity);
                _entityDatas[nextEntity.Id] = nextEntityData;
                CheckAndSetUniqueComponents(nextEntity, nextArcheTypeData);

                nextArcheTypeData->SetComponentsBuffer(nextEntityData, prevArcheTypeData->GetComponentsPtr(prevEntityData));
                for (var j = 0; j < nextArcheTypeData->ManagedConfigsLength; j++)
                {
                    var config = nextArcheTypeData->ManagedConfigs[j];
                    var managedPool = ManagePools.GetPool(config);
                    nextArcheTypeData->SetComponentAndIndex(
                        nextEntityData,
                        prevArcheTypeData->GetComponent(
                            prevEntityData,
                            config,
                            sourceContext.ManagePools.GetPool(config)),
                        config,
                        managedPool.AllocateComponent(),
                        managedPool);
                }
                if (destroyEntity)
                    sourceContext.DestroyEntityNoCheck(entity);
            }

            return nextEntity;
        }

        public Entity[] TransferEntities(EcsContext sourceContext, IEnumerable<Entity> entities, bool destroyEntities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (entities.Count() == 0)
                return new Entity[0];
            if (entities.Count() == 1)
                return new Entity[] { TransferEntity(sourceContext, entities.First(), destroyEntities) };
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (this == sourceContext)
                throw new EntityTransferSameEcsContextException(this);

            var transferedEntities = new HashSet<Entity>();
            var cachedArcheTypeDatas = new Dictionary<PtrWrapper, PtrWrapper>();
            var prevArcheTypeDataPtr = new PtrWrapper();
            var nextEntities = new Entity[entities.Count()];
            lock (LockObj)
            {
                CheckCapacity(entities.Count());

                var nextEntityIndex = 0;
                foreach (var prevEntity in entities)
                {
                    if (!sourceContext.HasEntity(prevEntity))
                        throw new EntityDoesNotExistException(prevEntity);
                    if (!transferedEntities.Add(prevEntity))
                        throw new EntityTransferAlreadyException(this, prevEntity);

                    var prevEntityData = sourceContext._entityDatas[prevEntity.Id];
                    var prevArcheTypeData = prevEntityData.ArcheTypeData;
                    prevArcheTypeDataPtr.Ptr = prevArcheTypeData;

                    ArcheTypeData* nextArcheTypeData;
                    if (!cachedArcheTypeDatas.TryGetValue(prevArcheTypeDataPtr, out var nextArcheTypeDataPtr))
                    {
                        ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                        nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();
                        nextArcheTypeDataPtr.Ptr = nextArcheTypeData;
                        cachedArcheTypeDatas.Add(prevArcheTypeDataPtr, nextArcheTypeDataPtr);
                    }
                    else
                    {
                        nextArcheTypeData = (ArcheTypeData*)nextArcheTypeDataPtr.Ptr;
                    }

                    var nextEntity = AllocateEntity();
                    var nextEntityData = nextArcheTypeData->AddEntity(nextEntity);
                    _entityDatas[nextEntity.Id] = nextEntityData;
                    nextEntities[nextEntityIndex++] = nextEntity;
                    CheckAndSetUniqueComponents(nextEntity, nextArcheTypeData);

                    nextArcheTypeData->SetComponentsBuffer(nextEntityData, prevArcheTypeData->GetComponentsPtr(prevEntityData));
                    for (var j = 0; j < nextArcheTypeData->ManagedConfigsLength; j++)
                    {
                        var config = nextArcheTypeData->ManagedConfigs[j];
                        var managedPool = ManagePools.GetPool(config);
                        nextArcheTypeData->SetComponentAndIndex(
                            nextEntityData,
                            prevArcheTypeData->GetComponent(
                                prevEntityData,
                                config,
                                sourceContext.ManagePools.GetPool(config)),
                            config,
                            managedPool.AllocateComponent(),
                            managedPool);
                    }
                }

                if (destroyEntities)
                {
                    foreach (var entity in entities)
                        sourceContext.DestroyEntityNoCheck(entity);
                }
            }

            return nextEntities;
        }

        public Entity[] TransferEntities(EcsContext sourceContext, EntityArcheType entityArcheType, bool destroyEntities)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (this == sourceContext)
                throw new EntityTransferSameEcsContextException(this);

            ArcheTypeData* prevArcheTypeData = null;
            lock (LockObj)
            {
                prevArcheTypeData = sourceContext.ArcheTypeManager.GetArcheTypeData(entityArcheType);
                var entitiesLength = prevArcheTypeData->EntityCount;
                if (entitiesLength == 0)
                    return new Entity[0];
                if (entitiesLength > 1)
                {
                    CheckUniqueComponents(prevArcheTypeData);
                    var nextEntities = new Entity[entitiesLength];

                    CheckCapacity(entitiesLength);
                    TransferArcheType(sourceContext, prevArcheTypeData, ref nextEntities, 0);
                    if (destroyEntities)
                        sourceContext.DestroyEntities(entityArcheType);

                    return nextEntities;
                }
            }

            return new Entity[] { TransferEntity(sourceContext, prevArcheTypeData->GetEntity(0), destroyEntities) };
        }

        public Entity[] TransferEntities(EcsContext sourceContext, EntityQuery entityQuery, bool destroyEntities)
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (sourceContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(sourceContext);
            if (this == sourceContext)
                throw new EntityTransferSameEcsContextException(this);

            var entitiesLength = sourceContext.EntityCount(entityQuery);
            if (entitiesLength == 0)
                return new Entity[0];
            if (entitiesLength == 1)
                return new Entity[] { TransferEntity(sourceContext, sourceContext.GetEntities(entityQuery)[0], destroyEntities) };

            var sourceContextQueryData = entityQuery.QueryData.ContextQueryData[sourceContext];
            var nextEntities = new Entity[entitiesLength];
            var entityIndex = 0;
            lock (LockObj)
            {
                CheckCapacity(entitiesLength);

                for (var queryIndex = 0; queryIndex < sourceContextQueryData.ArcheTypeDatas.Length; queryIndex++)
                {
                    var prevArcheTypeData = (ArcheTypeData*)sourceContextQueryData.ArcheTypeDatas[queryIndex].Ptr;
                    if (prevArcheTypeData->EntityCount == 1)
                    {
                        nextEntities[entityIndex++] = TransferEntity(sourceContext, prevArcheTypeData->GetEntity(0), false);
                    }
                    else
                    {
                        CheckUniqueComponents(prevArcheTypeData);
                        TransferArcheType(sourceContext, prevArcheTypeData, ref nextEntities, entityIndex);
                        entityIndex += prevArcheTypeData->EntityCount;
                    }
                }
                if (destroyEntities)
                    sourceContext.DestroyEntities(entityQuery);
            }

            return nextEntities;
        }

        public EntityArcheType GetEntityArcheType(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var archeTypeData = _entityDatas[entity.Id].ArcheTypeData;
            return new EntityArcheType(this, archeTypeData);
        }

        public EntityArcheType[] GetEntityArcheTypes(EntityQuery entityQuery)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));

            UpdateEntityQuery(entityQuery, false);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
            var entityArcheTypes = new EntityArcheType[contextQueryData.ArcheTypeDatas.Length];
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                entityArcheTypes[i] = new EntityArcheType(this, (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr);

            return entityArcheTypes;
        }

        public bool HasComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return entityData.ArcheTypeData->ArcheType.HasComponentConfig(ComponentConfig<TComponent>.Config);
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            var entityData = _entityDatas[entity.Id];
            if (config.IsBlittable)
                return entityData.ArcheTypeData->GetComponent<TComponent>(entityData, config);
            return entityData.ArcheTypeData->GetComponent(entityData, config, ManagePools.GetPool<TComponent>());
        }

        public TComponent[] GetComponents<TComponent>(IEnumerable<Entity> entities) where TComponent : IComponent
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (entities.Count() == 0)
                return new TComponent[0];
            if (entities.Count() == 1)
                return new[] { GetComponent<TComponent>(entities.First()) };
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var components = new TComponent[entities.Count()];
            var componentIndex = 0;
            var cachedArcheTypeDatas = new Dictionary<PtrWrapper, ComponentConfigOffset>();
            var config = ComponentConfig<TComponent>.Config;
            var archeTypeDataPtr = new PtrWrapper();
            if (config.IsBlittable)
            {
                foreach (var entity in entities)
                {
                    if (!HasComponent<TComponent>(entity))
                        throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                    var entityData = _entityDatas[entity.Id];
                    var archeTypeData = entityData.ArcheTypeData;
                    archeTypeDataPtr.Ptr = archeTypeData;
                    if (!cachedArcheTypeDatas.TryGetValue(archeTypeDataPtr, out var configOffset))
                    {
                        archeTypeData->GetComponentConfigOffset(config, out configOffset);
                        cachedArcheTypeDatas.Add(archeTypeDataPtr, configOffset);
                    }

                    components[componentIndex++] = archeTypeData->GetComponentOffset<TComponent>(entityData, configOffset);
                }
            }
            else
            {
                var managePool = ManagePools.GetPool<TComponent>();
                foreach (var entity in entities)
                {
                    if (!HasComponent<TComponent>(entity))
                        throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                    var entityData = _entityDatas[entity.Id];
                    var archeTypeData = entityData.ArcheTypeData;
                    archeTypeDataPtr.Ptr = archeTypeData;
                    if (!cachedArcheTypeDatas.TryGetValue(archeTypeDataPtr, out var configOffset))
                    {
                        archeTypeData->GetComponentConfigOffset(config, out configOffset);
                        cachedArcheTypeDatas.Add(archeTypeDataPtr, configOffset);
                    }

                    components[componentIndex++] = archeTypeData->GetComponentOffset(entityData, configOffset, managePool);
                }
            }

            return components;
        }

        public TComponent[] GetComponents<TComponent>(EntityArcheType entityArcheType) where TComponent : IComponent
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (!entityArcheType.HasComponentType<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            lock (LockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                var components = new TComponent[archeTypeData->EntityCount];
                if (config.IsBlittable)
                    archeTypeData->GetComponents(ref components, 0, config);
                else
                    archeTypeData->GetComponents(ref components, 0, config, ManagePools.GetPool<TComponent>());

                return components;
            }
        }

        public TComponent[] GetComponents<TComponent>(EntityQuery entityQuery) where TComponent : IComponent
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            UpdateEntityQuery(entityQuery, false);
            var contextQueryData = entityQuery.QueryData.ContextQueryData[this];

            var archeTypeDatas = new List<PtrWrapper>();
            var config = ComponentConfig<TComponent>.Config;
            var entitiesLength = 0;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
            {
                var archeTypeDataPtr = contextQueryData.ArcheTypeDatas[i];
                var archeTypeData = (ArcheTypeData*)archeTypeDataPtr.Ptr;
                if (archeTypeData->ArcheType.HasComponentConfig(config))
                {
                    archeTypeDatas.Add(archeTypeDataPtr);
                    entitiesLength += archeTypeData->EntityCount;
                }
            }

            var components = new TComponent[entitiesLength];
            var componentIndex = 0;
            if (config.IsBlittable)
            {
                for (var i = 0; i < archeTypeDatas.Count; i++)
                {
                    var archeTypeData = (ArcheTypeData*)archeTypeDatas[i].Ptr;
                    archeTypeData->GetComponents(ref components, componentIndex, config);
                    componentIndex += archeTypeData->EntityCount;
                }
            }
            else
            {
                var managePool = ManagePools.GetPool<TComponent>();
                for (var i = 0; i < archeTypeDatas.Count; i++)
                {
                    var archeTypeData = (ArcheTypeData*)archeTypeDatas[i].Ptr;
                    archeTypeData->GetComponents(ref components, componentIndex, config, managePool);
                    componentIndex += archeTypeData->EntityCount;
                }
            }

            return components;
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return entityData.ArcheTypeData->GetAllComponents(entityData, ManagePools);
        }

        public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex] != Entity.Null;
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return GetComponent<TComponentUnique>(
                _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex]);
        }

        public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        public void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            lock (LockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                var entityData = _entityDatas[entity.Id];
                if (config.IsShared)
                {
                    ArcheTypeManager.CachedArcheTypeCopyTo(entityData.ArcheTypeData->ArcheType);

                    var nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(entityData.ArcheTypeData,
                        SharedIndexDics.GetDataIndex(component));
                    if (entityData.ArcheTypeData != nextArcheTypeData)
                        entityData = ArcheTypeData.TransferEntity(entity, nextArcheTypeData, _entityDatas);
                }

                if (config.IsBlittable)
                    entityData.ArcheTypeData->SetComponent(entityData, component, config);
                else
                    entityData.ArcheTypeData->SetComponent(entityData, component, config, ManagePools.GetPool<TComponent>());
            }
        }

        public void UpdateComponents<TComponent>(IEnumerable<Entity> entities, TComponent component) where TComponent : IComponent
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

            lock (LockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                var cachedConfigOffsets = new Dictionary<PtrWrapper, ComponentConfigOffset>();
                if (config.IsShared)
                {
                    var cachedArcheTypeDatas = new Dictionary<PtrWrapper, PtrWrapper>();
                    var prevArcheTypePtr = new PtrWrapper();
                    var sharedIndexDic = SharedIndexDics.GetSharedIndexDic<TComponent>();
                    foreach (var entity in entities)
                    {
                        if (!HasComponent<TComponent>(entity))
                            throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                        var entityData = _entityDatas[entity.Id];
                        var prevArcheTypeData = entityData.ArcheTypeData;
                        ArcheTypeData* nextArcheTypeData;
                        prevArcheTypePtr.Ptr = entityData.ArcheTypeData;
                        if (!cachedArcheTypeDatas.TryGetValue(prevArcheTypePtr, out var nextArcheTypeDataPtr))
                        {
                            ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData, nextSharedDataIndex);

                            nextArcheTypeDataPtr.Ptr = nextArcheTypeData;
                            cachedArcheTypeDatas.Add(prevArcheTypePtr, nextArcheTypeDataPtr);
                        }
                        else
                        {
                            nextArcheTypeData = (ArcheTypeData*)nextArcheTypeDataPtr.Ptr;
                        }

                        if (prevArcheTypeData != nextArcheTypeData)
                            entityData = ArcheTypeData.TransferEntity(entity, nextArcheTypeData, _entityDatas);

                        if (!cachedConfigOffsets.TryGetValue(nextArcheTypeDataPtr, out var configOffset))
                        {
                            nextArcheTypeData->GetComponentConfigOffset(config, out configOffset);
                            cachedConfigOffsets.Add(nextArcheTypeDataPtr, configOffset);
                        }

                        if (config.IsBlittable)
                            nextArcheTypeData->SetComponent(entityData, component, config);
                        else
                            nextArcheTypeData->SetComponent(entityData, component, config, ManagePools.GetPool<TComponent>());
                    }
                }
                else
                {
                    var archeTypePtr = new PtrWrapper();
                    foreach (var entity in entities)
                    {
                        if (!HasComponent<TComponent>(entity))
                            throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                        var entityData = _entityDatas[entity.Id];
                        archeTypePtr.Ptr = entityData.ArcheTypeData;
                        if (!cachedConfigOffsets.TryGetValue(archeTypePtr, out var configOffset))
                        {
                            entityData.ArcheTypeData->GetComponentConfigOffset(config, out configOffset);
                            cachedConfigOffsets.Add(archeTypePtr, configOffset);
                        }

                        if (config.IsBlittable)
                            entityData.ArcheTypeData->SetComponent(entityData, component, config);
                        else
                            entityData.ArcheTypeData->SetComponent(entityData, component, config, ManagePools.GetPool<TComponent>());
                    }
                }
            }
        }

        public void UpdateComponents<TComponent>(EntityArcheType entityArcheType, TComponent component) where TComponent : IComponent
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (!entityArcheType.HasComponentType<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            lock (LockObj)
            {
                var config = ComponentConfig<TComponent>.Config;
                if (config.IsShared)
                {
                    var prevArcheTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                    ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                    var nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData,
                        SharedIndexDics.GetDataIndex(component));

                    if (nextArcheTypeData != prevArcheTypeData)
                        ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, _entityDatas);

                    if (config.IsBlittable)
                    {
                        nextArcheTypeData->SetAllComponents(component, config);
                    }
                    else
                    {
                        nextArcheTypeData->SetAllComponents(component, config, ManagePools.GetPool<TComponent>());
                    }
                }
                else
                {
                    var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityArcheType);
                    if (config.IsBlittable)
                        archeTypeData->SetAllComponents(component, config);
                    else
                        archeTypeData->SetAllComponents(component, config, ManagePools.GetPool<TComponent>());
                }
            }
        }

        public void UpdateComponents<TComponent>(EntityQuery entityQuery, TComponent component) where TComponent : IComponent
        {
            if (entityQuery == null)
                throw new ArgumentNullException(nameof(entityQuery));
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (!entityQuery.HasWhereAllOf<TComponent>())
                throw new EntityQueryNotHaveWhereOfAllException(typeof(TComponent));

            lock (LockObj)
            {
                UpdateEntityQuery(entityQuery, true);
                var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
                var config = ComponentConfig<TComponent>.Config;
                if (config.IsShared)
                {
                    var sharedIndexDic = SharedIndexDics.GetSharedIndexDic<TComponent>();
                    if (config.IsBlittable)
                    {
                        for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                        {
                            var prevArcheTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr;
                            ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            var nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData,
                                nextSharedDataIndex);
                            if (nextArcheTypeData != prevArcheTypeData)
                                ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, _entityDatas);

                            nextArcheTypeData->SetAllComponents(component, config);
                        }
                    }
                    else
                    {
                        var managePool = ManagePools.GetPool<TComponent>();
                        for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                        {
                            var prevArcheTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr;
                            ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            var nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData,
                                nextSharedDataIndex);
                            if (nextArcheTypeData != prevArcheTypeData)
                                ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, _entityDatas);

                            nextArcheTypeData->SetAllComponents(component, config, managePool);
                        }
                    }
                }
                else
                {
                    if (config.IsBlittable)
                    {
                        for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                        {
                            ((ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr)
                                ->SetAllComponents(component, config);
                        }
                    }
                    else
                    {
                        var managePool = ManagePools.GetPool<TComponent>();
                        for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                        {
                            ((ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr)
                                ->SetAllComponents(component, config, managePool);
                        }
                    }
                }
            }
        }

        internal void UpdateForEachArcheType(Entity entity, IComponentAdapter[] writeAdapters)
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var anyChangeArcheType = false;
                ArcheTypeManager.CachedArcheTypeCopyTo(entityData.ArcheTypeData->ArcheType);
                for (var i = 0; i < writeAdapters.Length; i++)
                {
                    var adapter = writeAdapters[i];
                    adapter.UpdateComponent(entityData, entityData.ArcheTypeData);
                    if (adapter.Config.IsShared && adapter.IsUpdated)
                    {
                        anyChangeArcheType |= ArcheTypeManager.CachedArcheTypeReplaceSharedDataIndex(
                            adapter.GetSharedDataIndex());
                    }
                }

                if (anyChangeArcheType)
                {
                    entityData = ArcheTypeData.TransferEntity(
                        entity,
                        ArcheTypeManager.GetCachedArcheTypeData(),
                        _entityDatas);
                }

            }
        }

        internal void InternalDestroy()
        {
            lock (LockObj)
            {
                _queryDataIndexes = null;
                _queryDatas = null;
                MemoryHelper.Free(_entities);
                _entities = null;
                _cachedEntities = null;
                _isCachedEntitiesDirty = true;
                MemoryHelper.Free(_entityDatas);
                _entityDatas = null;
                _nextId = 0;
                _entityCount = 0;
                _entityLength = 0;
                _reusableEntities = null;
                if (_uniqueComponentEntities != null)
                    MemoryHelper.Free(_uniqueComponentEntities);
                _uniqueComponentEntities = null;
                _commands.InternalDestroy();
                _systems.InternalDestroy();

                IsDestroyed = true;
                SharedIndexDics = null;
                ManagePools = null;
                ArcheTypeManager.InternalDestroy();
            }
        }

        private void CheckCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = _entityLength - ((_entityCount + 1) - _reusableEntities.Count);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_entityLength + count, 2) + 1);
                var newEntities = MemoryHelper.Alloc<Entity>(newCapacity);
                MemoryHelper.Copy(
                    _entities,
                    newEntities,
                    _entityLength * TypeCache<Entity>.SizeInBytes);
                MemoryHelper.Free(_entities);
                _entities = newEntities;

                var newEntityDatas = MemoryHelper.Alloc<EntityData>(newCapacity);
                MemoryHelper.Copy(
                    _entityDatas,
                    newEntityDatas,
                    _entityLength * TypeCache<EntityData>.SizeInBytes);
                MemoryHelper.Free(_entityDatas);
                _entityDatas = newEntityDatas;

                _entityLength = newCapacity;
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
            ClearUniqueComponents(archeTypeData);
            archeTypeData->RemoveEntity(entity, _entityDatas, ManagePools);
            DeallocateEntity(entity);
        }

        private void CheckAndSetUniqueComponents(Entity entity, ArcheTypeData* archeTypeData)
        {
            for (var i = 0; i < archeTypeData->UniqueConfigsLength; i++)
            {
                var config = archeTypeData->UniqueConfigs[i];
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }

                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }
        }

        private void CheckUniqueComponents(ArcheTypeData* archeTypeData)
        {
            for (var i = 0; i < archeTypeData->UniqueConfigsLength; i++)
            {
                var config = archeTypeData->UniqueConfigs[i];
                var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                if (uniqueEntity == Entity.Null)
                    uniqueEntity = AllocateEntity();

                throw new EntityAlreadyHasComponentException(
                    uniqueEntity,
                    ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
            }
        }

        private void ClearUniqueComponents(ArcheTypeData* archeTypeData)
        {
            for (var i = 0; i < archeTypeData->UniqueConfigsLength; i++)
                _uniqueComponentEntities[archeTypeData->UniqueConfigs[i].UniqueIndex] = Entity.Null;
        }

        private void TransferArcheType(EcsContext sourceContext, ArcheTypeData* prevArcheTypeData, ref Entity[] nextEntities, int startEntityIndex)
        {
            var entitiesLength = prevArcheTypeData->EntityCount;
            CheckCapacity(entitiesLength);

            ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
            var nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();

            var componentIndexes = new int[nextArcheTypeData->ManagedConfigsLength][];
            for (var i = 0; i < nextArcheTypeData->ManagedConfigsLength; i++)
            {
                componentIndexes[i] = ManagePools.GetPool(nextArcheTypeData->ManagedConfigs[i])
                    .AllocateComponents(entitiesLength);
            }

            nextArcheTypeData->PreCheckEntityAllocation(entitiesLength);
            for (var i = 0; i < entitiesLength; i++)
            {
                var prevEntity = prevArcheTypeData->GetEntity(i);
                var prevEntityData = sourceContext._entityDatas[prevEntity.Id];
                var prevComponentsPtr = prevArcheTypeData->GetComponentsPtr(sourceContext._entityDatas[prevEntity.Id]);

                var nextEntity = AllocateEntity();
                var nextEntityData = nextArcheTypeData->AddEntity(nextEntity);
                _entityDatas[nextEntity.Id] = nextEntityData;
                nextEntities[i + startEntityIndex] = nextEntity;

                nextArcheTypeData->SetComponentsBuffer(nextEntityData, prevArcheTypeData->GetComponentsPtr(prevEntityData));
                for (var j = 0; j < nextArcheTypeData->ManagedConfigsLength; j++)
                {
                    var config = nextArcheTypeData->ManagedConfigs[j];
                    nextArcheTypeData->SetComponentAndIndex(
                        nextEntityData,
                        prevArcheTypeData->GetComponent(
                            prevEntityData,
                            config,
                            sourceContext.ManagePools.GetPool(config)),
                        config,
                        componentIndexes[j][i],
                        ManagePools.GetPool(config));
                }
            }
        }

        #region Query

        internal void UpdateEntityQuery(EntityQuery query, bool behindLock)
        {
            if (!behindLock)
            {
                lock (LockObj)
                {
                    UpdateEntityQuery(query, true);
                }
            }
            else
            {
                if (!query.QueryData.ContextQueryData.ContainsKey(this))
                {
                    GetQueryIndexDic(query.QueryData.ConfigCount, out var indexDic, out var dataList);

                    var wasAdded = false;
                    var index = indexDic.GetOrAdd(query.QueryData,
                        (newIndex) =>
                        {
                            query.QueryData.ContextQueryData.Add(this, new EntityQueryEcsContextData
                            {
                                EntityQueryDataIndex = newIndex
                            });
                            dataList.Add(query.QueryData);
                            wasAdded = true;
                        });
                    if (!wasAdded)
                        query.QueryData = dataList[index];
                }
                ArcheTypeManager.UpdateEntityQuery(query);
            }
        }

        internal void GetQueryIndexDic(int configCount,
            out IndexDictionary<EntityQueryData> indexDic, out List<EntityQueryData> dataList)
        {
            while (_queryDatas.Count <= configCount)
            {
                _queryDataIndexes.Add(new IndexDictionary<EntityQueryData>(EntityQueryDataEqualityComparer.Comparer));
                _queryDatas.Add(new List<EntityQueryData>());
            }

            indexDic = _queryDataIndexes[configCount];
            dataList = _queryDatas[configCount];
        }

        #endregion Query
    }
}