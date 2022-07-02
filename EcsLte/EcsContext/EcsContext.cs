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
        private Entity[] _cachedDestroyEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData* _entityDatas;
        private int _nextId;
        private int _entityCount;
        private int _entityLength;
        private Stack<Entity> _reusableEntities;
        private Entity* _uniqueComponentEntities;
        private readonly EntityCommandManager _commands;
        private readonly SystemManager _systems;
        private readonly MemoryBookManager _bookManager;

        public string Name { get; private set; }
        public bool IsDestroyed { get; private set; }
        public ICommands Commands => _commands;
        public ISystems Systems => _systems;
        internal EntityData* EntityDatas => _entityDatas;
        internal SharedComponentIndexDictionaries SharedIndexDics { get; private set; }
        internal ManagedComponentPools ManagePools { get; private set; }
        internal ArcheTypeDataManager ArcheTypeManager { get; private set; }
        internal EntityTrackerManager TrackerManager { get; private set; }

        internal object LockObj;

        internal EcsContext(string name)
        {
            _queryDataIndexes = new List<IndexDictionary<EntityQueryData>>();
            _queryDatas = new List<List<EntityQueryData>>();
            _entities = MemoryHelper.Alloc<Entity>(1);
            _cachedEntities = new Entity[0];
            _cachedDestroyEntities = new Entity[0];
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
            _bookManager = new MemoryBookManager();

            Name = name;
            SharedIndexDics = new SharedComponentIndexDictionaries();
            ManagePools = new ManagedComponentPools();
            ArcheTypeManager = new ArcheTypeDataManager(this);
            TrackerManager = new EntityTrackerManager(this);
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
                return archeTypeData.EntityCount;
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
                entityCount += contextQueryData.ArcheTypeDatas[i].EntityCount;

            return entityCount;
        }

        public int EntityCount(EntityTracker tracker)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));

            return tracker.CahcedEntities.Length;
        }

        public int EntityCapacity()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _entityLength;
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
                return ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex) == archeTypeData;
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
            var archeTypeData = ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex);
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
            {
                if (archeTypeData == contextQueryData.ArcheTypeDatas[i])
                    return true;
            }

            return false;
        }

        public bool HasEntity(Entity entity, EntityTracker tracker)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));

            if (!HasEntity(entity))
                return false;

            return tracker.Entities[entity.Id] == entity;
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
                entities = new Entity[archeTypeData.EntityCount];
                archeTypeData.CopyEntities(ref entities, 0);
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
                var archeTypeData = contextQueryData.ArcheTypeDatas[i];
                archeTypeData.CopyEntities(ref entities, entitiesOffset);
                entitiesOffset += archeTypeData.EntityCount;
            }

            return entities;
        }

        public Entity[] GetEntities(EntityTracker tracker)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));

            return tracker.CahcedEntities;
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
                var entityData = archeTypeData.AddEntity(entity, _bookManager);
                _entityDatas[entity.Id] = entityData;
                CheckAndSetUniqueComponents(entity, archeTypeData);
                for (var i = 0; i < archeTypeData.ArcheType.ComponentConfigLength; i++)
                    TrackerManager.TrackAdd(entity, archeTypeData.ArcheType.ComponentConfigs[i]);

                var tempComponentBuffer = stackalloc byte[archeTypeData.ComponentsSizeInBytes];
                archeTypeData.CopyBlittableComponentDatasToBuffer(blueprint.BlittableComponentDatas,
                    tempComponentBuffer);

                if (blueprint.BlittableComponentDatas.Length > 0)
                {
                    archeTypeData.SetComponentsBuffer(entityData,
                        tempComponentBuffer);
                }
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
                archeTypeData.PrecheckEntityAllocation(count, _bookManager);
                CheckUniqueComponents(archeTypeData);

                var tempComponentBuffer = stackalloc byte[archeTypeData.ComponentsSizeInBytes];
                archeTypeData.CopyBlittableComponentDatasToBuffer(blueprint.BlittableComponentDatas,
                    tempComponentBuffer);

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
                    for (var j = 0; j < archeTypeData.ArcheType.ComponentConfigLength; j++)
                        TrackerManager.TrackAdd(entity, archeTypeData.ArcheType.ComponentConfigs[j]);

                    var entityData = archeTypeData.AddEntity(entity, _bookManager);
                    entities[i] = entity;
                    _entityDatas[entity.Id] = entityData;

                    if (blueprint.BlittableComponentDatas.Length > 0)
                    {
                        archeTypeData.SetComponentsBuffer(entityData,
                            tempComponentBuffer);
                    }
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
                DestroyEntities(ArcheTypeManager.GetArcheTypeData(entityArcheType));
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
                    DestroyEntities(contextQueryData.ArcheTypeDatas[entityQueryIndex]);
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
                var prevArcheTypeData = sourceContext.ArcheTypeManager.GetArcheTypeData(prevEntityData.ArcheTypeIndex);
                ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData.ArcheType);

                var nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();
                nextEntity = AllocateEntity();
                for (var i = 0; i < prevArcheTypeData.ArcheType.ComponentConfigLength; i++)
                    TrackerManager.TrackAdd(nextEntity, prevArcheTypeData.ArcheType.ComponentConfigs[i]);

                var nextEntityData = nextArcheTypeData.AddEntity(nextEntity, _bookManager);
                _entityDatas[nextEntity.Id] = nextEntityData;
                CheckAndSetUniqueComponents(nextEntity, nextArcheTypeData);

                nextArcheTypeData.SetComponentsBuffer(nextEntityData, prevEntityData.Slot.Buffer);
                for (var j = 0; j < nextArcheTypeData.ManagedConfigsLength; j++)
                {
                    var config = nextArcheTypeData.ManagedConfigs[j];
                    var managedPool = ManagePools.GetPool(config);
                    nextArcheTypeData.SetComponentAndIndex(
                        nextEntityData,
                        prevArcheTypeData.GetComponent(
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
            var cachedArcheTypeDatas = new Dictionary<ArcheTypeIndex, ArcheTypeData>();
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
                    var prevArcheTypeData = sourceContext.ArcheTypeManager.GetArcheTypeData(prevEntityData.ArcheTypeIndex);

                    if (!cachedArcheTypeDatas.TryGetValue(prevArcheTypeData.ArcheTypeIndex, out var nextArcheTypeData))
                    {
                        ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData.ArcheType);
                        nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();
                        cachedArcheTypeDatas.Add(prevArcheTypeData.ArcheTypeIndex, nextArcheTypeData);
                    }

                    var nextEntity = AllocateEntity();
                    for (var i = 0; i < prevArcheTypeData.ArcheType.ComponentConfigLength; i++)
                        TrackerManager.TrackAdd(nextEntity, prevArcheTypeData.ArcheType.ComponentConfigs[i]);

                    var nextEntityData = nextArcheTypeData.AddEntity(nextEntity, _bookManager);
                    _entityDatas[nextEntity.Id] = nextEntityData;
                    nextEntities[nextEntityIndex++] = nextEntity;
                    CheckAndSetUniqueComponents(nextEntity, nextArcheTypeData);

                    nextArcheTypeData.SetComponentsBuffer(nextEntityData, prevEntityData.Slot.Buffer);
                    for (var j = 0; j < nextArcheTypeData.ManagedConfigsLength; j++)
                    {
                        var config = nextArcheTypeData.ManagedConfigs[j];
                        var managedPool = ManagePools.GetPool(config);
                        nextArcheTypeData.SetComponentAndIndex(
                            nextEntityData,
                            prevArcheTypeData.GetComponent(
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

            lock (LockObj)
            {
                var prevArcheTypeData = sourceContext.ArcheTypeManager.GetArcheTypeData(entityArcheType);
                var entitiesLength = prevArcheTypeData.EntityCount;
                if (entitiesLength == 0)
                    return new Entity[0];
                else if (entitiesLength == 1)
                {
                    return new Entity[] { TransferEntity(sourceContext, prevArcheTypeData.GetEntity(0), destroyEntities) };
                }
                else
                {
                    CheckUniqueComponents(prevArcheTypeData);
                    var nextEntities = new Entity[entitiesLength];

                    CheckCapacity(entitiesLength);
                    TransferArcheTypeDiffContext(sourceContext, prevArcheTypeData, ref nextEntities, 0);
                    if (destroyEntities)
                        sourceContext.DestroyEntities(entityArcheType);

                    return nextEntities;
                }
            }
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
                    var prevArcheTypeData = sourceContextQueryData.ArcheTypeDatas[queryIndex];
                    if (prevArcheTypeData.EntityCount == 1)
                    {
                        nextEntities[entityIndex++] = TransferEntity(sourceContext, prevArcheTypeData.GetEntity(0), false);
                    }
                    else
                    {
                        CheckUniqueComponents(prevArcheTypeData);
                        TransferArcheTypeDiffContext(sourceContext, prevArcheTypeData, ref nextEntities, entityIndex);
                        entityIndex += prevArcheTypeData.EntityCount;
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

            return new EntityArcheType(this, ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
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
                entityArcheTypes[i] = new EntityArcheType(this, contextQueryData.ArcheTypeDatas[i]);

            return entityArcheTypes;
        }

        public bool HasComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            return ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex)
                .ArcheType.HasComponentConfig(ComponentConfig<TComponent>.Config);
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            var entityData = _entityDatas[entity.Id];
            if (config.IsBlittable)
                return ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex)
                    .GetComponent<TComponent>(entityData, config);
            else
                return ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex)
                    .GetComponent(entityData, config, ManagePools.GetPool<TComponent>());
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
            var cachedArcheTypeDatas = new Dictionary<ArcheTypeIndex, ComponentConfigOffset>();
            var config = ComponentConfig<TComponent>.Config;
            if (config.IsBlittable)
            {
                foreach (var entity in entities)
                {
                    if (!HasComponent<TComponent>(entity))
                        throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                    var entityData = _entityDatas[entity.Id];
                    var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                    if (!cachedArcheTypeDatas.TryGetValue(archeTypeData.ArcheTypeIndex, out var configOffset))
                    {
                        configOffset = archeTypeData.GetComponentConfigOffset(config);
                        cachedArcheTypeDatas.Add(archeTypeData.ArcheTypeIndex, configOffset);
                    }

                    components[componentIndex++] = archeTypeData.GetComponentOffset<TComponent>(entityData, configOffset);
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
                    var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                    if (!cachedArcheTypeDatas.TryGetValue(archeTypeData.ArcheTypeIndex, out var configOffset))
                    {
                        configOffset = archeTypeData.GetComponentConfigOffset(config);
                        cachedArcheTypeDatas.Add(archeTypeData.ArcheTypeIndex, configOffset);
                    }

                    components[componentIndex++] = archeTypeData.GetComponentOffset(entityData, configOffset, managePool);
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
                var components = new TComponent[archeTypeData.EntityCount];
                if (config.IsBlittable)
                    archeTypeData.GetComponents(ref components, 0, config);
                else
                    archeTypeData.GetComponents(ref components, 0, config, ManagePools.GetPool<TComponent>());

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

            var archeTypeDatas = new List<ArcheTypeData>();
            var config = ComponentConfig<TComponent>.Config;
            var entitiesLength = 0;
            for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
            {
                var archeTypeData = contextQueryData.ArcheTypeDatas[i];
                if (archeTypeData.ArcheType.HasComponentConfig(config))
                {
                    archeTypeDatas.Add(archeTypeData);
                    entitiesLength += archeTypeData.EntityCount;
                }
            }

            var components = new TComponent[entitiesLength];
            var componentIndex = 0;
            if (config.IsBlittable)
            {
                for (var i = 0; i < archeTypeDatas.Count; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    archeTypeData.GetComponents(ref components, componentIndex, config);
                    componentIndex += archeTypeData.EntityCount;
                }
            }
            else
            {
                var managePool = ManagePools.GetPool<TComponent>();
                for (var i = 0; i < archeTypeDatas.Count; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    archeTypeData.GetComponents(ref components, componentIndex, config, managePool);
                    componentIndex += archeTypeData.EntityCount;
                }
            }

            return components;
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            return ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex)
                .GetAllComponents(entityData, ManagePools);
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
                var prevArcheTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                if (config.IsShared)
                {
                    ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData.ArcheType);

                    var nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData,
                        SharedIndexDics.GetDataIndex(component));
                    if (prevArcheTypeData != nextArcheTypeData)
                    {
                        entityData = ArcheTypeData.TransferEntity(
                            entity,
                            prevArcheTypeData,
                            nextArcheTypeData,
                            _entityDatas,
                            _bookManager);
                    }
                }

                if (config.IsBlittable)
                    prevArcheTypeData.SetComponent(entityData, component, config);
                else
                    prevArcheTypeData.SetComponent(entityData, component, config, ManagePools.GetPool<TComponent>());
                TrackerManager.TrackUpdate(entity, config);
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
                var cachedConfigOffsets = new Dictionary<ArcheTypeIndex, ComponentConfigOffset>();
                if (config.IsShared)
                {
                    var cachedArcheTypeDatas = new Dictionary<ArcheTypeIndex, ArcheTypeData>();
                    var sharedIndexDic = SharedIndexDics.GetSharedIndexDic<TComponent>();
                    foreach (var entity in entities)
                    {
                        if (!HasComponent<TComponent>(entity))
                            throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                        var entityData = _entityDatas[entity.Id];
                        var prevArcheTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                        if (!cachedArcheTypeDatas.TryGetValue(prevArcheTypeData.ArcheTypeIndex, out var nextArcheTypeData))
                        {
                            ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData.ArcheType);
                            var nextSharedDataIndex = new SharedComponentDataIndex
                            {
                                SharedIndex = config.SharedIndex,
                                SharedDataIndex = sharedIndexDic.GetOrAdd(component),
                            };
                            nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData, nextSharedDataIndex);
                            cachedArcheTypeDatas.Add(prevArcheTypeData.ArcheTypeIndex, nextArcheTypeData);
                        }

                        if (prevArcheTypeData != nextArcheTypeData)
                        {
                            entityData = ArcheTypeData.TransferEntity(
                                entity,
                                prevArcheTypeData,
                                nextArcheTypeData,
                                _entityDatas,
                                _bookManager);
                        }

                        if (!cachedConfigOffsets.TryGetValue(nextArcheTypeData.ArcheTypeIndex, out var configOffset))
                        {
                            cachedConfigOffsets.Add(nextArcheTypeData.ArcheTypeIndex,
                                nextArcheTypeData.GetComponentConfigOffset(config));
                        }

                        if (config.IsBlittable)
                            nextArcheTypeData.SetComponent(entityData, component, config);
                        else
                            nextArcheTypeData.SetComponent(entityData, component, config, ManagePools.GetPool<TComponent>());
                        TrackerManager.TrackUpdate(entity, config);
                    }
                }
                else
                {
                    foreach (var entity in entities)
                    {
                        if (!HasComponent<TComponent>(entity))
                            throw new EntityNotHaveComponentException(entity, typeof(TComponent));

                        var entityData = _entityDatas[entity.Id];
                        var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                        if (!cachedConfigOffsets.TryGetValue(archeTypeData.ArcheTypeIndex, out var configOffset))
                        {
                            cachedConfigOffsets.Add(archeTypeData.ArcheTypeIndex,
                                archeTypeData.GetComponentConfigOffset(config));
                        }

                        if (config.IsBlittable)
                            archeTypeData.SetComponent(entityData, component, config);
                        else
                            archeTypeData.SetComponent(entityData, component, config, ManagePools.GetPool<TComponent>());
                        TrackerManager.TrackUpdate(entity, config);
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
                UpdateComponentsArcheType(ArcheTypeManager.GetArcheTypeData(entityArcheType), component);
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
                for (var i = 0; i < contextQueryData.ArcheTypeDatas.Length; i++)
                {
                    UpdateComponentsArcheType(contextQueryData.ArcheTypeDatas[i], component);
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
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                ArcheTypeManager.CachedArcheTypeCopyTo(archeTypeData.ArcheType);
                for (var i = 0; i < writeAdapters.Length; i++)
                {
                    var adapter = writeAdapters[i];
                    adapter.UpdateComponent(entityData, archeTypeData);
                    if (adapter.Config.IsShared)
                    {
                        anyChangeArcheType |= ArcheTypeManager.CachedArcheTypeReplaceSharedDataIndex(
                            adapter.GetSharedDataIndex());
                    }
                    TrackerManager.TrackUpdate(entity, adapter.Config);
                }

                if (anyChangeArcheType)
                {
                    entityData = ArcheTypeData.TransferEntity(
                        entity,
                        archeTypeData,
                        ArcheTypeManager.GetCachedArcheTypeData(),
                        _entityDatas,
                        _bookManager);
                }

            }
        }

        #region InternalForEachGetComponents

        public void GetComponentsNoCheck<T1>(Entity entity,
            out T1 component1)
            where T1 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2>(
            Entity entity,
            out T1 component1,
            out T2 component2)
            where T1 : IComponent
            where T2 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2, T3>(
            Entity entity,
            out T1 component1,
            out T2 component2,
            out T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
                component3 = GetForEachComponent<T3>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2, T3, T4>(
            Entity entity,
            out T1 component1,
            out T2 component2,
            out T3 component3,
            out T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));

                /*CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });*/

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
                component3 = GetForEachComponent<T3>(entityData, archeTypeData);
                component4 = GetForEachComponent<T4>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2, T3, T4, T5>(
            Entity entity,
            out T1 component1,
            out T2 component2,
            out T3 component3,
            out T4 component4,
            out T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                });

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
                component3 = GetForEachComponent<T3>(entityData, archeTypeData);
                component4 = GetForEachComponent<T4>(entityData, archeTypeData);
                component5 = GetForEachComponent<T5>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2, T3, T4, T5, T6>(
            Entity entity,
            out T1 component1,
            out T2 component2,
            out T3 component3,
            out T4 component4,
            out T5 component5,
            out T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T6>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T6));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                });

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
                component3 = GetForEachComponent<T3>(entityData, archeTypeData);
                component4 = GetForEachComponent<T4>(entityData, archeTypeData);
                component5 = GetForEachComponent<T5>(entityData, archeTypeData);
                component6 = GetForEachComponent<T6>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2, T3, T4, T5, T6, T7>(
            Entity entity,
            out T1 component1,
            out T2 component2,
            out T3 component3,
            out T4 component4,
            out T5 component5,
            out T6 component6,
            out T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T6>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T6));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T7>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T7));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                });

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
                component3 = GetForEachComponent<T3>(entityData, archeTypeData);
                component4 = GetForEachComponent<T4>(entityData, archeTypeData);
                component5 = GetForEachComponent<T5>(entityData, archeTypeData);
                component6 = GetForEachComponent<T6>(entityData, archeTypeData);
                component7 = GetForEachComponent<T7>(entityData, archeTypeData);
            }
        }

        public void GetComponentsNoCheck<T1, T2, T3, T4, T5, T6, T7, T8>(
            Entity entity,
            out T1 component1,
            out T2 component2,
            out T3 component3,
            out T4 component4,
            out T5 component5,
            out T6 component6,
            out T7 component7,
            out T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T6>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T6));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T7>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T7));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T8>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T8));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                });

                component1 = GetForEachComponent<T1>(entityData, archeTypeData);
                component2 = GetForEachComponent<T2>(entityData, archeTypeData);
                component3 = GetForEachComponent<T3>(entityData, archeTypeData);
                component4 = GetForEachComponent<T4>(entityData, archeTypeData);
                component5 = GetForEachComponent<T5>(entityData, archeTypeData);
                component6 = GetForEachComponent<T6>(entityData, archeTypeData);
                component7 = GetForEachComponent<T7>(entityData, archeTypeData);
                component8 = GetForEachComponent<T8>(entityData, archeTypeData);
            }
        }

        private TComponent GetForEachComponent<TComponent>(EntityData entityData, ArcheTypeData archeTypeData)
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (config.IsBlittable)
                return archeTypeData.GetComponent<TComponent>(entityData, config);
            else
                return archeTypeData.GetComponent(entityData, config, ManagePools.GetPool<TComponent>());
        }

        private static void CheckDuplicateConfigs(ComponentConfig[] configs)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                for (var j = i + 1; j < configs.Length; j++)
                {
                    if (configs[i] == configs[j])
                    {
                        throw new EntityQueryDuplicateComponentException(
                            ComponentConfigs.Instance.AllComponentTypes[configs[i].ComponentIndex]);
                    }
                }
            }
        }

        #endregion

        #region InternalForEachGetComponents

        public void SetComponentsNoCheck<T1>(Entity entity,
            in T1 component1)
            where T1 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        public void SetComponentsNoCheck<T1, T2>(Entity entity,
            in T1 component1,
            in T2 component2)
            where T1 : IComponent
            where T2 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component2);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        public void SetComponentsNoCheck<T1, T2, T3>(Entity entity,
            in T1 component1,
            in T2 component2,
            in T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component2);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component3);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        private readonly List<SharedComponentDataIndex> _cacheSharedIndexes = new List<SharedComponentDataIndex>();
        public void SetComponentsNoCheck<T1, T2, T3, T4>(Entity entity,
            in T1 component1,
            in T2 component2,
            in T3 component3,
            in T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                _cacheSharedIndexes.Clear();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));

                /*CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });*/

                SetForEachComponent(_cacheSharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(_cacheSharedIndexes, entityData, archeTypeData, component2);
                SetForEachComponent(_cacheSharedIndexes, entityData, archeTypeData, component3);
                SetForEachComponent(_cacheSharedIndexes, entityData, archeTypeData, component4);

                SetForEachArcheTypeData(entity, _cacheSharedIndexes, archeTypeData);
            }
        }

        public void SetComponentsNoCheck<T1, T2, T3, T4, T5>(Entity entity,
            in T1 component1,
            in T2 component2,
            in T3 component3,
            in T4 component4,
            in T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                });

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component2);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component3);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component4);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component5);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        public void SetComponentsNoCheck<T1, T2, T3, T4, T5, T6>(Entity entity,
            in T1 component1,
            in T2 component2,
            in T3 component3,
            in T4 component4,
            in T5 component5,
            in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T6>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T6));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                });

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component2);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component3);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component4);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component5);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component6);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        public void SetComponentsNoCheck<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            in T1 component1,
            in T2 component2,
            in T3 component3,
            in T4 component4,
            in T5 component5,
            in T6 component6,
            in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T6>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T6));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T7>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T7));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                });

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component2);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component3);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component4);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component5);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component6);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component7);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        public void SetComponentsNoCheck<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            in T1 component1,
            in T2 component2,
            in T3 component3,
            in T4 component4,
            in T5 component5,
            in T6 component6,
            in T7 component7,
            in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            lock (LockObj)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);
                var sharedIndexes = new List<SharedComponentDataIndex>();

                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T1>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T1));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T2>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T2));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T3>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T3));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T4>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T4));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T5>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T5));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T6>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T6));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T7>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T7));
                if (!archeTypeData.ArcheType.HasComponentConfig(ComponentConfig<T8>.Config))
                    throw new EntityNotHaveComponentException(entity, typeof(T8));

                CheckDuplicateConfigs(new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                });

                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component1);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component2);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component3);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component4);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component5);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component6);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component7);
                SetForEachComponent(sharedIndexes, entityData, archeTypeData, component8);

                SetForEachArcheTypeData(entity, sharedIndexes, archeTypeData);
            }
        }

        private void SetForEachComponent<TComponent>(
            List<SharedComponentDataIndex> sharedIndexes,
            EntityData entityData,
            ArcheTypeData archeTypeData,
            in TComponent component)
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
            {
                sharedIndexes.Add(new SharedComponentDataIndex
                {
                    SharedIndex = config.SharedIndex,
                    SharedDataIndex = SharedIndexDics.GetSharedIndexDic<TComponent>()
                        .GetOrAdd(component)
                });
            }
            if (config.IsManaged)
            {
                archeTypeData.SetComponent(entityData, component, config,
                    ManagePools.GetPool<TComponent>());
            }
            else
            {
                archeTypeData.SetComponent(entityData, component, config);
            }
        }

        private void SetForEachArcheTypeData(
            Entity entity,
            List<SharedComponentDataIndex> sharedIndexes,
            ArcheTypeData archeTypeData)
        {
            if (sharedIndexes.Count == 0)
                return;

            ArcheTypeManager.CachedArcheTypeCopyTo(archeTypeData.ArcheType);
            for (var i = 0; i < sharedIndexes.Count; i++)
                ArcheTypeManager.CachedArcheTypeReplaceSharedDataIndex(sharedIndexes[i]);

            var nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();
            if (archeTypeData != nextArcheTypeData)
            {
                ArcheTypeData.TransferEntity(
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas,
                    _bookManager);
            }
        }

        #endregion

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
                TrackerManager.InternalDestroy();
                _bookManager.InternalDestroy();
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
                Array.Resize(ref _cachedDestroyEntities, newCapacity);
                TrackerManager.ResizeTrackers(newCapacity);
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
            TrackerManager.TrackDestroy(entity);
            _entityDatas[entity.Id] = new EntityData();
            _entities[entity.Id] = Entity.Null;
            _reusableEntities.Push(entity);
            _isCachedEntitiesDirty = true;
            _entityCount--;
        }

        private void DestroyEntityNoCheck(Entity entity)
        {
            var archeTypeData = ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex);
            ClearUniqueComponents(archeTypeData);
            archeTypeData.RemoveEntity(entity, _entityDatas, ManagePools, _bookManager);
            DeallocateEntity(entity);
        }

        private void DestroyEntities(ArcheTypeData archeTypeData)
        {
            ClearUniqueComponents(archeTypeData);
            var entitiesCount = 0;
            archeTypeData.RemoveAllEntities(_entityDatas,
                ManagePools,
                _bookManager,
                ref _cachedDestroyEntities,
                ref entitiesCount);
            for (var i = 0; i < entitiesCount; i++)
                DeallocateEntity(_cachedDestroyEntities[i]);
        }

        private void CheckAndSetUniqueComponents(Entity entity, ArcheTypeData archeTypeData)
        {
            for (var i = 0; i < archeTypeData.UniqueConfigsLength; i++)
            {
                var config = archeTypeData.UniqueConfigs[i];
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }

                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }
        }

        private void CheckUniqueComponents(ArcheTypeData archeTypeData)
        {
            for (var i = 0; i < archeTypeData.UniqueConfigsLength; i++)
            {
                var config = archeTypeData.UniqueConfigs[i];
                var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                if (uniqueEntity == Entity.Null)
                    uniqueEntity = AllocateEntity();

                throw new EntityAlreadyHasComponentException(
                    uniqueEntity,
                    ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
            }
        }

        private void ClearUniqueComponents(ArcheTypeData archeTypeData)
        {
            for (var i = 0; i < archeTypeData.UniqueConfigsLength; i++)
                _uniqueComponentEntities[archeTypeData.UniqueConfigs[i].UniqueIndex] = Entity.Null;
        }

        private void TransferArcheTypeDiffContext(EcsContext sourceContext, ArcheTypeData prevArcheTypeData, ref Entity[] nextEntities, int startEntityIndex)
        {
            var entitiesLength = prevArcheTypeData.EntityCount;
            CheckCapacity(entitiesLength);

            ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData.ArcheType);
            var nextArcheTypeData = ArcheTypeManager.GetCachedArcheTypeData();

            var componentIndexes = new int[nextArcheTypeData.ManagedConfigsLength][];
            for (var i = 0; i < nextArcheTypeData.ManagedConfigsLength; i++)
            {
                componentIndexes[i] = ManagePools.GetPool(nextArcheTypeData.ManagedConfigs[i])
                    .AllocateComponents(entitiesLength);
            }

            nextArcheTypeData.PrecheckEntityAllocation(entitiesLength, _bookManager);
            for (var i = 0; i < entitiesLength; i++)
            {
                var prevEntity = prevArcheTypeData.GetEntity(i);
                var prevEntityData = sourceContext._entityDatas[prevEntity.Id];

                var nextEntity = AllocateEntity();
                for (var j = 0; j < prevArcheTypeData.ArcheType.ComponentConfigLength; j++)
                    TrackerManager.TrackAdd(nextEntity, prevArcheTypeData.ArcheType.ComponentConfigs[j]);

                var nextEntityData = nextArcheTypeData.AddEntity(nextEntity, _bookManager);
                _entityDatas[nextEntity.Id] = nextEntityData;
                nextEntities[i + startEntityIndex] = nextEntity;

                nextArcheTypeData.SetComponentsBuffer(nextEntityData, prevEntityData.Slot.Buffer);
                for (var j = 0; j < nextArcheTypeData.ManagedConfigsLength; j++)
                {
                    var config = nextArcheTypeData.ManagedConfigs[j];
                    nextArcheTypeData.SetComponentAndIndex(
                        nextEntityData,
                        prevArcheTypeData.GetComponent(
                            prevEntityData,
                            config,
                            sourceContext.ManagePools.GetPool(config)),
                        config,
                        componentIndexes[j][i],
                        ManagePools.GetPool(config));
                }
            }
        }

        private void UpdateComponentsArcheType<TComponent>(ArcheTypeData prevArcheTypeData, TComponent component) where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
            {
                ArcheTypeManager.CachedArcheTypeCopyTo(prevArcheTypeData.ArcheType);
                var nextArcheTypeData = ArcheTypeManager.CachedArcheTypeGetNextArcheTypeData(prevArcheTypeData,
                    SharedIndexDics.GetDataIndex(component));

                if (nextArcheTypeData != prevArcheTypeData)
                    ArcheTypeData.TransferAllEntities(
                        prevArcheTypeData,
                        nextArcheTypeData,
                        _entityDatas,
                        _bookManager);

                if (config.IsBlittable)
                {
                    nextArcheTypeData.SetAllComponents(component, config);
                }
                else
                {
                    nextArcheTypeData.SetAllComponents(component, config, ManagePools.GetPool<TComponent>());
                }
                prevArcheTypeData = nextArcheTypeData;
            }
            else
            {
                if (config.IsBlittable)
                    prevArcheTypeData.SetAllComponents(component, config);
                else
                    prevArcheTypeData.SetAllComponents(component, config, ManagePools.GetPool<TComponent>());
            }
            for (var i = 0; i < prevArcheTypeData.EntityCount; i++)
                TrackerManager.TrackUpdate(prevArcheTypeData.GetEntity(i), config);
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