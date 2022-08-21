using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public enum TrackingState
    {
        Not = 0,

        Added = 1,
        Updated = 2,
        Removed = 4,

        AddedOrUpdated = 3,
        AddedOrRemoved = 5,

        UpdatedOrRemoved = 6,

        AddedOrUpdatedOrRemoved = 7
    }

    public class EntityTracker
    {
        private Dictionary<ArcheTypeData, HashSet<Entity>> _archeTypeDatas;
        private ArcheTypeData[] _cachedArcheTypeDatas;
        private readonly Queue<HashSet<Entity>> _cachedEntities;
        private bool _isCachedArcheTypeDataDirty;

        public EcsContext Context { get; private set; }
        public string Name { get; private set; }
        public bool IsDestroyed { get; private set; }
        public bool IsTracking { get; private set; }
        internal Dictionary<ComponentConfig, TrackingState> TrackingConfigs { get; private set; }
        internal int EntityCount
        {
            get
            {
                var entityCount = 0;
                foreach (var entities in _archeTypeDatas.Values)
                    entityCount += entities.Count;

                return entityCount;
            }
        }
        internal ArcheTypeData[] CachedArcheTypeDatas
        {
            get
            {
                if (_isCachedArcheTypeDataDirty)
                {
                    _cachedArcheTypeDatas = _archeTypeDatas.Keys.ToArray();
                    _isCachedArcheTypeDataDirty = false;
                }

                return _cachedArcheTypeDatas;
            }
        }

        internal EntityTracker(EcsContext context, string name)
        {
            _archeTypeDatas = new Dictionary<ArcheTypeData, HashSet<Entity>>();
            _cachedEntities = new Queue<HashSet<Entity>>();

            Context = context;
            Name = name;
            TrackingConfigs = new Dictionary<ComponentConfig, TrackingState>();
        }

        public TrackingState GetTrackingState<TComponent>()
            where TComponent : IComponent
        {
            Context.AssertContext();
            AssertEntityTracker();

            if (TrackingConfigs.TryGetValue(ComponentConfig<TComponent>.Config, out var state))
                return state;

            return TrackingState.Not;
        }

        public EntityTracker SetTrackingState<TComponent>(TrackingState state)
            where TComponent : IComponent
        {
            Context.AssertContext();
            AssertEntityTracker();

            var config = ComponentConfig<TComponent>.Config;
            if (!TrackingConfigs.ContainsKey(ComponentConfig<TComponent>.Config) &&
                state != TrackingState.Not)
                TrackingConfigs.Add(config, state);
            else if (state == TrackingState.Not)
                TrackingConfigs.Remove(config);
            else
                TrackingConfigs[config] = state;

            if (IsTracking)
            {
                Context.Tracking.TrackerStateChanged(
                    this, config, state);
            }

            return this;
        }

        public EntityTracker ClearAllTrackingStates()
        {
            Context.AssertContext();
            AssertEntityTracker();

            if (IsTracking)
            {
                foreach (var config in TrackingConfigs.Keys)
                {
                    Context.Tracking.TrackerStateChanged(
                        this, config, TrackingState.Not);
                }
            }

            TrackingConfigs.Clear();

            return this;

        }

        public EntityTracker StartTracking()
        {
            Context.AssertContext();
            AssertEntityTracker();

            if (!IsTracking)
            {
                Context.Tracking.StartTracker(this);

                IsTracking = true;
            }

            return this;
        }

        public EntityTracker StopTracking()
        {
            Context.AssertContext();
            AssertEntityTracker();

            if (IsTracking)
            {
                Context.Tracking.StopTracker(this);

                IsTracking = false;
            }

            return this;
        }

        public EntityTracker ClearEntities()
        {
            Context.AssertContext();
            AssertEntityTracker();

            UntrackedAllEntities();

            return this;
        }

        internal bool HasArcheTypeData(ArcheTypeData archeTypeData) => _archeTypeDatas.ContainsKey(archeTypeData);

        internal int GetArcheTypeDataEntityCount(ArcheTypeData archeTypeData)
        {
            if (!_archeTypeDatas.TryGetValue(archeTypeData, out var hashedEntities))
                return 0;

            return hashedEntities.Count;
        }

        internal int GetAllEntities(ref Entity[] entities, int startingIndex)
        {
            var entityIndex = startingIndex;
            foreach (var hashedEntities in _archeTypeDatas.Values)
            {
                Helper.ResizeRefArray(ref entities, entityIndex, hashedEntities.Count);
                hashedEntities.CopyTo(entities, entityIndex);
                entityIndex += hashedEntities.Count;
            }

            return entityIndex - startingIndex;
        }

        internal int GetArcheTypeDataEntities(ArcheTypeData archeTypeData, ref Entity[] entities, int startingIndex)
        {
            if (!_archeTypeDatas.TryGetValue(archeTypeData, out var hashedEntities))
                return 0;

            Helper.ResizeRefArray(ref entities, startingIndex, hashedEntities.Count);
            hashedEntities.CopyTo(entities, startingIndex);

            return hashedEntities.Count;
        }

        internal void TrackArcheTypeDataChange(Entity entity,
            ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData)
        {
#if DEBUG
            if (entity == Entity.Null)
                throw new Exception();
#endif
            if (_archeTypeDatas.TryGetValue(prevArcheTypeData, out var hashedEntities))
            {
                if (hashedEntities.Remove(entity))
                {
                    if (hashedEntities.Count == 0)
                        RemoveArcheType(prevArcheTypeData, hashedEntities);

                    AppendEntity(nextArcheTypeData, entity);
                }
            }
        }

        internal void TrackArcheTypeDataChanges(in Entity[] entities, int startingIndex, int count,
            ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData)
        {
            if (_archeTypeDatas.TryGetValue(prevArcheTypeData, out var hashedEntities))
            {
                for (var i = 0; i < count; i++)
                    hashedEntities.Remove(entities[i]);

                if (hashedEntities.Count == 0)
                    RemoveArcheType(prevArcheTypeData, hashedEntities);

                AppendEntities(nextArcheTypeData, entities.Skip(startingIndex).Take(count));
            }
        }

        internal void TrackAllArcheTypeDataChange(ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData)
        {
            if (_archeTypeDatas.TryGetValue(prevArcheTypeData, out var prevEntities))
            {
                if (!_archeTypeDatas.TryGetValue(nextArcheTypeData, out var nextEntities))
                    _archeTypeDatas.Add(nextArcheTypeData, prevEntities);
                else
                {
                    nextEntities.UnionWith(prevEntities);
                    prevEntities.Clear();
                    _cachedEntities.Enqueue(prevEntities);
                }

                _archeTypeDatas.Remove(prevArcheTypeData);
            }
        }

        internal void Tracked(Entity entity, ArcheTypeData archeTypeData) => AppendEntity(archeTypeData, entity);

        internal void Tracked(in Entity[] entities, int startingIndex, int count, ArcheTypeData archeTypeData) => AppendEntities(archeTypeData, entities.Skip(startingIndex).Take(count));

        internal void Untracked(Entity entity, ArcheTypeData archeTypeData)
        {
            if (_archeTypeDatas.TryGetValue(archeTypeData, out var hashedEntities))
            {
                if (hashedEntities.Remove(entity))
                {
                    if (hashedEntities.Count == 0)
                        RemoveArcheType(archeTypeData, hashedEntities);
                }
            }
        }

        internal void UntrackedArcheTypeData(ArcheTypeData archeTypeData)
        {
            if (_archeTypeDatas.TryGetValue(archeTypeData, out var hashedEntities))
                RemoveArcheType(archeTypeData, hashedEntities);
        }

        internal void UntrackedAllEntities()
        {
            foreach (var hashedEntities in _archeTypeDatas.Values)
            {
                hashedEntities.Clear();
                _cachedEntities.Enqueue(hashedEntities);
            }
            _archeTypeDatas.Clear();
            _isCachedArcheTypeDataDirty = true;
        }

        internal void InternalDestroy()
        {
            StopTracking();
            _archeTypeDatas = null;
            _cachedArcheTypeDatas = null;
            _isCachedArcheTypeDataDirty = true;
            IsDestroyed = true;
        }

        private void AppendEntity(ArcheTypeData archeTypeData, Entity entity)
        {
#if DEBUG
            if (entity == Entity.Null)
                throw new Exception();
#endif
            if (!_archeTypeDatas.TryGetValue(archeTypeData, out var hashedEntities))
            {
                hashedEntities = _cachedEntities.Count > 0
                    ? _cachedEntities.Dequeue()
                    : new HashSet<Entity>();
                hashedEntities.Add(entity);

                _archeTypeDatas.Add(archeTypeData, hashedEntities);
                _isCachedArcheTypeDataDirty = true;
            }
            else
                hashedEntities.Add(entity);
        }

        private void AppendEntities(ArcheTypeData archeTypeData, IEnumerable<Entity> entities)
        {
#if DEBUG
            if (entities.Any(x => x == Entity.Null))
                throw new Exception();
#endif
            if (!_archeTypeDatas.TryGetValue(archeTypeData, out var hashedEntities))
            {
                hashedEntities = _cachedEntities.Count > 0
                    ? _cachedEntities.Dequeue()
                    : new HashSet<Entity>();
                hashedEntities.UnionWith(entities);

                _archeTypeDatas.Add(archeTypeData, hashedEntities);
                _isCachedArcheTypeDataDirty = true;
            }
            else
                hashedEntities.UnionWith(entities);
        }

        private void RemoveArcheType(ArcheTypeData archeTypeData, HashSet<Entity> entities)
        {
            entities.Clear();
            _cachedEntities.Enqueue(entities);
            _archeTypeDatas.Remove(archeTypeData);
            _isCachedArcheTypeDataDirty = true;
        }

        #region Assert

        internal static void AssertEntityTracker(EntityTracker tracker, EcsContext context)
        {
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));
            if (tracker.IsDestroyed)
                throw new EntityTrackerIsDestroyedException(tracker);
            if (tracker.Context != context)
                throw new EcsContextNotSameException(tracker.Context, context);
        }

        private void AssertEntityTracker()
        {
            if (IsDestroyed)
                throw new EntityTrackerIsDestroyedException(this);
        }

        #endregion
    }
}
