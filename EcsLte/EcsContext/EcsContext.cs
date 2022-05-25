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
        private List<IndexDictionary<ArcheType>> _archeTypeIndexes;

        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        private List<IndexDictionary<EntityQueryData>> _queryDataIndexes;
        private List<List<EntityQueryData>> _queryDatas;
        private int _archeTypeDataVersion;
        private Entity[] _entities;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData[] _entityDatas;
        private int _nextId;
        private int _entityCount;
        private Stack<Entity> _reusableEntities;
        private Entity[] _uniqueComponentEntities;
        private readonly EntityCommandManager _commands;
        private readonly SystemManager _systems;
        private ArcheType _cachedArcheType;

        public string Name { get; }
        public bool IsDestroyed { get; private set; }
        public ICommands Commands => _commands;
        public ISystems Systems => _systems;
        internal EntityData[] EntityDatas => _entityDatas;
        internal SharedComponentIndexDictionaries SharedIndexDics { get; private set; }
        internal ManagedComponentPools ManagePools { get; private set; }

        internal object LockObj;

        internal EcsContext(string name)
        {
            _archeTypeIndexes = new List<IndexDictionary<ArcheType>>();
            _archeTypeDatas = new List<List<PtrWrapper>>();
            _queryDataIndexes = new List<IndexDictionary<EntityQueryData>>();
            _queryDatas = new List<List<EntityQueryData>>();
            _entities = new Entity[1];
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _entityDatas = new EntityData[1];
            _nextId = 1;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.AllUniqueCount];
            _commands = new EntityCommandManager(this);
            _systems = new SystemManager(this);
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);

            Name = name;
            SharedIndexDics = new SharedComponentIndexDictionaries();
            ManagePools = new ManagedComponentPools();
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

            var archeTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, false);
            return archeTypeData->EntityCount;
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
                entity.Id < _entities.Length &&
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

            var archeTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, false);
            return _entityDatas[entity.Id].ArcheTypeData == archeTypeData;
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

            var archeTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, false);
            var entities = new Entity[archeTypeData->EntityCount];
            archeTypeData->CopyEntities(ref entities, 0);

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

                var archeTypeData = GetArcheTypeDataFromEntityArcheType(blueprint.GetEntityArcheType(), true);
                var entity = AllocateEntity();
                var entityData = archeTypeData->AddEntity(entity);
                _entityDatas[entity.Id] = entityData;
                CheckAndSetUniqueComponents(entity, archeTypeData->ArcheType);

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

                var archeTypeData = GetArcheTypeDataFromEntityArcheType(blueprint.GetEntityArcheType(), true);
                archeTypeData->PreCheckEntityAllocation(count);
                CheckUniqueComponents(archeTypeData->ArcheType);

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
                var archeTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, true);
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
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (LockObj)
            {
                UpdateEntityQuery(entityQuery, true);
                var contextQueryData = entityQuery.QueryData.ContextQueryData[this];
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
                CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);

                var nextArcheTypeData = GetArcheTypeDataFromCachedArcheType();
                nextEntity = AllocateEntity();
                var nextEntityData = nextArcheTypeData->AddEntity(entity);
                _entityDatas[nextEntity.Id] = nextEntityData;
                CheckAndSetUniqueComponents(nextEntity, _cachedArcheType);

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
                        CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                        nextArcheTypeData = GetArcheTypeDataFromCachedArcheType();
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
                    CheckAndSetUniqueComponents(nextEntity, nextArcheTypeData->ArcheType);

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

            var prevArcheTypeData = sourceContext.GetArcheTypeDataFromEntityArcheType(entityArcheType, false);
            var entitiesLength = prevArcheTypeData->EntityCount;
            if (entitiesLength == 0)
                return new Entity[0];
            if (entitiesLength == 1)
                return new Entity[] { TransferEntity(sourceContext, prevArcheTypeData->GetEntity(0), destroyEntities) };

            CheckUniqueComponents(prevArcheTypeData->ArcheType);
            var nextEntities = new Entity[entitiesLength];
            lock (LockObj)
            {
                CheckCapacity(entitiesLength);
                TransferArcheType(sourceContext, prevArcheTypeData, ref nextEntities, 0);
                if (destroyEntities)
                    sourceContext.DestroyEntities(entityArcheType);
            }

            return nextEntities;
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
                        CheckUniqueComponents(prevArcheTypeData->ArcheType);
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

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, false);
            var components = new TComponent[archeTypeData->EntityCount];
            if (config.IsBlittable)
                archeTypeData->GetComponents(ref components, 0, config);
            else
                archeTypeData->GetComponents(ref components, 0, config, ManagePools.GetPool<TComponent>());

            return components;
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
                    CachedArcheTypeCopyTo(entityData.ArcheTypeData->ArcheType);
                    var nextArcheTypeData = CachedArcheTypeGetNextArcheTypeData(entityData.ArcheTypeData,
                        SharedIndexDics.GetDataIndex(component));
                    if (entityData.ArcheTypeData != nextArcheTypeData)
                        entityData = ArcheTypeData.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);
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
                            CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            nextArcheTypeData = CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData, nextSharedDataIndex);

                            nextArcheTypeDataPtr.Ptr = nextArcheTypeData;
                            cachedArcheTypeDatas.Add(prevArcheTypePtr, nextArcheTypeDataPtr);
                        }
                        else
                        {
                            nextArcheTypeData = (ArcheTypeData*)nextArcheTypeDataPtr.Ptr;
                        }

                        if (prevArcheTypeData != nextArcheTypeData)
                            entityData = ArcheTypeData.TransferEntity(entity, nextArcheTypeData, ref _entityDatas);

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
                    var prevArcheTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, true);
                    CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                    var nextArcheTypeData = CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData,
                        SharedIndexDics.GetDataIndex(component));

                    if (nextArcheTypeData != prevArcheTypeData)
                        ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, ref _entityDatas);

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
                    var archeTypeData = GetArcheTypeDataFromEntityArcheType(entityArcheType, true);
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
                            CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            var nextArcheTypeData = CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData, nextSharedDataIndex);
                            if (nextArcheTypeData != prevArcheTypeData)
                                ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, ref _entityDatas);

                            nextArcheTypeData->SetAllComponents(component, config);
                        }
                    }
                    else
                    {
                        var managePool = ManagePools.GetPool<TComponent>();
                        for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                        {
                            var prevArcheTypeData = (ArcheTypeData*)contextQueryData.ArcheTypeDatas[i].Ptr;
                            CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            var nextArcheTypeData = CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData, nextSharedDataIndex);
                            if (nextArcheTypeData != prevArcheTypeData)
                                ArcheTypeData.TransferAllEntities(prevArcheTypeData, nextArcheTypeData, ref _entityDatas);

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
                CachedArcheTypeCopyTo(entityData.ArcheTypeData->ArcheType);
                for (var i = 0; i < writeAdapters.Length; i++)
                {
                    var adapter = writeAdapters[i];
                    adapter.UpdateComponent(entityData, entityData.ArcheTypeData);
                    if (adapter.Config.IsShared && adapter.IsUpdated)
                        anyChangeArcheType |= _cachedArcheType.ReplaceSharedComponentDataIndex(adapter.GetSharedDataIndex());
                }

                if (anyChangeArcheType)
                    entityData = ArcheTypeData.TransferEntity(entity, GetArcheTypeDataFromCachedArcheType(), ref _entityDatas);

            }
        }

        internal void InternalDestroy()
        {
            lock (LockObj)
            {
                foreach (var indexDic in _archeTypeIndexes)
                {
                    while (indexDic.PopKey(out var archeType))
                        archeType.Dispose();
                }
                _archeTypeIndexes = null;
                foreach (var dataList in _archeTypeDatas)
                {
                    foreach (var archeTypeData in dataList)
                    {
                        ((ArcheTypeData*)archeTypeData.Ptr)->Dispose();
                        MemoryHelper.Free(archeTypeData.Ptr);
                    }
                }
                _archeTypeDatas = null;
                _queryDataIndexes = null;
                _queryDatas = null;
                _entities = null;
                _cachedEntities = null;
                _isCachedEntitiesDirty = true;
                _entityDatas = null;
                _nextId = 0;
                _entityCount = 0;
                _reusableEntities = null;
                _uniqueComponentEntities = null;
                _commands.InternalDestroy();
                _systems.InternalDestroy();
                _cachedArcheType.Dispose();

                IsDestroyed = true;
                SharedIndexDics = null;
                ManagePools = null;
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
            archeTypeData->RemoveEntity(entity, ref _entityDatas, ManagePools);
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

        private void TransferArcheType(EcsContext sourceContext, ArcheTypeData* prevArcheTypeData, ref Entity[] nextEntities, int startEntityIndex)
        {
            var entitiesLength = prevArcheTypeData->EntityCount;
            CheckCapacity(entitiesLength);

            CachedArcheTypeCopyTo(prevArcheTypeData->ArcheType);
            var nextArcheTypeData = GetArcheTypeDataFromCachedArcheType();

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

        #region ArcheType

        private void CachedArcheTypeCopyTo(ArcheType source)
        {
            _cachedArcheType.ComponentConfigLength = source.ComponentConfigLength;
            _cachedArcheType.SharedComponentDataLength = source.SharedComponentDataLength;

            MemoryHelper.Copy(
                source.ComponentConfigs,
                _cachedArcheType.ComponentConfigs,
                source.ComponentConfigLength * TypeCache<ComponentConfig>.SizeInBytes);
            if (source.SharedComponentDataLength > 0)
            {
                MemoryHelper.Copy(
                    source.SharedComponentDataIndexes,
                    _cachedArcheType.SharedComponentDataIndexes,
                    source.SharedComponentDataLength * TypeCache<SharedComponentDataIndex>.SizeInBytes);
            }
        }

        private ArcheTypeData* CachedArcheTypeGetNextArcheTypeData(ArcheTypeData* prevArcheTypeData, SharedComponentDataIndex replaceSharedDataIndex) =>
            _cachedArcheType.ReplaceSharedComponentDataIndex(replaceSharedDataIndex)
                ? GetArcheTypeDataFromCachedArcheType()
                : prevArcheTypeData;

        private ArcheTypeData* GetArcheTypeDataFromEntityArcheType(EntityArcheType entityArcheType, bool behindLock)
        {
            if (entityArcheType.ArcheTypeData.TryGetArcheTypeIndex(this, out var archeTypeIndex))
                return GetArcheTypeDataFromIndex(archeTypeIndex);
            if (!behindLock)
            {
                lock (LockObj)
                {
                    return GetArcheTypeDataFromEntityArcheType(entityArcheType, true);
                }
            }
            else
            {
                var entityArcheTypeData = entityArcheType.ArcheTypeData;
                _cachedArcheType.ComponentConfigLength = entityArcheTypeData.ComponentConfigs.Length;
                _cachedArcheType.SharedComponentDataLength = entityArcheTypeData.SharedComponentDatas.Length;

                for (var i = 0; i < _cachedArcheType.ComponentConfigLength; i++)
                    _cachedArcheType.ComponentConfigs[i] = entityArcheTypeData.ComponentConfigs[i];
                for (var i = 0; i < _cachedArcheType.SharedComponentDataLength; i++)
                {
                    var component = entityArcheTypeData.SharedComponentDatas[i];
                    _cachedArcheType.SharedComponentDataIndexes[i] = component.GetSharedComponentDataIndex(SharedIndexDics);
                }

                GetArcheTypeIndexDic(_cachedArcheType.ComponentConfigLength, out var indexDic, out var dataList);
                ArcheTypeData* archeTypeData = null;

                var wasAdded = false;
                var index = indexDic.GetOrAdd(_cachedArcheType,
                    (newIndex) =>
                    {
                        var archeType = ArcheType.AllocClone(_cachedArcheType);
                        archeTypeData = ArcheTypeData.Alloc(archeType, new ArcheTypeIndex
                        {
                            ComponentConfigLength = _cachedArcheType.ComponentConfigLength,
                            Index = newIndex
                        });
                        dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                        _archeTypeDataVersion++;
                        wasAdded = true;

                        return archeType;
                    });
                if (!wasAdded)
                    archeTypeData = (ArcheTypeData*)dataList[index].Ptr;

                entityArcheType.ArcheTypeData.AddArcheTypeIndex(this, archeTypeData->ArcheTypeIndex);

                return archeTypeData;
            }
        }

        private ArcheTypeData* GetArcheTypeDataFromCachedArcheType()
        {
            GetArcheTypeIndexDic(_cachedArcheType.ComponentConfigLength, out var indexDic, out var dataList);
            ArcheTypeData* archeTypeData = null;

            var wasAdded = false;
            var index = indexDic.GetOrAdd(_cachedArcheType,
                (newIndex) =>
                {
                    var archeType = ArcheType.AllocClone(_cachedArcheType);
                    archeTypeData = ArcheTypeData.Alloc(archeType, new ArcheTypeIndex
                    {
                        ComponentConfigLength = _cachedArcheType.ComponentConfigLength,
                        Index = newIndex
                    });
                    dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                    _archeTypeDataVersion++;
                    wasAdded = true;

                    return archeType;
                });
            if (!wasAdded)
                archeTypeData = (ArcheTypeData*)dataList[index].Ptr;

            return archeTypeData;
        }

        private ArcheTypeData* GetArcheTypeDataFromIndex(ArcheTypeIndex archeTypeIndex) =>
            (ArcheTypeData*)_archeTypeDatas
                [archeTypeIndex.ComponentConfigLength]
                [archeTypeIndex.Index]
                .Ptr;

        private void GetArcheTypeIndexDic(int configCount,
            out IndexDictionary<ArcheType> indexDic, out List<PtrWrapper> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<ArcheType>(ArcheTypeEqualityComparer.Comparer));
                _archeTypeDatas.Add(new List<PtrWrapper>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }

        #endregion ArcheType

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

                var contextQueryData = query.QueryData.ContextQueryData[this];
                if (contextQueryData.ArcheTypeChangeVersion != _archeTypeDataVersion)
                {
                    var archeTypeDatas = new List<PtrWrapper>();
                    for (var i = query.QueryData.ConfigCount; i < _archeTypeDatas.Count; i++)
                    {
                        var dataList = _archeTypeDatas[i];
                        for (var j = 0; j < dataList.Count; j++)
                        {
                            var archeTypePtr = dataList[j];
                            if (query.QueryData.IsFiltered(((ArcheTypeData*)archeTypePtr.Ptr)->ArcheType))
                                archeTypeDatas.Add(archeTypePtr);
                        }
                    }

                    contextQueryData.ArcheTypeDatas = archeTypeDatas.ToArray();
                    contextQueryData.ArcheTypeChangeVersion = _archeTypeDataVersion;
                }
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