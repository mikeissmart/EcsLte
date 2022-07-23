using EcsLte.Data;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public unsafe class EntityManager
    {
        private Entity* _entities;
        private EntityState* _entityStates;
        private Entity[] _cachedEntities;
        private Entity[] _cachedInternalEntities;
        private bool _isCachedEntitiesDirty;
        private EntityData* _entityDatas;
        private int _nextId;
        private int _entityCount;
        private int _entityLength;
        private Stack<Entity> _reusableEntities;
        private Entity* _uniqueComponentEntities;
        private ArcheType _cachedArcheType;
        private readonly MemoryBookManager _bookManager;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }
        internal int EntityCapacity => _entityLength;

        internal EntityManager(EcsContext context)
        {
            _entities = MemoryHelper.Alloc<Entity>(1);
            _entityStates = (EntityState*)MemoryHelper.Alloc<int>(1);
            _cachedEntities = new Entity[0];
            _cachedInternalEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _entityDatas = MemoryHelper.Alloc<EntityData>(1);
            _nextId = 1;
            _reusableEntities = new Stack<Entity>();
            if (ComponentConfigs.Instance.AllUniqueCount > 0)
                _uniqueComponentEntities = MemoryHelper.Alloc<Entity>(ComponentConfigs.Instance.AllUniqueCount);
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);
            _bookManager = new MemoryBookManager();
            _lockObj = new object();

            Context = context;
        }

        #region EntityCount

        public int EntityCount()
        {
            Context.AssertContext();

            return _entityCount;
        }

        public int EntityCount(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                return Context.ArcheTypeManager.GetArcheTypeData(archeType)
                    .EntityCount;
            }
        }

        public int EntityCount(EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                var entityCount = 0;
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;

                for (var i = 0; i < archeTypeDatas.Length; i++)
                    entityCount += archeTypeDatas[i].EntityCount;

                return entityCount;
            }
        }

        public int EntityCount(EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            return tracker.CachedEntities.Length;
        }

        public int EntityCount(EntityQuery query)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            var entityCount = EntityCount(query.Filter);
            if (query.Tracker != null)
                entityCount = Math.Min(entityCount, EntityCount(query.Tracker));

            return entityCount;
        }

        #endregion

        #region EntityHas

        public bool HasEntity(Entity entity)
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                return InternalHasEntity(entity);
            }
        }

        public bool HasEntity(Entity entity, EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                if (entity.Id > 0 &&
                    entity.Id < _entityLength &&
                    _entities[entity.Id] == entity)
                {
                    var entityData = _entityDatas[entity.Id];
                    var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);

                    return entityData.ArcheTypeIndex == archeTypeData.ArcheTypeIndex;
                }

                return false;
            }
        }

        public bool HasEntity(Entity entity, EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                return InternalHasEntity(entity) &&
                    InternalHasEntity(entity, filter);
            }
        }

        public bool HasEntity(Entity entity, EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                return InternalHasEntity(entity) &&
                    InternalHasEntity(entity, tracker);
            }
        }

        public bool HasEntity(Entity entity, EntityQuery query)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            lock (_lockObj)
            {
                return InternalHasEntity(entity, query);
            }
        }

        #endregion

        #region EntityGet

        public Entity[] GetEntities()
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                if (_isCachedEntitiesDirty)
                {
                    if (_cachedEntities.Length != _entityCount)
                        Array.Resize(ref _cachedEntities, _entityCount);
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
        }

        public int GetEntities(ref Entity[] entities) => GetEntities(ref entities, 0);

        public int GetEntities(ref Entity[] entities, int startingIndex)
        {
            var cachedEntities = GetEntities();

            AssertResizeEntities(ref entities, startingIndex, cachedEntities.Length);
            Array.Copy(cachedEntities, 0, entities, startingIndex, cachedEntities.Length);

            return cachedEntities.Length;
        }

        public Entity[] GetEntities(EntityArcheType archeType)
        {
            var entities = new Entity[0];
            GetEntities(archeType, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityArcheType archeType, ref Entity[] entities) => GetEntities(archeType, ref entities, 0);

        public int GetEntities(EntityArcheType archeType, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);
                AssertResizeEntities(ref entities, startingIndex, archeTypeData.EntityCount);

                archeTypeData.GetEntities(ref entities, startingIndex);

                return archeTypeData.EntityCount;
            }
        }

        public Entity[] GetEntities(EntityFilter filter)
        {
            var entities = new Entity[0];
            GetEntities(filter, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityFilter filter, ref Entity[] entities) => GetEntities(filter, ref entities, 0);

        public int GetEntities(EntityFilter filter, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                var entityIndex = startingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    AssertResizeEntities(ref entities, entityIndex, archeTypeData.EntityCount);
                    archeTypeData.GetEntities(ref entities, entityIndex);

                    entityIndex += archeTypeData.EntityCount;
                }

                return entityIndex - startingIndex;
            }
        }

        public Entity[] GetEntities(EntityTracker tracker)
        {
            var entities = new Entity[0];
            GetEntities(tracker, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityTracker tracker, ref Entity[] entities) => GetEntities(tracker, ref entities, 0);

        public int GetEntities(EntityTracker tracker, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                AssertResizeEntities(ref entities, startingIndex, tracker.CachedEntities.Length);
                Array.Copy(tracker.CachedEntities, 0, entities, startingIndex, tracker.CachedEntities.Length);

                return tracker.CachedEntities.Length;
            }
        }

        public Entity[] GetEntities(EntityQuery query)
        {
            var entities = new Entity[0];
            GetEntities(query, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityQuery query, ref Entity[] entities) => GetEntities(query, ref entities, 0);

        public int GetEntities(EntityQuery query, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Tracker == null)
                return GetEntities(query.Filter, ref entities, startingIndex);

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                var entityIndex = startingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                        ref _cachedInternalEntities, 0);

                    if (trackedEntityCount > 0)
                    {
                        AssertResizeEntities(ref entities, entityIndex, trackedEntityCount);
                        Array.Copy(_cachedInternalEntities, 0, entities, entityIndex, trackedEntityCount);

                        entityIndex += trackedEntityCount;
                    }
                }

                return entityIndex - startingIndex;
            }
        }

        public EntityArcheType GetArcheType(Entity entity)
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                return new EntityArcheType(Context,
                    Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
            }
        }

        #endregion

        #region EntityCreate

        public Entity CreateEntity(EntityArcheType archeType, EntityState state)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                CheckCapacity(1);

                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);

                CreateAndAddArcheType(state, archeTypeData, out var entity, out var _);

                AssertSetUniqueComponents(entity, archeTypeData);
                for (var i = 0; i < archeTypeData.UniqueConfigs.Length; i++)
                    _uniqueComponentEntities[archeTypeData.UniqueConfigs[i].UniqueIndex] = entity;

                return entity;
            }
        }

        public Entity CreateEntity(EntityBlueprint blueprint, EntityState state)
        {
            Context.AssertContext();
            EntityBlueprint.AssertEntityBlueprint(blueprint);

            lock (_lockObj)
            {
                CheckCapacity(1);

                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(blueprint.GetBlueprintArcheType());

                var tempComponentBuffer = stackalloc byte[archeTypeData.ComponentsSizeInBytes];
                archeTypeData.CopyComponentDatasToBuffer(blueprint.GeneralComponentDatas,
                    tempComponentBuffer);
                archeTypeData.CopyComponentDatasToBuffer(blueprint.UniqueComponentDatas,
                    tempComponentBuffer);

                CreateAndAddArcheType(state, archeTypeData, out var entity, out var entityData);

                AssertSetUniqueComponents(entity, archeTypeData);

                archeTypeData.SetComponentsFromBuffer(tempComponentBuffer, entityData);

                return entity;
            }
        }

        public Entity[] CreateEntities(EntityArcheType archeType, EntityState state, int count)
        {
            var entities = new Entity[count];
            CreateEntities(archeType, state, ref entities, 0, count);

            return entities;
        }

        public void CreateEntities(EntityArcheType archeType, EntityState state, ref Entity[] entities, int count) => CreateEntities(archeType, state, ref entities, 0, count);

        public void CreateEntities(EntityArcheType archeType, EntityState state, ref Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            AssertResizeEntities(ref entities, startingIndex, count);

            if (count == 0)
                return;
            if (count == 1)
            {
                entities[startingIndex] = CreateEntity(archeType, state);
                return;
            }

            lock (_lockObj)
            {
                CheckCapacity(count);

                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);
                AssertUniqueComponentsThrow(archeTypeData);

                archeTypeData.PrecheckEntityAllocation(count, _bookManager);

                for (int i = 0, entityIndex = startingIndex; i < count; i++, entityIndex++)
                {
                    CreateAndAddArcheType(state, archeTypeData, out var entity, out var entityData);

                    entities[entityIndex] = entity;
                }
            }
        }

        public Entity[] CreateEntities(EntityBlueprint blueprint, EntityState state, int count)
        {
            var entities = new Entity[count];
            CreateEntities(blueprint, state, ref entities, 0, count);

            return entities;
        }

        public void CreateEntities(EntityBlueprint blueprint, EntityState state, ref Entity[] entities, int count) => CreateEntities(blueprint, state, ref entities, 0, count);

        public void CreateEntities(EntityBlueprint blueprint, EntityState state, ref Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            EntityBlueprint.AssertEntityBlueprint(blueprint);

            AssertResizeEntities(ref entities, startingIndex, count);

            if (count == 0)
                return;
            if (count == 1)
            {
                entities[startingIndex] = CreateEntity(blueprint, state);
                return;
            }

            Context.AssertContext();

            lock (_lockObj)
            {
                CheckCapacity(count);

                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(blueprint.GetBlueprintArcheType());

                AssertUniqueComponentsThrow(archeTypeData);

                var tempComponentBuffer = stackalloc byte[archeTypeData.ComponentsSizeInBytes];
                archeTypeData.CopyComponentDatasToBuffer(blueprint.GeneralComponentDatas,
                    tempComponentBuffer);
                archeTypeData.CopyComponentDatasToBuffer(blueprint.UniqueComponentDatas,
                    tempComponentBuffer);

                archeTypeData.PrecheckEntityAllocation(count, _bookManager);

                for (int i = 0, entityIndex = startingIndex; i < count; i++, entityIndex++)
                {
                    CreateAndAddArcheType(state, archeTypeData, out var entity, out var entityData);

                    entities[entityIndex] = entity;

                    archeTypeData.SetComponentsFromBuffer(tempComponentBuffer, entityData);
                }
            }
        }

        #endregion

        #region EntityDestroy

        public void DestroyEntity(Entity entity)
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                DestroyEntityAndRemoveArcheType(entity,
                    Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
            }
        }

        public void DestroyEntities(in Entity[] entities) => DestroyEntities(entities, 0, entities?.Length ?? 0);

        public void DestroyEntities(in Entity[] entities, int startingIndex) => DestroyEntities(entities, startingIndex, (entities?.Length ?? 0) - startingIndex);

        public void DestroyEntities(in Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            AssertEntities(entities, startingIndex, count);

            lock (_lockObj)
            {
                for (int i = 0, entityIndex = startingIndex; i < count; i++, entityIndex++)
                {
                    var entity = entities[entityIndex];
                    AssertHasEntity(entity);

                    DestroyEntityAndRemoveArcheType(entity,
                        Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
                }
            }
        }

        public void DestroyEntities(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                ArcheTypeRemoveAllEntities(Context.ArcheTypeManager.GetArcheTypeData(archeType));
            }
        }

        public void DestroyEntities(EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                    ArcheTypeRemoveAllEntities(archeTypeDatas[i]);
            }
        }

        public void DestroyEntities(EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                var entityCount = tracker.CachedEntities.Length;
                ResizeRefEntities(ref _cachedInternalEntities, 0, entityCount);
                Array.Copy(tracker.CachedEntities, _cachedInternalEntities, entityCount);

                for (var i = 0; i < entityCount; i++)
                {
                    var entity = _cachedInternalEntities[i];

                    DestroyEntityAndRemoveArcheType(entity,
                        Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
                }
            }
        }

        public void DestroyEntities(EntityQuery query)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Tracker == null)
            {
                DestroyEntities(query.Filter);
                return;
            }

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                        ref _cachedInternalEntities, 0);

                    if (trackedEntityCount > 0)
                    {
                        for (var j = 0; j < trackedEntityCount; j++)
                            DestroyEntityAndRemoveArcheType(_cachedInternalEntities[j], archeTypeData);
                    }
                }
            }
        }

        #endregion

        #region EntityStateGet

        public EntityState GetEntityState(Entity entity)
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                return _entityStates[entity.Id];
            }
        }

        public EntityState[] GetEntityStates(in Entity[] entities)
        {
            var states = new EntityState[0];
            GetEntityStates(entities, 0, entities?.Length ?? 0,
                ref states, 0);

            return states;
        }

        public EntityState[] GetEntityStates(in Entity[] entities, int startingIndex)
        {
            var states = new EntityState[0];
            GetEntityStates(entities, startingIndex, (entities?.Length ?? 0) - startingIndex,
                ref states, 0);

            return states;
        }

        public EntityState[] GetEntityStates(in Entity[] entities, int startingIndex, int count)
        {
            var states = new EntityState[0];
            GetEntityStates(entities, startingIndex, count,
                ref states, 0);

            return states;
        }

        public int GetEntityStates(in Entity[] entities,
            ref EntityState[] destStates)
        {
            GetEntityStates(entities, 0, entities?.Length ?? 0,
                ref destStates, 0);

            return entities.Length;
        }

        public int GetEntityStates(in Entity[] entities, int startingIndex,
            ref EntityState[] destStates)
        {
            GetEntityStates(entities, startingIndex, (entities?.Length ?? 0) - startingIndex,
                ref destStates, 0);

            return entities.Length - startingIndex;
        }

        public void GetEntityStates(in Entity[] entities, int startingIndex, int count,
            ref EntityState[] destStates) => GetEntityStates(entities, startingIndex, count,
                ref destStates, 0);

        public int GetEntityStates(in Entity[] entities,
            ref EntityState[] destStates, int destStartingIndex)
        {
            GetEntityStates(entities, 0, entities?.Length ?? 0,
                ref destStates, destStartingIndex);

            return entities.Length;
        }

        public int GetEntityStates(in Entity[] entities, int startingIndex,
            ref EntityState[] destStates, int destStartingIndex)
        {
            GetEntityStates(entities, startingIndex, (entities?.Length ?? 0) - startingIndex,
                ref destStates, destStartingIndex);

            return entities.Length - startingIndex;
        }

        public void GetEntityStates(in Entity[] entities, int startingIndex, int count,
            ref EntityState[] destStates, int destStartingIndex)
        {
            Context.AssertContext();
            AssertEntities(entities, startingIndex, count);
            AssertResizeEntityStates(ref destStates, destStartingIndex, count);

            lock (_lockObj)
            {
                for (int i = 0, entityIndex = startingIndex, stateIndex = destStartingIndex;
                    i < count;
                    i++, entityIndex++, stateIndex++)
                {
                    var entity = entities[entityIndex];
                    AssertHasEntity(entity);

                    destStates[stateIndex] = _entityStates[entity.Id];
                }
            }
        }

        public EntityState[] GetEntityStates(EntityArcheType archeType)
        {
            var states = new EntityState[0];
            GetEntityStates(archeType, ref states, 0);

            return states;
        }

        public int GetEntityStates(EntityArcheType archeType,
            ref EntityState[] destStates) => GetEntityStates(archeType, ref destStates, 0);

        public int GetEntityStates(EntityArcheType archeType,
            ref EntityState[] destStates, int destStartingIndex)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);

                AssertResizeEntityStates(ref destStates, destStartingIndex, archeTypeData.EntityCount);
                for (int i = 0, stateIndex = destStartingIndex;
                    i < archeTypeData.EntityCount;
                    i++, stateIndex++)
                {
                    destStates[stateIndex] = _entityStates[archeTypeData.GetEntity(i).Id];
                }

                return archeTypeData.EntityCount;
            }
        }

        public EntityState[] GetEntityStates(EntityFilter filter)
        {
            var states = new EntityState[0];
            GetEntityStates(filter, ref states, 0);

            return states;
        }

        public int GetEntityStates(EntityFilter filter,
            ref EntityState[] destStates) => GetEntityStates(filter, ref destStates, 0);

        public int GetEntityStates(EntityFilter filter,
            ref EntityState[] destStates, int destStartingIndex)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);
            AssertEntityStates(destStates, destStartingIndex);

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;

                var stateIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];

                    ResizeRefEntityStates(ref destStates, stateIndex, archeTypeData.EntityCount);
                    for (var j = 0; j < archeTypeData.EntityCount; j++)
                        destStates[stateIndex++] = _entityStates[archeTypeData.GetEntity(j).Id];
                }

                return stateIndex - destStartingIndex;
            }
        }

        public EntityState[] GetEntityStates(EntityTracker tracker)
        {
            var states = new EntityState[0];
            GetEntityStates(tracker, ref states, 0);

            return states;
        }

        public int GetEntityStates(EntityTracker tracker,
            ref EntityState[] destStates) => GetEntityStates(tracker, ref destStates, 0);

        public int GetEntityStates(EntityTracker tracker,
            ref EntityState[] destStates, int destStartingIndex)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            AssertEntityStates(destStates, destStartingIndex);

            lock (_lockObj)
            {
                ResizeRefEntityStates(ref destStates, destStartingIndex, tracker.CachedEntities.Length);
                for (int i = 0, stateIndex = destStartingIndex;
                    i < tracker.CachedEntities.Length;
                    i++, stateIndex++)
                {
                    destStates[stateIndex] = _entityStates[tracker.CachedEntities[i].Id];
                }

                return tracker.CachedEntities.Length;
            }
        }

        public EntityState[] GetEntityStates(EntityQuery query)
        {
            var states = new EntityState[0];
            GetEntityStates(query, ref states, 0);

            return states;
        }

        public int GetEntityStates(EntityQuery query,
            ref EntityState[] destStates) => GetEntityStates(query, ref destStates, 0);

        public int GetEntityStates(EntityQuery query,
            ref EntityState[] destStates, int destStartingIndex)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);
            AssertEntityStates(destStates, destStartingIndex);

            if (query.Tracker == null)
                return GetEntityStates(query.Filter, ref destStates, destStartingIndex);

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                var stateIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                        ref _cachedInternalEntities, 0);

                    if (trackedEntityCount > 0)
                    {
                        ResizeRefEntityStates(ref destStates, stateIndex, trackedEntityCount);
                        for (var j = 0; j < trackedEntityCount; j++)
                            destStates[stateIndex++] = _entityStates[_cachedInternalEntities[j].Id];
                    }
                }

                return stateIndex - destStartingIndex;
            }
        }

        #endregion

        #region EntityStateSet

        public void SetEntityState(Entity entity, EntityState state)
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                if (_entityStates[entity.Id] != state)
                {
                    _entityStates[entity.Id] = state;
                    Context.Tracking.TrackStateChange(entity,
                        Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
                }
            }
        }

        public void SetEntityStates(in Entity[] entities, EntityState state) => SetEntityStates(in entities, 0, entities?.Length ?? 0, state);

        public void SetEntityStates(in Entity[] entities, int startingIndex, EntityState state) => SetEntityStates(in entities, startingIndex, (entities?.Length ?? 0) - startingIndex, state);

        public void SetEntityStates(in Entity[] entities, int startingIndex, int count, EntityState state)
        {
            Context.AssertContext();
            AssertEntities(entities, startingIndex, count);

            lock (_lockObj)
            {
                for (int i = 0, entityIndex = startingIndex; i < count; i++, entityIndex++)
                {
                    var entity = entities[entityIndex];
                    AssertHasEntity(entity);

                    if (_entityStates[entity.Id] != state)
                    {
                        _entityStates[entity.Id] = state;
                        Context.Tracking.TrackStateChange(entity,
                            Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
                    }
                }
            }
        }

        public void SetEntityStates(EntityArcheType archeType, EntityState state)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                InternalChangeArcheTypeState(Context.ArcheTypeManager.GetArcheTypeData(archeType), state);
            }
        }

        public void SetEntityStates(EntityFilter filter, EntityState state)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                    InternalChangeArcheTypeState(archeTypeDatas[i], state);
            }
        }

        public void SetEntityStates(EntityTracker tracker, EntityState state)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                var entityCount = tracker.CachedEntities.Length;
                ResizeRefEntities(ref _cachedInternalEntities, 0, entityCount);
                Array.Copy(tracker.CachedEntities, _cachedInternalEntities, entityCount);

                for (var i = 0; i < entityCount; i++)
                {
                    var entity = _cachedInternalEntities[i];
                    if (_entityStates[entity.Id] != state)
                    {
                        _entityStates[entity.Id] = state;
                        Context.Tracking.TrackStateChange(entity,
                            Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex));
                    }
                }
            }
        }

        public void SetEntityStates(EntityQuery query, EntityState state)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Tracker == null)
            {
                SetEntityStates(query.Filter, state);
                return;
            }

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                        ref _cachedInternalEntities, 0);

                    if (trackedEntityCount > 0)
                    {
                        for (var j = 0; j < trackedEntityCount; j++)
                        {
                            var entity = _cachedInternalEntities[j];
                            if (_entityStates[entity.Id] != state)
                            {
                                _entityStates[entity.Id] = state;
                                Context.Tracking.TrackStateChange(entity, archeTypeData);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region EntityDuplicate

        public Entity DuplicateEntity(Entity srcEntity, EntityState? state = null)
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(srcEntity);

                CheckCapacity(1);

                var entityData = _entityDatas[srcEntity.Id];

                return InternalDuplicateEntity(entityData, state ?? _entityStates[srcEntity.Id],
                    Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex));
            }
        }

        public Entity[] DuplicateEntities(in Entity[] srcEntities,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0,
                ref entities, 0,
                state);

            return entities;
        }

        public Entity[] DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref entities, 0,
                state);

            return entities;
        }

        public Entity[] DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(srcEntities, srcStartingIndex, srcCount,
                ref entities, 0,
                state);

            return entities;
        }

        public int DuplicateEntities(in Entity[] srcEntities,
            ref Entity[] destEntities,
            EntityState? state = null)
        {
            DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, 0,
                state);

            return srcEntities.Length;
        }

        public int DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities,
            EntityState? state = null)
        {
            DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, 0,
                state);

            return srcEntities.Length - srcStartingIndex;
        }

        public void DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities,
            EntityState? state = null) => DuplicateEntities(srcEntities, srcStartingIndex, srcCount,
                ref destEntities, 0,
                state);

        public int DuplicateEntities(in Entity[] srcEntities,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, destStartingIndex,
                state);

            return srcEntities.Length;
        }

        public int DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, destStartingIndex,
                state);

            return srcEntities.Length - srcStartingIndex;
        }

        public void DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            AssertEntities(srcEntities, srcStartingIndex, srcCount);
            AssertResizeEntities(ref destEntities, destStartingIndex, srcCount);

            lock (_lockObj)
            {
                CheckCapacity(srcCount);

                for (int i = 0, entityIndex = srcStartingIndex, destEntityIndex = destStartingIndex;
                    i < srcCount;
                    i++, entityIndex++, destEntityIndex++)
                {
                    var entity = srcEntities[entityIndex];
                    AssertHasEntity(entity);

                    var entityData = _entityDatas[entity.Id];

                    destEntities[destEntityIndex] = InternalDuplicateEntity(entityData, state ?? _entityStates[entity.Id],
                        Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex));
                }
            }
        }

        public Entity[] DuplicateEntities(EntityArcheType archeType,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(archeType, ref entities, 0, state);

            return entities;
        }

        public int DuplicateEntities(EntityArcheType archeType,
            ref Entity[] destEntities,
            EntityState? state = null) => DuplicateEntities(archeType, ref destEntities, 0, state);

        public int DuplicateEntities(EntityArcheType archeType,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);
                AssertResizeEntities(ref destEntities, destStartingIndex, archeTypeData.EntityCount);

                return InternalDuplicateArcheType(archeTypeData,
                    ref destEntities, destStartingIndex, state);
            }
        }

        public Entity[] DuplicateEntities(EntityFilter filter,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(filter, ref entities, 0, state);

            return entities;
        }

        public int DuplicateEntities(EntityFilter filter,
            ref Entity[] destEntities,
            EntityState? state = null) => DuplicateEntities(filter, ref destEntities, 0, state);

        public int DuplicateEntities(EntityFilter filter,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);
            AssertResizeEntities(ref destEntities, destStartingIndex, 1);

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                var entityIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    ResizeRefEntities(ref destEntities, destStartingIndex, archeTypeData.EntityCount);

                    entityIndex += InternalDuplicateArcheType(archeTypeData, ref destEntities,
                        entityIndex, state);
                }

                return entityIndex - destStartingIndex;
            }
        }

        public Entity[] DuplicateEntities(EntityTracker tracker,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(tracker, ref entities, 0, state);

            return entities;
        }

        public int DuplicateEntities(EntityTracker tracker,
            ref Entity[] destEntities,
            EntityState? state = null) => DuplicateEntities(tracker, ref destEntities, 0, state);

        public int DuplicateEntities(EntityTracker tracker,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            AssertResizeEntities(ref destEntities, destStartingIndex, 1);

            lock (_lockObj)
            {
                CheckCapacity(tracker.CachedEntities.Length);

                var entityCount = tracker.CachedEntities.Length;
                ResizeRefEntities(ref _cachedInternalEntities, 0, entityCount);
                Array.Copy(tracker.CachedEntities, _cachedInternalEntities, entityCount);

                ResizeRefEntities(ref destEntities, destStartingIndex, entityCount);

                for (int i = 0, destEntityIndex = destStartingIndex;
                    i < entityCount;
                    i++, destEntityIndex++)
                {
                    var entity = _cachedInternalEntities[i];
                    var entityData = _entityDatas[entity.Id];

                    try
                    {
                        destEntities[destEntityIndex] = InternalDuplicateEntity(entityData, state ?? _entityStates[entity.Id],
                            Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex));
                    }
                    catch
                    {
                        ;
                    }
                }

                return entityCount;
            }
        }

        public Entity[] DuplicateEntities(EntityQuery query,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            DuplicateEntities(query, ref entities, 0, state);

            return entities;
        }

        public int DuplicateEntities(EntityQuery query,
            ref Entity[] destEntities,
            EntityState? state = null) => DuplicateEntities(query, ref destEntities, 0, state);

        public int DuplicateEntities(EntityQuery query,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);
            AssertResizeEntities(ref destEntities, destStartingIndex, 1);

            if (query.Tracker == null)
                return DuplicateEntities(query.Filter, ref destEntities, destStartingIndex, state);

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                var destEntityIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                        ref _cachedInternalEntities, 0);

                    if (trackedEntityCount > 0)
                    {
                        ResizeRefEntities(ref destEntities, destEntityIndex, trackedEntityCount);

                        for (var j = 0; j < trackedEntityCount; j++)
                        {
                            var entity = _cachedInternalEntities[j];
                            var entityData = _entityDatas[entity.Id];

                            destEntities[destEntityIndex++] = InternalDuplicateEntity(
                                entityData,
                                state ?? _entityStates[entity.Id],
                                archeTypeData);
                        }
                    }
                }

                return destEntityIndex - destStartingIndex;
            }
        }

        #endregion

        #region EntityCopy

        public Entity CopyEntity(EntityManager srcEntityManager,
            Entity srcEntity,
            EntityState? state = null)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopySameContextException();

            Context.AssertContext();
            srcEntityManager.Context.AssertContext();

            lock (_lockObj)
            {
                lock (srcEntityManager._lockObj)
                {
                    srcEntityManager.AssertHasEntity(srcEntity);

                    CheckCapacity(1);

                    return InternalCopyEntity(srcEntityManager, srcEntity, state);
                }
            }
        }

        public Entity[] CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(srcEntityManager,
                srcEntities, 0, srcEntities?.Length ?? 0,
                ref entities, 0,
                state);

            return entities;
        }

        public Entity[] CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(srcEntityManager,
                srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref entities, 0,
                state);

            return entities;
        }

        public Entity[] CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(srcEntityManager,
                srcEntities, srcStartingIndex, srcCount,
                ref entities, 0,
                state);

            return entities;
        }

        public int CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities,
            ref Entity[] destEntities,
            EntityState? state = null)
        {
            CopyEntities(srcEntityManager,
                srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, 0,
                state);

            return srcEntities.Length;
        }

        public int CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities,
            EntityState? state = null)
        {
            CopyEntities(srcEntityManager,
                srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, 0,
                state);

            return srcEntities.Length - srcStartingIndex;
        }

        public void CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities,
            EntityState? state = null) => CopyEntities(srcEntityManager,
                srcEntities, srcStartingIndex, srcCount,
                ref destEntities, 0,
                state);

        public int CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            CopyEntities(srcEntityManager,
                srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, destStartingIndex,
                state);

            return srcEntities.Length;
        }

        public int CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            CopyEntities(srcEntityManager,
                srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, destStartingIndex,
                state);

            return srcEntities.Length - srcStartingIndex;
        }

        public void CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopySameContextException();

            Context.AssertContext();
            srcEntityManager.Context.AssertContext();
            AssertEntities(srcEntities, srcStartingIndex, srcCount);
            AssertResizeEntities(ref destEntities, destStartingIndex, srcCount);

            lock (_lockObj)
            {
                lock (srcEntityManager._lockObj)
                {
                    CheckCapacity(srcCount);

                    for (int i = 0, entityIndex = srcStartingIndex, destEntityIndex = destStartingIndex;
                        i < srcCount;
                        i++, entityIndex++, destEntityIndex++)
                    {
                        var entity = srcEntities[entityIndex];
                        srcEntityManager.AssertHasEntity(entity);

                        destEntities[destEntityIndex] = InternalCopyEntity(srcEntityManager, entity, state);
                    }
                }
            }
        }

        public Entity[] CopyEntities(EntityManager srcEntityManager,
            EntityArcheType archeType,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(srcEntityManager, archeType, ref entities, 0, state);

            return entities;
        }

        public int CopyEntities(EntityManager srcEntityManager,
            EntityArcheType archeType,
            ref Entity[] destEntities,
            EntityState? state = null) => CopyEntities(srcEntityManager, archeType, ref destEntities, 0, state);

        public int CopyEntities(EntityManager srcEntityManager,
            EntityArcheType archeType,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopySameContextException();
            if (srcEntityManager.Context == Context)
                throw new EntityCopySameContextException();

            Context.AssertContext();
            srcEntityManager.Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                lock (srcEntityManager._lockObj)
                {
                    var prevArcheTypeData = srcEntityManager.Context.ArcheTypeManager.GetArcheTypeData(archeType);

                    AssertResizeEntities(ref destEntities, destStartingIndex, prevArcheTypeData.EntityCount);

                    return InternalCopyArcheType(srcEntityManager, prevArcheTypeData,
                        ref destEntities, destStartingIndex, state);
                }
            }
        }

        public Entity[] CopyEntities(EntityManager srcEntityManager,
            EntityFilter filter,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(srcEntityManager, filter, ref entities, 0, state);

            return entities;
        }

        public int CopyEntities(EntityManager srcEntityManager,
            EntityFilter filter,
            ref Entity[] destEntities,
            EntityState? state = null) => CopyEntities(srcEntityManager, filter, ref destEntities, 0, state);

        public int CopyEntities(EntityManager srcEntityManager,
            EntityFilter filter,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopySameContextException();
            if (srcEntityManager.Context == Context)
                throw new EntityCopySameContextException();

            Context.AssertContext();
            srcEntityManager.Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);
            AssertResizeEntities(ref destEntities, destStartingIndex, 1);

            lock (_lockObj)
            {
                lock (srcEntityManager._lockObj)
                {
                    var archeTypeDatas = filter.GetContextData(srcEntityManager.Context).ArcheTypeDatas;
                    var entityIndex = destStartingIndex;
                    for (var i = 0; i < archeTypeDatas.Length; i++)
                    {
                        var prevArcheTypeData = archeTypeDatas[i];
                        ResizeRefEntities(ref destEntities, entityIndex, prevArcheTypeData.EntityCount);

                        entityIndex += InternalCopyArcheType(srcEntityManager, prevArcheTypeData,
                            ref destEntities, entityIndex, state);
                    }

                    return entityIndex - destStartingIndex;
                }
            }
        }

        public Entity[] CopyEntities(EntityTracker tracker,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(tracker, ref entities, 0, state);

            return entities;
        }

        public int CopyEntities(EntityTracker tracker,
            ref Entity[] destEntities,
            EntityState? state = null) => CopyEntities(tracker, ref destEntities, 0, state);

        public int CopyEntities(EntityTracker tracker,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, tracker?.Context);
            tracker.Context.AssertContext();
            AssertResizeEntities(ref destEntities, destStartingIndex, 1);
            if (tracker.Context == Context)
                throw new EntityCopySameContextException();

            var srcEntityManager = tracker.Context.Entities;

            lock (_lockObj)
            {
                lock (srcEntityManager._lockObj)
                {
                    CheckCapacity(tracker.CachedEntities.Length);

                    var entityCount = tracker.CachedEntities.Length;
                    ResizeRefEntities(ref _cachedInternalEntities, 0, entityCount);
                    Array.Copy(tracker.CachedEntities, _cachedInternalEntities, entityCount);

                    ResizeRefEntities(ref destEntities, destStartingIndex, tracker.CachedEntities.Length);

                    for (int i = 0, destEntityIndex = destStartingIndex;
                        i < entityCount;
                        i++, destEntityIndex++)
                    {
                        destEntities[destEntityIndex] = InternalCopyEntity(srcEntityManager,
                            _cachedInternalEntities[i], state);
                    }

                    return tracker.CachedEntities.Length;
                }
            }
        }

        public Entity[] CopyEntities(EntityQuery query,
            EntityState? state = null)
        {
            var entities = new Entity[0];
            CopyEntities(query, ref entities, 0, state);

            return entities;
        }

        public int CopyEntities(EntityQuery query,
            ref Entity[] destEntities,
            EntityState? state = null) => CopyEntities(query, ref destEntities, 0, state);

        public int CopyEntities(EntityQuery query,
            ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, query?.Context);
            query.Context.AssertContext();
            AssertResizeEntities(ref destEntities, destStartingIndex, 1);
            if (query.Context == Context)
                throw new EntityCopySameContextException();

            if (query.Tracker == null)
                return CopyEntities(query.Context.Entities, query.Filter, ref destEntities, destStartingIndex, state);

            var srcEntityManager = query.Context.Entities;

            lock (_lockObj)
            {
                lock (srcEntityManager._lockObj)
                {
                    var archeTypeDatas = query.Filter.GetContextData(srcEntityManager.Context).ArcheTypeDatas;
                    var destEntityIndex = destStartingIndex;
                    for (var i = 0; i < archeTypeDatas.Length; i++)
                    {
                        var archeTypeData = archeTypeDatas[i];
                        var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                            ref _cachedInternalEntities, 0);

                        if (trackedEntityCount > 0)
                        {
                            ResizeRefEntities(ref destEntities, destEntityIndex, trackedEntityCount);

                            for (var j = 0; j < trackedEntityCount; j++)
                            {
                                destEntities[destEntityIndex++] = InternalCopyEntity(
                                    srcEntityManager, _cachedInternalEntities[j], state);
                            }
                        }
                    }

                    return destEntityIndex - destStartingIndex;
                }
            }
        }

        #endregion

        #region ComponentHas

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                return Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex)
                    .ArcheType.HasComponentConfig(ComponentConfig<TComponent>.Config);
            }
        }

        public bool HasSharedComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                return Context.ArcheTypeManager.GetArcheTypeData(_entityDatas[entity.Id].ArcheTypeIndex)
                    .ArcheType.HasComponentConfig(ComponentConfig<TComponent>.Config);
            }
        }

        public bool HasUniqueComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, IUniqueComponent
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                return _uniqueComponentEntities[ComponentConfig<TComponent>.Config.UniqueIndex] == entity;
            }
        }

        public bool HasUniqueComponent<TComponent>()
            where TComponent : unmanaged, IUniqueComponent
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                return _uniqueComponentEntities[ComponentConfig<TComponent>.Config.UniqueIndex] != Entity.Null;
            }
        }

        #endregion

        #region ComponentGetAll

        public IComponent[] GetAllComponents(Entity entity)
        {
            var components = new IComponent[0];
            GetAllComponents(entity, ref components, 0);

            return components;
        }

        public int GetAllComponents(Entity entity,
            ref IComponent[] destComponents) => GetAllComponents(entity, ref destComponents, 0);

        public int GetAllComponents(Entity entity,
            ref IComponent[] destComponents, int destStartingIndex)
        {
            Context.AssertContext();
            if (destComponents == null)
                throw new ArgumentNullException(nameof(destComponents));
            if (destStartingIndex < 0 || destStartingIndex > destComponents.Length)
                throw new ArgumentOutOfRangeException(nameof(destStartingIndex));

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                ResizeRefComponents(ref destComponents, destStartingIndex, archeTypeData.ArcheType.ComponentConfigLength);
                archeTypeData.GetAllEntityComponents(entityData, ref destComponents, destStartingIndex);

                return archeTypeData.ArcheType.ComponentConfigLength;
            }
        }

        #endregion

        #region ComponentGet

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config, archeTypeData);

                return *(TComponent*)archeTypeData.GetComponentPtr(entityData, config);
            }
        }

        public void GetComponents<T1, T2>(Entity entity,
            out T1 component1, out T2 component2)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
            }
        }

        public void GetComponents<T1, T2, T3>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
                component3 = config3.IsShared
                    ? *(T3*)archeTypeData.GetSharedComponentPtr(config3)
                    : *(T3*)archeTypeData.GetComponentPtr(entityData, config3);
            }
        }

        public void GetComponents<T1, T2, T3, T4>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
                component3 = config3.IsShared
                    ? *(T3*)archeTypeData.GetSharedComponentPtr(config3)
                    : *(T3*)archeTypeData.GetComponentPtr(entityData, config3);
                component4 = config4.IsShared
                    ? *(T4*)archeTypeData.GetSharedComponentPtr(config4)
                    : *(T4*)archeTypeData.GetComponentPtr(entityData, config4);
            }
        }

        public void GetComponents<T1, T2, T3, T4, T5>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
                component3 = config3.IsShared
                    ? *(T3*)archeTypeData.GetSharedComponentPtr(config3)
                    : *(T3*)archeTypeData.GetComponentPtr(entityData, config3);
                component4 = config4.IsShared
                    ? *(T4*)archeTypeData.GetSharedComponentPtr(config4)
                    : *(T4*)archeTypeData.GetComponentPtr(entityData, config4);
                component5 = config5.IsShared
                    ? *(T5*)archeTypeData.GetSharedComponentPtr(config5)
                    : *(T5*)archeTypeData.GetComponentPtr(entityData, config5);
            }
        }

        public void GetComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5, out T6 component6)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);
                AssertHasComponent(entity, config6, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
                component3 = config3.IsShared
                    ? *(T3*)archeTypeData.GetSharedComponentPtr(config3)
                    : *(T3*)archeTypeData.GetComponentPtr(entityData, config3);
                component4 = config4.IsShared
                    ? *(T4*)archeTypeData.GetSharedComponentPtr(config4)
                    : *(T4*)archeTypeData.GetComponentPtr(entityData, config4);
                component5 = config5.IsShared
                    ? *(T5*)archeTypeData.GetSharedComponentPtr(config5)
                    : *(T5*)archeTypeData.GetComponentPtr(entityData, config5);
                component6 = config6.IsShared
                    ? *(T6*)archeTypeData.GetSharedComponentPtr(config6)
                    : *(T6*)archeTypeData.GetComponentPtr(entityData, config6);
            }
        }

        public void GetComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5, out T6 component6, out T7 component7)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);
                AssertHasComponent(entity, config6, archeTypeData);
                AssertHasComponent(entity, config7, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
                component3 = config3.IsShared
                    ? *(T3*)archeTypeData.GetSharedComponentPtr(config3)
                    : *(T3*)archeTypeData.GetComponentPtr(entityData, config3);
                component4 = config4.IsShared
                    ? *(T4*)archeTypeData.GetSharedComponentPtr(config4)
                    : *(T4*)archeTypeData.GetComponentPtr(entityData, config4);
                component5 = config5.IsShared
                    ? *(T5*)archeTypeData.GetSharedComponentPtr(config5)
                    : *(T5*)archeTypeData.GetComponentPtr(entityData, config5);
                component6 = config6.IsShared
                    ? *(T6*)archeTypeData.GetSharedComponentPtr(config6)
                    : *(T6*)archeTypeData.GetComponentPtr(entityData, config6);
                component7 = config7.IsShared
                    ? *(T7*)archeTypeData.GetSharedComponentPtr(config7)
                    : *(T7*)archeTypeData.GetComponentPtr(entityData, config7);
            }
        }

        public void GetComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5, out T6 component6, out T7 component7, out T8 component8)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;
            var config8 = ComponentConfig<T8>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7,
                config8);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);
                AssertHasComponent(entity, config6, archeTypeData);
                AssertHasComponent(entity, config7, archeTypeData);
                AssertHasComponent(entity, config8, archeTypeData);

                component1 = config1.IsShared
                    ? *(T1*)archeTypeData.GetSharedComponentPtr(config1)
                    : *(T1*)archeTypeData.GetComponentPtr(entityData, config1);
                component2 = config2.IsShared
                    ? *(T2*)archeTypeData.GetSharedComponentPtr(config2)
                    : *(T2*)archeTypeData.GetComponentPtr(entityData, config2);
                component3 = config3.IsShared
                    ? *(T3*)archeTypeData.GetSharedComponentPtr(config3)
                    : *(T3*)archeTypeData.GetComponentPtr(entityData, config3);
                component4 = config4.IsShared
                    ? *(T4*)archeTypeData.GetSharedComponentPtr(config4)
                    : *(T4*)archeTypeData.GetComponentPtr(entityData, config4);
                component5 = config5.IsShared
                    ? *(T5*)archeTypeData.GetSharedComponentPtr(config5)
                    : *(T5*)archeTypeData.GetComponentPtr(entityData, config5);
                component6 = config6.IsShared
                    ? *(T6*)archeTypeData.GetSharedComponentPtr(config6)
                    : *(T6*)archeTypeData.GetComponentPtr(entityData, config6);
                component7 = config7.IsShared
                    ? *(T7*)archeTypeData.GetSharedComponentPtr(config7)
                    : *(T7*)archeTypeData.GetComponentPtr(entityData, config7);
                component8 = config8.IsShared
                    ? *(T8*)archeTypeData.GetSharedComponentPtr(config8)
                    : *(T8*)archeTypeData.GetComponentPtr(entityData, config8);
            }
        }

        public TComponent[] GetComponents<TComponent>(EntityArcheType archeType) where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(archeType, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityArcheType archeType,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent => GetComponents(archeType, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityArcheType archeType,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);

                AssertHasComponent(archeTypeData, config);
                AssertResizeComponents(ref destComponents, destStartingIndex, archeTypeData.EntityCount);

                archeTypeData.GetComponents(ref destComponents, destStartingIndex, config);

                return archeTypeData.EntityCount;
            }
        }

        public TComponent[] GetComponents<TComponent>(EntityFilter filter) where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(filter, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent => GetComponents(filter, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);
            AssertResizeComponents(ref destComponents, destStartingIndex, 1);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                var destComponentIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];

                    AssertHasComponent(archeTypeData, config);

                    ResizeRefComponents(ref destComponents, destStartingIndex, archeTypeData.EntityCount);

                    archeTypeData.GetComponents(ref destComponents, destComponentIndex, config);

                    destComponentIndex += archeTypeData.EntityCount;
                }

                return destComponentIndex - destStartingIndex;
            }
        }

        public TComponent[] GetComponents<TComponent>(EntityTracker tracker) where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(tracker, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent => GetComponents(tracker, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertResizeComponents(ref destComponents, destStartingIndex, tracker.CachedEntities.Length);

                for (int i = 0, destComponentIndex = destStartingIndex;
                    i < tracker.CachedEntities.Length;
                    i++, destComponentIndex++)
                {
                    var entity = tracker.CachedEntities[i];
                    var entityData = _entityDatas[entity.Id];
                    var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                    AssertHasComponent(archeTypeData, config);

                    destComponents[destComponentIndex] = *(TComponent*)archeTypeData.GetComponentPtr(entityData, config);
                }

                return tracker.CachedEntities.Length;
            }
        }

        public TComponent[] GetComponents<TComponent>(EntityQuery query) where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(query, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent => GetComponents(query, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Tracker == null)
                return GetComponents(query.Filter, ref destComponents, destStartingIndex);

            AssertResizeComponents(ref destComponents, destStartingIndex, 1);
            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                var destComponentIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    var trackedEntityCount = GetTrackedArcheTypeEntities(query.Tracker, archeTypeData,
                        ref _cachedInternalEntities, 0);

                    if (trackedEntityCount > 0)
                    {
                        ResizeRefComponents(ref destComponents, destComponentIndex, trackedEntityCount);

                        for (var j = 0; j < trackedEntityCount; j++)
                        {
                            destComponents[destComponentIndex++] = *(TComponent*)archeTypeData
                                .GetComponentPtr(_entityDatas[_cachedInternalEntities[j].Id], config);
                        }
                    }
                }

                return destComponentIndex - destStartingIndex;
            }
        }

        #endregion

        #region ComponentGetShared

        public TComponent GetSharedComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config, archeTypeData);

                return *(TComponent*)archeTypeData.GetSharedComponentPtr(config);
            }
        }

        public TComponent GetSharedComponent<TComponent>(EntityArcheType archeType)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);

                AssertHasComponent(archeTypeData, config);

                return *(TComponent*)archeTypeData.GetSharedComponentPtr(config);
            }
        }

        public TComponent[] GetSharedComponents<TComponent>(EntityFilter filter) where TComponent : unmanaged, ISharedComponent
        {
            var components = new TComponent[0];
            GetSharedComponents(filter, ref components, 0);

            return components;
        }

        public int GetSharedComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, ISharedComponent => GetSharedComponents(filter, ref destComponents, 0);

        public int GetSharedComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);
            AssertResizeComponents(ref destComponents, destStartingIndex, 1);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                ResizeRefComponents(ref destComponents, destStartingIndex, archeTypeDatas.Length);

                var destComponentIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    if (archeTypeData.ArcheType.HasComponentConfig(config))
                        destComponents[destComponentIndex++] = *(TComponent*)archeTypeData.GetSharedComponentPtr(config);
                }

                return destComponentIndex - destStartingIndex;
            }
        }

        public TComponent[] GetSharedComponents<TComponent>(EntityTracker tracker) where TComponent : unmanaged, ISharedComponent
        {
            var components = new TComponent[0];
            GetSharedComponents(tracker, ref components, 0);

            return components;
        }

        public int GetSharedComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, ISharedComponent => GetSharedComponents(tracker, ref destComponents, 0);

        public int GetSharedComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            AssertResizeComponents(ref destComponents, destStartingIndex, 1);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = tracker.CachedArcheTypeDatas;
                ResizeRefComponents(ref destComponents, destStartingIndex, archeTypeDatas.Length);

                var destComponentIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    if (archeTypeData.ArcheType.HasComponentConfig(config))
                        destComponents[destComponentIndex++] = *(TComponent*)archeTypeData.GetSharedComponentPtr(config);
                }

                return destComponentIndex - destStartingIndex;
            }
        }

        public TComponent[] GetSharedComponents<TComponent>(EntityQuery query) where TComponent : unmanaged, ISharedComponent
        {
            var components = new TComponent[0];
            GetSharedComponents(query, ref components, 0);

            return components;
        }

        public int GetSharedComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, ISharedComponent => GetSharedComponents(query, ref destComponents, 0);

        public int GetSharedComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Tracker == null)
                return GetSharedComponents(query.Filter, ref destComponents, destStartingIndex);

            AssertResizeComponents(ref destComponents, destStartingIndex, 1);
            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                ResizeRefComponents(ref destComponents, destStartingIndex, archeTypeDatas.Length);

                var destComponentIndex = destStartingIndex;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    if (archeTypeData.ArcheType.HasComponentConfig(config))
                        destComponents[destComponentIndex++] = *(TComponent*)archeTypeData.GetSharedComponentPtr(config);
                }

                return destComponentIndex - destStartingIndex;
            }
        }

        #endregion

        #region ComponentGetUnique

        public TComponent GetUniqueComponent<TComponent>(Entity entity) where TComponent : unmanaged, IUniqueComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config, archeTypeData);

                return *(TComponent*)archeTypeData.GetComponentPtr(entityData, config);
            }
        }

        public TComponent GetUniqueComponent<TComponent>() where TComponent : unmanaged, IUniqueComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var entity = _uniqueComponentEntities[config.UniqueIndex];
                if (entity == Entity.Null)
                    throw new EntityUniqueComponentNotExistsException(config.ComponentType);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                return *(TComponent*)archeTypeData.GetComponentPtr(entityData, config);
            }
        }

        #endregion

        #region ComponentUpdate

        public void UpdateComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config, archeTypeData);

                *(TComponent*)archeTypeData.GetComponentPtr(entityData, config) = component;
                Context.Tracking.TrackUpdate(entity, archeTypeData, config);
            }
        }

        public void UpdateComponents<T1, T2>(Entity entity,
            T1 component1,
            T2 component2)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateComponents<T1, T2, T3>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config3, component3);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateComponents<T1, T2, T3, T4>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config3, component3);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config4, component4);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config3, component3);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config4, component4);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config5, component5);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);
                AssertHasComponent(entity, config6, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config3, component3);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config4, component4);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config5, component5);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config6, component6);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);
                AssertHasComponent(entity, config6, archeTypeData);
                AssertHasComponent(entity, config7, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config3, component3);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config4, component4);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config5, component5);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config6, component6);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config7, component7);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7,
            T8 component8)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;
            var config8 = ComponentConfig<T8>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7,
                config8);

            Context.AssertContext();

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config1, archeTypeData);
                AssertHasComponent(entity, config2, archeTypeData);
                AssertHasComponent(entity, config3, archeTypeData);
                AssertHasComponent(entity, config4, archeTypeData);
                AssertHasComponent(entity, config5, archeTypeData);
                AssertHasComponent(entity, config6, archeTypeData);
                AssertHasComponent(entity, config7, archeTypeData);
                AssertHasComponent(entity, config8, archeTypeData);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                var checkArcheType = false;

                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config1, component1);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config2, component2);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config3, component3);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config4, component4);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config5, component5);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config6, component6);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config7, component7);
                checkArcheType |= InternalUpdateComponent(entity, entityData, archeTypeData, config8, component8);

                if (checkArcheType)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        #endregion

        #region ComponentUpdateShared

        public void UpdateSharedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config, archeTypeData);

                Context.Tracking.TrackUpdate(entity, archeTypeData, config);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                if (_cachedArcheType.ReplaceSharedComponentDataIndex(Context.SharedIndexDics.GetDataIndex(component)))
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        public void UpdateSharedComponent<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(archeType);
                AssertHasComponent(archeTypeData, config);

                ResizeRefEntities(ref _cachedInternalEntities, 0, archeTypeData.EntityCount);
                archeTypeData.GetEntities(ref _cachedInternalEntities, 0);

                for (var i = 0; i < archeTypeData.EntityCount; i++)
                    Context.Tracking.TrackUpdate(_cachedInternalEntities[i], archeTypeData, config);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                if (_cachedArcheType.ReplaceSharedComponentDataIndex(Context.SharedIndexDics.GetDataIndex(component)))
                    CheckTransferEntities(archeTypeData);
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = filter.GetContextData(Context).ArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    AssertHasComponent(archeTypeData, config);

                    ResizeRefEntities(ref _cachedInternalEntities, 0, archeTypeData.EntityCount);
                    archeTypeData.GetEntities(ref _cachedInternalEntities, 0);

                    for (var j = 0; j < archeTypeData.EntityCount; j++)
                        Context.Tracking.TrackUpdate(_cachedInternalEntities[j], archeTypeData, config);

                    ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                    if (_cachedArcheType.ReplaceSharedComponentDataIndex(Context.SharedIndexDics.GetDataIndex(component)))
                        CheckTransferEntities(archeTypeData);
                }
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityTracker tracker, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = tracker.CachedArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    AssertHasComponent(archeTypeData, config);

                    ResizeRefEntities(ref _cachedInternalEntities, 0, archeTypeData.EntityCount);
                    archeTypeData.GetEntities(ref _cachedInternalEntities, 0);

                    for (var j = 0; j < archeTypeData.EntityCount; j++)
                        Context.Tracking.TrackUpdate(_cachedInternalEntities[j], archeTypeData, config);

                    ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                    if (_cachedArcheType.ReplaceSharedComponentDataIndex(Context.SharedIndexDics.GetDataIndex(component)))
                        CheckTransferEntities(archeTypeData);
                }
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityQuery query, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Tracker == null)
            {
                UpdateSharedComponents(query.Filter, component);
                return;
            }

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var archeTypeDatas = query.Filter.GetContextData(Context).ArcheTypeDatas;
                for (var i = 0; i < archeTypeDatas.Length; i++)
                {
                    var archeTypeData = archeTypeDatas[i];
                    if (query.Tracker.HasArcheTypeData(archeTypeData))
                    {
                        AssertHasComponent(archeTypeData, config);

                        ResizeRefEntities(ref _cachedInternalEntities, 0, archeTypeData.EntityCount);
                        archeTypeData.GetEntities(ref _cachedInternalEntities, 0);

                        for (var j = 0; j < archeTypeData.EntityCount; j++)
                            Context.Tracking.TrackUpdate(_cachedInternalEntities[j], archeTypeData, config);

                        ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                        if (_cachedArcheType.ReplaceSharedComponentDataIndex(Context.SharedIndexDics.GetDataIndex(component)))
                            CheckTransferEntities(archeTypeData);
                    }
                }
            }
        }

        #endregion

        #region ComponentUpdateUnique

        public void UpdateUniqueComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IUniqueComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                AssertHasComponent(entity, config, archeTypeData);

                *(TComponent*)archeTypeData.GetComponentPtr(entityData, config) = component;
                Context.Tracking.TrackUpdate(entity, archeTypeData, config);
            }
        }

        public void UpdateUniqueComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IUniqueComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                var entity = _uniqueComponentEntities[config.UniqueIndex];
                if (entity == Entity.Null)
                    throw new EntityUniqueComponentNotExistsException(config.ComponentType);

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                *(TComponent*)archeTypeData.GetComponentPtr(entityData, config) = component;
                Context.Tracking.TrackUpdate(entity, archeTypeData, config);
            }
        }

        #endregion

        #region Internal

        internal bool GetForEach(Entity entity, ref ArcheTypeData prevArcheTypeData, ref IComponentAdapter[] componentAdapters)
        {
            lock (_lockObj)
            {
                if (!InternalHasEntity(entity))
                    return false;

                var entityData = _entityDatas[entity.Id];
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                if (prevArcheTypeData != archeTypeData)
                {
                    prevArcheTypeData = archeTypeData;
                    for (var i = 0; i < componentAdapters.Length; i++)
                    {
                        var configOffset = archeTypeData.GetComponentConfigOffset(componentAdapters[i].Config);
                        componentAdapters[i].SetComponentConfigOffset(configOffset);
                        componentAdapters[i].StoreComponent(entityData, archeTypeData);
                    }
                }
                else
                {
                    for (var i = 0; i < componentAdapters.Length; i++)
                        componentAdapters[i].StoreComponent(entityData, archeTypeData);
                }

                return true;
            }
        }

        internal void UpdateForEach(Entity entity, in IComponentAdapter[] writeAdapters)
        {
            lock (_lockObj)
            {
                AssertHasEntity(entity);

                var entityData = _entityDatas[entity.Id];
                var checkArcheTypeChange = false;
                var archeTypeData = Context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);
                for (var i = 0; i < writeAdapters.Length; i++)
                {
                    var adapter = writeAdapters[i];

                    checkArcheTypeChange |= adapter.UpdateComponent(entityData, archeTypeData, ref _cachedArcheType);
                    Context.Tracking.TrackUpdate(entity, archeTypeData, adapter.Config);
                }

                if (checkArcheTypeChange)
                    CheckTransferEntity(entity, archeTypeData);
            }
        }

        internal void InternalDestroy()
        {
            MemoryHelper.Free(_entities);
            _entities = null;
            MemoryHelper.Free(_entityStates);
            _entityStates = null;
            _cachedEntities = null;
            _cachedInternalEntities = null;
            _isCachedEntitiesDirty = true;
            MemoryHelper.Free(_entityDatas);
            _entityDatas = null;
            _nextId = 0;
            _entityLength = 0;
            _reusableEntities = null;
            MemoryHelper.Free(_uniqueComponentEntities);
            _uniqueComponentEntities = null;
            _cachedArcheType.Dispose();
            _cachedArcheType = new ArcheType();

            _entityCount = 0;
        }

        #endregion

        #region Private

        private static void ResizeRefEntities(ref Entity[] entities, int startingIndex, int count)
        {
            if (startingIndex + count > entities.Length)
                Array.Resize(ref entities, startingIndex + count);
        }

        private static void ResizeRefComponents<TComponent>(ref TComponent[] components, int startingIndex, int count)
            where TComponent : IComponent
        {
            if (startingIndex + count > components.Length)
                Array.Resize(ref components, startingIndex + count);
        }

        private static void ResizeRefEntityStates(ref EntityState[] states, int startingIndex, int count)
        {
            if (startingIndex + count > states.Length)
                Array.Resize(ref states, startingIndex + count);
        }

        private bool InternalHasEntity(Entity entity) => entity.Id > 0 &&
                entity.Id < _entityLength &&
                _entities[entity.Id] == entity;

        private bool InternalHasEntity(Entity entity, EntityTracker tracker) => tracker.HasEntity(entity);

        private bool InternalHasEntity(Entity entity, EntityFilter filter)
        {
            var entityData = _entityDatas[entity.Id];
            return Helper.HasArcheTypeData(filter.GetContextData(Context).ArcheTypeDatas,
                entityData.ArcheTypeIndex);
        }

        private bool InternalHasEntity(Entity entity, EntityQuery query)
        {
            if (query.Tracker == null)
                return InternalHasEntity(entity, query.Filter);

            var entityData = _entityDatas[entity.Id];
            return query.Tracker.HasEntity(entity) &&
                Helper.HasArcheTypeData(query.Filter.GetContextData(Context).ArcheTypeDatas,
                    entityData.ArcheTypeIndex);
        }

        private void CheckCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = _entityLength - ((_entityCount + 1) - _reusableEntities.Count);
            if (unusedCount < count)
            {
                var newLength = Helper.NextPow2(_entityLength + count);
                _entities = MemoryHelper.ReallocCopy1(_entities, _entityLength, newLength);
                _entityStates = (EntityState*)MemoryHelper.ReallocCopy1((int*)_entityStates, _entityLength, newLength);
                _entityDatas = MemoryHelper.ReallocCopy1(_entityDatas, _entityLength, newLength);
                _entityLength = newLength;

                Context.Tracking.ResizeTrackers(newLength);
            }
        }

        private Entity AllocateEntity(EntityState state, ArcheTypeData archeTypeData)
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
            _entityStates[entity.Id] = state;
            _isCachedEntitiesDirty = true;
            _entityCount++;
            for (var i = 0; i < archeTypeData.ArcheType.ComponentConfigLength; i++)
                Context.Tracking.TrackAdd(entity, archeTypeData, archeTypeData.ArcheType.ComponentConfigs[i]);

            return entity;
        }

        private void DeallocateEntity(Entity entity, ArcheTypeData archeTypeData)
        {
            // Dont need to clear EntityData or State since Entity.Version will be different
            _entities[entity.Id] = Entity.Null;
            _reusableEntities.Push(entity);
            _isCachedEntitiesDirty = true;
            _entityCount--;
            Context.Tracking.TrackDestroy(entity, archeTypeData);
        }

        private void CreateAndAddArcheType(EntityState state, ArcheTypeData archeTypeData,
            out Entity entity, out EntityData entityData)
        {
            entity = AllocateEntity(state, archeTypeData);
            entityData = archeTypeData.AddEntity(entity, _bookManager);
            _entityDatas[entity.Id] = entityData;
        }

        private void DestroyEntityAndRemoveArcheType(Entity entity, ArcheTypeData archeTypeData)
        {
            ClearUniqueComponents(archeTypeData);
            archeTypeData.RemoveEntity(entity, _entityDatas, _bookManager);
            DeallocateEntity(entity, archeTypeData);
        }

        private void ArcheTypeRemoveAllEntities(ArcheTypeData archeTypeData)
        {
            ClearUniqueComponents(archeTypeData);

            ResizeRefEntities(ref _cachedInternalEntities, 0, archeTypeData.EntityCount);

            var entitiesCount = 0;
            archeTypeData.RemoveAllEntities(
                _entityDatas,
                _bookManager,
                ref _cachedInternalEntities,
                ref entitiesCount);

            for (var i = 0; i < entitiesCount; i++)
                DeallocateEntity(_cachedInternalEntities[i], archeTypeData);
        }

        private int GetTrackedArcheTypeEntities(EntityTracker tracker, ArcheTypeData archeTypeData,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (!tracker.HasArcheTypeData(archeTypeData))
                return 0;

            ResizeRefEntities(ref _cachedInternalEntities, destStartingIndex, archeTypeData.EntityCount);

            var entityIndex = destStartingIndex;
            if (tracker.CachedEntities.Length < archeTypeData.EntityCount)
            {
                for (var i = 0; i < tracker.CachedEntities.Length; i++)
                {
                    var entity = tracker.CachedEntities[i];
                    if (_entityDatas[entity.Id].ArcheTypeIndex == archeTypeData.ArcheTypeIndex)
                        destEntities[entityIndex++] = entity;
                }
            }
            else
            {
                for (var i = 0; i < archeTypeData.EntityCount; i++)
                {
                    var entity = archeTypeData.GetEntity(i);
                    if (entity.Id < tracker.Entities.Length && tracker.Entities[entity.Id] == entity)
                        destEntities[entityIndex++] = entity;
                }
            }

            return entityIndex - destStartingIndex;
        }

        private void InternalChangeArcheTypeState(ArcheTypeData archeTypeData, EntityState state)
        {
            for (var i = 0; i < archeTypeData.EntityCount; i++)
            {
                var entity = archeTypeData.GetEntity(i);
                if (_entityStates[entity.Id] != state)
                {
                    _entityStates[entity.Id] = state;
                    Context.Tracking.TrackStateChange(entity, archeTypeData);
                }
            }
        }

        private Entity InternalDuplicateEntity(EntityData srcEntityData, EntityState state, ArcheTypeData archeTypeData)
        {
            AssertUniqueComponentsThrow(archeTypeData);

            CreateAndAddArcheType(state, archeTypeData, out var dupEntity, out var dupEntityData);

            archeTypeData.CopyComponents(srcEntityData, dupEntityData);

            return dupEntity;
        }

        private int InternalDuplicateArcheType(ArcheTypeData archeTypeData, ref Entity[] destEntities, int destStartingIndex,
            EntityState? state = null)
        {
            var entityCount = archeTypeData.EntityCount;

            if (entityCount > 0)
            {
                CheckCapacity(entityCount);
                archeTypeData.PrecheckEntityAllocation(entityCount, _bookManager);

                ResizeRefEntities(ref _cachedInternalEntities, 0, entityCount);
                archeTypeData.GetEntities(ref _cachedInternalEntities, 0);

                for (int i = 0, destEntityIndex = destStartingIndex;
                    i < entityCount;
                    i++, destEntityIndex++)
                {
                    var entity = _cachedInternalEntities[i];
                    var entityData = _entityDatas[entity.Id];

                    destEntities[destEntityIndex] = InternalDuplicateEntity(entityData, state ?? _entityStates[entity.Id],
                        archeTypeData);
                }
            }

            return entityCount;
        }

        private Entity InternalCopyEntity(EntityManager srcEntityManager, Entity srcEntity, EntityState? state)
        {
            var prevEntityData = srcEntityManager._entityDatas[srcEntity.Id];
            var prevArcheTypeData = srcEntityManager.Context.ArcheTypeManager.GetArcheTypeData(prevEntityData.ArcheTypeIndex);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            for (var i = 0; i < prevArcheTypeData.SharedConfigs.Length; i++)
            {
                var config = prevArcheTypeData.SharedConfigs[i];
                _cachedArcheType.ReplaceSharedComponentDataIndex(
                    new SharedComponentDataIndex
                    {
                        SharedIndex = config.SharedIndex,
                        SharedDataIndex = Context.SharedIndexDics
                            .GetSharedIndexDic(config)
                            .GetOrAdd(prevArcheTypeData.GetSharedComponentPtr(config))
                    });
            }
            var copiedArcheTypeData = Context.ArcheTypeManager.GetArcheTypeData(_cachedArcheType);

            CreateAndAddArcheType(state ?? srcEntityManager._entityStates[srcEntity.Id],
                copiedArcheTypeData, out var copiedEntity, out var copiedEntityData);

            AssertSetUniqueComponents(copiedEntity, copiedArcheTypeData);

            copiedArcheTypeData.CopyComponents(prevEntityData, copiedEntityData);

            return copiedEntity;
        }

        private int InternalCopyArcheType(EntityManager srcEntityManager, ArcheTypeData prevArcheTypeData,
            ref Entity[] destEntities, int destStartingIndex, EntityState? state)
        {
            if (prevArcheTypeData.EntityCount == 0)
                return 0;
            if (prevArcheTypeData.EntityCount == 1)
            {
                destEntities[destStartingIndex] = InternalCopyEntity(srcEntityManager, prevArcheTypeData.GetEntity(0), state);
                return 1;
            }

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            for (var i = 0; i < prevArcheTypeData.SharedConfigs.Length; i++)
            {
                var config = prevArcheTypeData.SharedConfigs[i];
                _cachedArcheType.ReplaceSharedComponentDataIndex(
                    new SharedComponentDataIndex
                    {
                        SharedIndex = config.SharedIndex,
                        SharedDataIndex = Context.SharedIndexDics
                            .GetSharedIndexDic(config)
                            .GetOrAdd(prevArcheTypeData.GetSharedComponentPtr(config))
                    });
            }
            var copiedArcheTypeData = Context.ArcheTypeManager.GetArcheTypeData(_cachedArcheType);

            AssertUniqueComponentsThrow(copiedArcheTypeData);

            CheckCapacity(prevArcheTypeData.EntityCount);
            ResizeRefEntities(ref _cachedInternalEntities, 0, prevArcheTypeData.EntityCount);
            prevArcheTypeData.GetEntities(ref _cachedInternalEntities, 0);

            for (int i = 0, destEntityIndex = destStartingIndex;
                i < prevArcheTypeData.EntityCount;
                i++, destEntityIndex++)
            {
                var prevEntity = _cachedInternalEntities[i];
                var prevEntityData = srcEntityManager._entityDatas[prevEntity.Id];

                CreateAndAddArcheType(state ?? srcEntityManager._entityStates[prevEntity.Id],
                    copiedArcheTypeData, out var copiedEntity, out var copiedEntityData);

                copiedArcheTypeData.CopyComponents(prevEntityData, copiedEntityData);

                destEntities[destEntityIndex] = copiedEntity;
            }

            return prevArcheTypeData.EntityCount;
        }

        private byte* InternalGetComponent(EntityData entityData, ArcheTypeData archeTypeData,
            ComponentConfig config) => InternalGetComponent(entityData, archeTypeData,
                archeTypeData.GetComponentConfigOffset(config));

        private byte* InternalGetComponent(EntityData entityData, ArcheTypeData archeTypeData,
            ComponentConfigOffset configOffset)
        {
            if (configOffset.Config.IsShared)
                return archeTypeData.GetSharedComponentOffsetPtr(configOffset);
            else
                return archeTypeData.GetComponentOffsetPtr(entityData, configOffset);
        }

        private bool InternalUpdateComponent<TComponent>(Entity entity, EntityData entityData, ArcheTypeData archeTypeData,
            ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IComponent => InternalUpdateComponent(entity, entityData, archeTypeData,
                archeTypeData.GetComponentConfigOffset(config), component);

        private bool InternalUpdateComponent<TComponent>(Entity entity, EntityData entityData, ArcheTypeData archeTypeData,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IComponent
        {
            Context.Tracking.TrackUpdate(entity, archeTypeData, configOffset.Config);

            if (configOffset.Config.IsShared)
            {
                return _cachedArcheType.ReplaceSharedComponentDataIndex(
                    Context.SharedIndexDics.GetDataIndex(component));
            }
            else
            {
                *(TComponent*)archeTypeData.GetComponentOffsetPtr(entityData, configOffset) = component;
                return false;
            }
        }

        private void ClearUniqueComponents(ArcheTypeData archeTypeData)
        {
            for (var i = 0; i < archeTypeData.UniqueConfigs.Length; i++)
                _uniqueComponentEntities[archeTypeData.UniqueConfigs[i].UniqueIndex] = Entity.Null;
        }

        private void CheckTransferEntity(Entity entity, ArcheTypeData prevArcheTypeData)
        {
            var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeData(_cachedArcheType);
            if (prevArcheTypeData != nextArcheTypeData)
            {
                ArcheTypeData.TransferEntity(entity,
                    prevArcheTypeData,
                    nextArcheTypeData,
                    _entityDatas,
                    _bookManager);
                Context.Tracking.TrackArcheTypeChange(entity, prevArcheTypeData, nextArcheTypeData);
            }
        }

        private void CheckTransferEntities(ArcheTypeData prevArcheTypeData)
        {
            var nextArcheTypeData = Context.ArcheTypeManager.GetArcheTypeData(_cachedArcheType);
            if (prevArcheTypeData != nextArcheTypeData)
            {
                ArcheTypeData.TransferAllEntities(
                    prevArcheTypeData,
                    nextArcheTypeData,
                    _entityDatas,
                    _bookManager);
            }
        }

        #endregion

        #region Assert

        private static void AssertEntities(in Entity[] entities, int startingIndex, int count)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (startingIndex < 0 || startingIndex > entities.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startingIndex + count < 0 || startingIndex + count >= entities.Length + 1)
                throw new ArgumentOutOfRangeException();
        }

        private static void AssertResizeEntities(ref Entity[] entities, int startingIndex, int count)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (startingIndex < 0 || startingIndex > entities.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefEntities(ref entities, startingIndex, count);
        }

        private static void AssertEntityStates(in EntityState[] states, int startingIndex)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));
            if (startingIndex < 0 || startingIndex > states.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
        }

        private static void AssertResizeEntityStates(ref EntityState[] states, int startingIndex, int count)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));
            if (startingIndex < 0 || startingIndex > states.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefEntityStates(ref states, startingIndex, count);
        }

        private static void AssertComponents<TComponent>(in TComponent[] components, int startingIndex, int count)
            where TComponent : IComponent
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));
            if (startingIndex < 0 || startingIndex > components.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startingIndex + count < 0 || startingIndex + count >= components.Length + 1)
                throw new ArgumentOutOfRangeException();
        }

        private static void AssertResizeComponents<TComponent>(ref TComponent[] components, int startingIndex, int count)
            where TComponent : IComponent
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));
            if (startingIndex < 0 || startingIndex > components.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefComponents(ref components, startingIndex, count);
        }

        private void AssertHasEntity(Entity entity)
        {
            if (!(entity.Id > 0 && entity.Id < _entityLength && _entities[entity.Id] == entity))
                throw new EntityNotExistException(entity);
        }

        private void AssertSetUniqueComponents(Entity entity, ArcheTypeData archeTypeData)
        {
            for (var i = 0; i < archeTypeData.UniqueConfigs.Length; i++)
            {
                var config = archeTypeData.UniqueConfigs[i];
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityUniqueComponentExistsException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        config.ComponentType);
                }
                else
                    _uniqueComponentEntities[config.UniqueIndex] = entity;
            }
        }

        private void AssertUniqueComponentsThrow(ArcheTypeData archeTypeData)
        {
            for (var i = 0; i < archeTypeData.UniqueConfigs.Length; i++)
            {
                var config = archeTypeData.UniqueConfigs[i];
                var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                if (uniqueEntity == Entity.Null)
                    uniqueEntity = AllocateEntity(EntityState.Creating, archeTypeData);

                throw new EntityUniqueComponentExistsException(
                    uniqueEntity,
                    ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
            }
        }

        private void AssertHasComponent(Entity entity, ComponentConfig config, ArcheTypeData archeTypeData)
        {
            if (!archeTypeData.ArcheType.HasComponentConfig(config))
                throw new EntityNotHaveComponentException(entity, config.ComponentType);
        }

        private void AssertHasComponent(ArcheTypeData archeTypeData, ComponentConfig config)
        {
            if (!archeTypeData.ArcheType.HasComponentConfig(config))
                throw new EntityArcheTypeNotHaveComponentException(config.ComponentType);
        }

        #endregion
    }
}
