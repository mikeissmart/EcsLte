using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var sets = new HashSet<EntityTracker>[ComponentConfigs.Instance.AllComponentCount];
            for (var i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
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
