using System.Collections.Generic;

namespace EcsLte
{
    internal class WatcherTable
    {
        private EcsContext _context;
        private Dictionary<Filter, Watcher> _watchers;
        private HashSet<Watcher> _added;
        private HashSet<Watcher> _updated;
        private HashSet<Watcher> _removed;

        public WatcherTable()
        {
            _watchers = new Dictionary<Filter, Watcher>();
            _added = new HashSet<Watcher>();
            _updated = new HashSet<Watcher>();
            _removed = new HashSet<Watcher>();
        }

        #region WatcherTable

        internal Watcher Added(Filter filter)
        {
            var watcher = CreateOrGetWatcher(filter);
            lock (_added)
            {
                _added.Add(watcher);
            }

            return watcher;
        }

        internal Watcher Updated(Filter filter)
        {
            var watcher = CreateOrGetWatcher(filter);
            lock (_updated)
            {
                _updated.Add(watcher);
            }

            return watcher;
        }

        internal Watcher Removed(Filter filter)
        {
            var watcher = CreateOrGetWatcher(filter);
            lock (_removed)
            {
                _removed.Add(watcher);
            }

            return watcher;
        }

        internal Watcher AddedOrUpdated(Filter filter)
        {
            var watcher = CreateOrGetWatcher(filter);
            lock (_added)
            {
                _added.Add(watcher);
            }
            lock (_updated)
            {
                _updated.Add(watcher);
            }

            return watcher;
        }

        internal Watcher AddedOrRemoved(Filter filter)
        {
            var watcher = CreateOrGetWatcher(filter);
            lock (_added)
            {
                _added.Add(watcher);
            }
            lock (_removed)
            {
                _removed.Add(watcher);
            }

            return watcher;
        }

        private Watcher CreateOrGetWatcher(Filter filter)
        {
            lock (_watchers)
            {
                if (!_watchers.TryGetValue(filter, out var watcher))
                {
                    watcher = new Watcher(_context);
                    _watchers.Add(filter, watcher);
                }
                watcher.Activate();

                return watcher;
            }
        }

        #endregion

        #region WatcherCallback

        internal void AddedEntity(Entity entity)
        {
            lock (_added)
            {
                foreach (var watcher in _added)
                    watcher.AddedEntity(entity);
            }
        }

        internal void UpdatedEntity(Entity entity)
        {
            lock (_updated)
            {
                foreach (var watcher in _updated)
                    watcher.AddedEntity(entity);
            }
        }

        internal void RemovedEntity(Entity entity)
        {
            lock (_removed)
            {
                foreach (var watcher in _removed)
                    watcher.AddedEntity(entity);
            }
        }

        #endregion

        #region ObjectCache

        internal void Initialize(EcsContext context)
        {
            _context = context;
        }

        internal void Reset()
        {
            foreach (var watcher in _watchers.Values)
                watcher.InternalDestroy();
            _watchers.Clear();
            _added.Clear();
            _updated.Clear();
            _removed.Clear();
        }

        #endregion
    }
}