using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte
{
    public class EntityTrackerManager
    {
        private readonly Dictionary<string, EntityTracker> _trackers;
        private readonly object _lockObj;

        private HashSet<EntityTracker>[] _addedEvent;
        private HashSet<EntityTracker>[] _updatedEvent;
        private HashSet<EntityTracker>[] _removedEvent;

        public EcsContext Context { get; private set; }

        internal EntityTrackerManager(EcsContext context)
        {
            _trackers = new Dictionary<string, EntityTracker>();
            _lockObj = new object();

            _addedEvent = CreateComponentHashSet();
            _updatedEvent = CreateComponentHashSet();
            _removedEvent = CreateComponentHashSet();

            Context = context;
        }

        public bool HasTracker(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            return _trackers.ContainsKey(name);
        }

        public EntityTracker[] GetAllTracker()
        {
            Context.AssertContext();

            return _trackers.Values.ToArray();
        }

        public EntityTracker GetTracker(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            AssertNotExistTracker(name);

            return _trackers[name];
        }

        public EntityTracker CreateTracker(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            AssertAlreadyHaveTracker(name);

            var tracker = new EntityTracker(Context, name);
            _trackers.Add(name, tracker);

            return tracker;
        }

        public void RemoveTracker(EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            AssertNotExistTracker(tracker.Name);

            tracker.InternalDestroy();
            _trackers.Remove(tracker.Name);
        }

        internal void StartTracker(EntityTracker tracker)
        {
            foreach (var configState in tracker.TrackingConfigs)
            {
                if ((configState.Value & TrackingState.Added) == TrackingState.Added)
                    _addedEvent[configState.Key.ComponentIndex].Add(tracker);
                if ((configState.Value & TrackingState.Updated) == TrackingState.Updated)
                    _updatedEvent[configState.Key.ComponentIndex].Add(tracker);
                if ((configState.Value & TrackingState.Removed) == TrackingState.Removed)
                    _removedEvent[configState.Key.ComponentIndex].Add(tracker);
            }
        }

        internal void StopTracker(EntityTracker tracker)
        {
            foreach (var config in tracker.TrackingConfigs.Keys)
            {
                _addedEvent[config.ComponentIndex].Remove(tracker);
                _updatedEvent[config.ComponentIndex].Remove(tracker);
                _removedEvent[config.ComponentIndex].Remove(tracker);
            }
        }

        internal void TrackerStateChanged(EntityTracker tracker, ComponentConfig config, TrackingState state)
        {
            _addedEvent[config.ComponentIndex].Remove(tracker);
            _updatedEvent[config.ComponentIndex].Remove(tracker);
            _removedEvent[config.ComponentIndex].Remove(tracker);

            if ((state & TrackingState.Added) == TrackingState.Added)
                _addedEvent[config.ComponentIndex].Add(tracker);
            if ((state & TrackingState.Updated) == TrackingState.Updated)
                _updatedEvent[config.ComponentIndex].Add(tracker);
            if ((state & TrackingState.Removed) == TrackingState.Removed)
                _removedEvent[config.ComponentIndex].Add(tracker);
        }

        internal void TrackArcheTypeDataChange(Entity entity,
            ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData)
        {
            foreach (var tracker in _trackers.Values)
                tracker.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
        }

        internal void TrackAllArcheTypeDataChange(
            ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData)
        {
            foreach (var tracker in _trackers.Values)
                tracker.TrackAllArcheTypeDataChange(prevArcheTypeData, nextArcheTypeData);
        }

        internal void TrackAdd(Entity entity, ComponentConfig config, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _addedEvent[config.ComponentIndex])
                tracker.Tracked(entity, archeTypeData);
        }

        internal void TrackAdds(in Entity[] entities, int startingIndex, int count,
            ComponentConfig config, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _addedEvent[config.ComponentIndex])
                tracker.Tracked(entities, startingIndex, count, archeTypeData);
        }

        internal void TrackUpdate(Entity entity, ComponentConfig config, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _updatedEvent[config.ComponentIndex])
                tracker.Tracked(entity, archeTypeData);
        }

        internal void TrackUpdates(in Entity[] entities, int startingIndex, int count,
            ComponentConfig config, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _updatedEvent[config.ComponentIndex])
                tracker.Tracked(entities, startingIndex, count, archeTypeData);
        }

        internal void TrackParallelUpdates(in Entity[] entities, int startingIndex, int count,
            ComponentConfig config, ArcheTypeData archeTypeData)
        {
            lock (_lockObj)
            {
                foreach (var tracker in _updatedEvent[config.ComponentIndex])
                    tracker.Tracked(entities, startingIndex, count, archeTypeData);
            }
        }

        internal void TrackRemove(Entity entity, ComponentConfig config, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _removedEvent[config.ComponentIndex])
                tracker.Tracked(entity, archeTypeData);
        }

        internal void TrackRemoves(in Entity[] entities, int startingIndex, int count,
            ComponentConfig config, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _removedEvent[config.ComponentIndex])
                tracker.Tracked(entities, startingIndex, count, archeTypeData);
        }

        internal void EntityDestroyed(Entity entity, ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _trackers.Values)
                tracker.Untracked(entity, archeTypeData);
        }

        internal void ArcheTypeEntitiesDestroyed(ArcheTypeData archeTypeData)
        {
            foreach (var tracker in _trackers.Values)
                tracker.UntrackedArcheTypeData(archeTypeData);
        }

        internal void AllEntitiesDestroyed()
        {
            foreach (var tracker in _trackers.Values)
                tracker.UntrackedAllEntities();
        }

        internal void InternalDestroy()
        {
            foreach (var tracker in _trackers.Values)
                tracker.InternalDestroy();

            _trackers.Clear();
            _addedEvent = null;
            _updatedEvent = null;
            _removedEvent = null;
        }

        private HashSet<EntityTracker>[] CreateComponentHashSet()
        {
            var sets = new HashSet<EntityTracker>[ComponentConfigs.AllComponentCount];
            for (var i = 0; i < ComponentConfigs.AllComponentCount; i++)
                sets[i] = new HashSet<EntityTracker>();

            return sets;
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
    }
}
