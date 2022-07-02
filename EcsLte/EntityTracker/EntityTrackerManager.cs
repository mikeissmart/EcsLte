using System;
using System.Collections.Generic;

namespace EcsLte
{
    internal delegate void TrackerChange(Entity entity);

    internal class EntityTrackerManager
    {
        private readonly EcsContext _context;
        private readonly TrackEvents[] _events;
        private readonly TrackEvents _destroyEvent;
        private readonly List<EntityTracker> _trackers;
        private readonly Queue<int> _unusedTrackerIndexes;
        private readonly object _lockObj;

        internal EntityTrackerManager(EcsContext context)
        {
            _context = context;
            _events = new TrackEvents[ComponentConfigs.Instance.AllComponentCount];
            _destroyEvent = new TrackEvents();
            _trackers = new List<EntityTracker>();
            _unusedTrackerIndexes = new Queue<int>();
            _lockObj = new object();

            for (var i = 0; i < _events.Length; i++)
                _events[i] = new TrackEvents();
        }

        internal void TrackAdd(Entity entity, ComponentConfig config) => _events[config.ComponentIndex].ComponentAddedInvoke(entity);

        internal void TrackUpdate(Entity entity, ComponentConfig config) => _events[config.ComponentIndex].ComponentUpdatedInvoke(entity);

        internal void TrackDestroy(Entity entity) => _destroyEvent.EntityDestroyedInvoke(entity);

        internal void ResizeTrackers(int entityCapacity)
        {
            lock (_lockObj)
            {
                foreach (var tracker in _trackers)
                {
                    if (tracker != null && tracker.IsTracking)
                        tracker.ResizeTracker(entityCapacity);
                }
            }
        }

        internal void RegisterTracker(EntityTracker tracker)
        {
            lock (_lockObj)
            {
                if (_unusedTrackerIndexes.Count > 0)
                {
                    tracker.TrackerIndex = _unusedTrackerIndexes.Dequeue();
                    _trackers[tracker.TrackerIndex] = tracker;
                }
                else
                {
                    tracker.TrackerIndex = _trackers.Count;
                    _trackers.Add(tracker);
                }
            }
        }

        internal void UnregisterTracker(EntityTracker tracker)
        {
            lock (_lockObj)
            {
                _trackers.RemoveAt(tracker.TrackerIndex);
                _unusedTrackerIndexes.Enqueue(tracker.TrackerIndex);
            }
        }

        internal void RegisterComponentEvent(EntityTracker tracker, int trackingState, ComponentConfig config)
        {
            var events = _events[config.ComponentIndex];

            events.ComponentAdded -= tracker.AddTracked;
            events.ComponentUpdated -= tracker.UpdateTracked;
            switch (trackingState)
            {
                case (int)EntityTracker.TrackingState.Added:
                    events.ComponentAdded += tracker.AddTracked;
                    break;
                case (int)EntityTracker.TrackingState.Updated:
                    events.ComponentUpdated += tracker.UpdateTracked;
                    break;
                case (int)EntityTracker.TrackingState.AddedOrUpdated:
                    events.ComponentAdded += tracker.AddTracked;
                    events.ComponentUpdated += tracker.UpdateTracked;
                    break;
            }
        }

        internal void StartTraking(EntityTracker tracker) => _destroyEvent.EntityDestroyed += tracker.RemoveTracked;

        internal void StopTracking(EntityTracker tracker) => _destroyEvent.EntityDestroyed -= tracker.RemoveTracked;

        internal void InternalDestroy()
        {
            Array.Clear(_events, 0, _events.Length);
            _trackers.Clear();
            _unusedTrackerIndexes.Clear();
        }

        private class TrackEvents
        {
            internal event TrackerChange ComponentAdded;
            internal event TrackerChange ComponentUpdated;
            internal event TrackerChange EntityDestroyed;

            internal void ComponentAddedInvoke(Entity entity) => ComponentAdded?.Invoke(entity);

            internal void ComponentUpdatedInvoke(Entity entity) => ComponentUpdated?.Invoke(entity);

            internal void EntityDestroyedInvoke(Entity entity) => EntityDestroyed?.Invoke(entity);
        }
    }
}
