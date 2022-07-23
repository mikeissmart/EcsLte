using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    internal delegate void TrackerChange(Entity entity, ArcheTypeData archeTypeData);
    internal delegate void TrackerArcheTypeChange(Entity entity, ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData);
    internal delegate void TrackerResize(int entityCapacity);

    public class EntityTrackerManager
    {
        private readonly Dictionary<string, EntityTracker> _trackers;
        private readonly TrackEvents[] _events;
        private readonly object _lockObj;

        private event TrackerChange _stateChangeEvent;
        private event TrackerChange _destroyEvent;
        private event TrackerArcheTypeChange _archeTypeChange;
        private event TrackerResize _resizeEvent;

        public EcsContext Context { get; private set; }

        internal EntityTrackerManager(EcsContext context)
        {
            _trackers = new Dictionary<string, EntityTracker>();
            _events = new TrackEvents[ComponentConfigs.Instance.AllComponentCount];
            _lockObj = new object();

            Context = context;

            for (var i = 0; i < _events.Length; i++)
                _events[i] = new TrackEvents();
        }

        public bool HasTracker(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            lock (_lockObj)
            {
                return _trackers.ContainsKey(name);
            }
        }

        public EntityTracker[] GetAllTrackers()
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                return _trackers.Values.ToArray();
            }
        }

        public EntityTracker GetTracker(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertNotExistTracker(name);

                return _trackers[name];
            }
        }

        public EntityTracker CreateTracker(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertAlreadyHaveTracker(name);

                var tracker = new EntityTracker(Context, name);
                _trackers.Add(name, tracker);

                return tracker;
            }
        }

        public void RemoveTracker(EntityTracker tracker)
        {
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));
            if (tracker.Context != Context)
                throw new EcsContextDifferentException(Context, tracker.Context);
            Context.AssertContext();

            lock (_lockObj)
            {
                tracker.AssertTracker();
                AssertNotExistTracker(tracker.Name);

                tracker.InternalDestroy();
                _trackers.Remove(tracker.Name);
            }
        }

        internal void TrackStateChange(Entity entity, ArcheTypeData archeTypeData) => _stateChangeEvent?.Invoke(entity, archeTypeData);

        internal void TrackAdd(Entity entity, ArcheTypeData archeTypeData, ComponentConfig config) =>
            _events[config.ComponentIndex].ComponentAddedInvoke(entity, archeTypeData);

        internal void TrackUpdate(Entity entity, ArcheTypeData archeTypeData, ComponentConfig config) =>
            _events[config.ComponentIndex].ComponentUpdatedInvoke(entity, archeTypeData);

        internal void TrackArcheTypeChange(Entity entity, ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData) =>
            _archeTypeChange?.Invoke(entity, prevArcheTypeData, nextArcheTypeData);

        internal void TrackDestroy(Entity entity, ArcheTypeData archeTypeData) => _destroyEvent?.Invoke(entity, archeTypeData);

        internal void ResizeTrackers(int entityCapacity) => _resizeEvent?.Invoke(entityCapacity);

        internal void RegisterComponentEvent(EntityTracker tracker, int trackingState, ComponentConfig config)
        {
            var events = _events[config.ComponentIndex];

            events.ComponentAdded -= tracker.AddTracked;
            events.ComponentUpdated -= tracker.UpdateTracked;
            switch (trackingState)
            {
                case (int)EntityTrackerState.Added:
                    events.ComponentAdded += tracker.AddTracked;
                    break;
                case (int)EntityTrackerState.Updated:
                    events.ComponentUpdated += tracker.UpdateTracked;
                    break;
                case (int)EntityTrackerState.AddedOrUpdated:
                    events.ComponentAdded += tracker.AddTracked;
                    events.ComponentUpdated += tracker.UpdateTracked;
                    break;
            }
        }

        internal void RegisterEntityStateEvent(EntityTracker tracker, bool tracking)
        {
            _stateChangeEvent -= tracker.EntityStateTracked;
            if (tracking)
                _stateChangeEvent += tracker.EntityStateTracked;
        }

        internal void StartTraking(EntityTracker tracker)
        {
            _destroyEvent += tracker.RemoveTracked;
            _resizeEvent += tracker.ResizeTracked;
            _archeTypeChange += tracker.ArcheTypeChangeTracked;

            tracker.ResizeTracked(Context.Entities.EntityCapacity);
        }

        internal void StopTracking(EntityTracker tracker)
        {
            _destroyEvent -= tracker.RemoveTracked;
            _resizeEvent -= tracker.ResizeTracked;
            _archeTypeChange -= tracker.ArcheTypeChangeTracked;
        }

        internal void InternalDestroy()
        {
            foreach (var tracker in _trackers.Values)
                tracker.InternalDestroy();
            _trackers.Clear();
            Array.Clear(_events, 0, _events.Length);

            _stateChangeEvent = null;
            _destroyEvent = null;
            _resizeEvent = null;
            _archeTypeChange = null;
        }

        private void AssertNotExistTracker(string name)
        {
            if (!_trackers.ContainsKey(name))
                throw new EntityTrackerNotExistException(name);
        }

        private void AssertAlreadyHaveTracker(string name)
        {
            if (_trackers.ContainsKey(name))
                throw new EntityTrackerAlreadyExistException(name);
        }

        private class TrackEvents
        {
            internal event TrackerChange ComponentAdded;
            internal event TrackerChange ComponentUpdated;

            internal void ComponentAddedInvoke(Entity entity, ArcheTypeData archeTypeData) => ComponentAdded?.Invoke(entity, archeTypeData);

            internal void ComponentUpdatedInvoke(Entity entity, ArcheTypeData archeTypeData) => ComponentUpdated?.Invoke(entity, archeTypeData);
        }
    }
}
