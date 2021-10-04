using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal delegate void WatcherDataEvent(Entity entity);

    internal interface IWatcherData
    {
        bool IsActive { get; }
        Filter Filter { get; }

        void IncRefCount();
        void DecRefCount();

        void SetActive(bool active);
        bool HasEntity(Entity entity);
        Entity[] GetEntities();
        void ClearEntities();
    }

    internal class WatcherTable
    {
        internal static WatcherTable Initialize()
        {
            var data = ObjectCache<WatcherTable>.Pop();

            // data._watchers;
            // data._addedWatchers;
            // data._updatedWatchers;
            // data._removedWatchers;

            return data;
        }

        internal static void Uninitialize(WatcherTable data)
        {
            foreach (var watcher in data._watchers.Values)
            {
                WatcherData.Uninitialize(watcher);
            }
            data._watchers.Clear();
            data._addedWatchers.Clear();
            data._updatedWatchers.Clear();
            data._removedWatchers.Clear();

            ObjectCache<WatcherTable>.Push(data);
        }

        private readonly Dictionary<Filter, WatcherData> _watchers;
        private readonly HashSet<WatcherData> _addedWatchers;
        private readonly HashSet<WatcherData> _updatedWatchers;
        private readonly HashSet<WatcherData> _removedWatchers;

        public WatcherTable()
        {
            _watchers = new Dictionary<Filter, WatcherData>();
            _addedWatchers = new HashSet<WatcherData>();
            _updatedWatchers = new HashSet<WatcherData>();
            _removedWatchers = new HashSet<WatcherData>();
        }

        #region WatcherTable

        internal Watcher Added(EcsContext context, Filter filter)
        {
            var data = CreateOrGetWatcher(filter);

            lock (_addedWatchers)
            {
                _addedWatchers.Add(data);
            }

            return new Watcher(context, data);
        }

        internal Watcher Updated(EcsContext context, Filter filter)
        {
            var data = CreateOrGetWatcher(filter);

            lock (_updatedWatchers)
            {
                _updatedWatchers.Add(data);
            }

            return new Watcher(context, data);
        }

        internal Watcher Removed(EcsContext context, Filter filter)
        {
            var data = CreateOrGetWatcher(filter);

            lock (_removedWatchers)
            {
                _removedWatchers.Add(data);
            }

            return new Watcher(context, data);
        }

        internal Watcher AddedOrUpdated(EcsContext context, Filter filter)
        {
            var data = CreateOrGetWatcher(filter);

            lock (_addedWatchers)
            {
                _addedWatchers.Add(data);
            }

            lock (_updatedWatchers)
            {
                _updatedWatchers.Add(data);
            }

            return new Watcher(context, data);
        }

        internal Watcher AddedOrRemoved(EcsContext context, Filter filter)
        {
            var data = CreateOrGetWatcher(filter);

            lock (_addedWatchers)
            {
                _addedWatchers.Add(data);
            }

            lock (_removedWatchers)
            {
                _removedWatchers.Add(data);
            }

            return new Watcher(context, data);
        }

        private WatcherData CreateOrGetWatcher(Filter filter)
        {
            lock (_watchers)
            {
                if (!_watchers.TryGetValue(filter, out var data))
                {
                    data = WatcherData.Initialize(filter);
                    data.NoRef += RemoveWatcherData;
                    // TODO
                    // lock (_ecsContextData.AllWatchers)
                    // {
                    //     _ecsContextData.AllWatchers.Add(watcher);
                    // }
                    _watchers.Add(filter, data);
                }

                data.SetActive(true);

                return data;
            }
        }

        private void RemoveWatcherData(WatcherData data)
        {
            lock (_watchers)
            {
                _watchers.Remove(data.Filter);
                WatcherData.Uninitialize(data);
            }
        }

        #endregion

        #region WatcherCallback

        internal void AddedEntity(Entity entity)
        {
            lock (_addedWatchers)
            {
                foreach (var watcher in _addedWatchers)
                    watcher.AddEntity(entity);
            }
        }

        internal void UpdatedEntity(Entity entity)
        {
            lock (_updatedWatchers)
            {
                foreach (var watcher in _updatedWatchers)
                    watcher.AddEntity(entity);
            }
        }

        internal void RemovedEntity(Entity entity)
        {
            lock (_removedWatchers)
            {
                foreach (var watcher in _removedWatchers)
                    watcher.AddEntity(entity);
            }
        }

        #endregion

        private class WatcherData : IWatcherData
        {
            internal static WatcherData Initialize(Filter filter)
            {
                var data = ObjectCache<WatcherData>.Pop();
                // data._entities
                // data._refCount

                data.Filter = filter;

                return data;
            }

            internal static void Uninitialize(WatcherData data)
            {
                data._entities.UncachedData.Clear();
                data._entities.SetDirty();
                data._refCount = 0;

                data.NoRef = null;

                ObjectCache<WatcherData>.Push(data);
            }

            private readonly DataCache<Dictionary<int, Entity>, Entity[]> _entities;
            private int _refCount;

            public WatcherData()
            {
                _entities = new DataCache<Dictionary<int, Entity>, Entity[]>(
                    new Dictionary<int, Entity>(), UpdateCachedData);
            }

            public bool IsActive { get; private set; }
            public Filter Filter { get; private set; }

            internal event RefCountZeroEvent<WatcherData> NoRef;

            public void IncRefCount()
            {
                _refCount++;
            }

            public void DecRefCount()
            {
                _refCount--;
                if (_refCount == 0 && NoRef != null)
                    NoRef.Invoke(this);
            }

            public void SetActive(bool active)
            {
                if (IsActive != active)
                {
                    IsActive = active;
                    if (!active)
                        ClearEntities();
                }
            }

            public bool HasEntity(Entity entity)
            {
                return _entities.UncachedData.ContainsKey(entity.Id);
            }

            public Entity[] GetEntities()
            {
                return _entities.CachedData;
            }

            public void ClearEntities()
            {
                lock (_entities)
                {
                    _entities.UncachedData.Clear();
                    _entities.SetDirty();
                }
            }

            internal void AddEntity(Entity entity)
            {
                if (IsActive)
                {
                    lock (_entities)
                    {
                        if (!_entities.UncachedData.ContainsKey(entity.Id))
                        {
                            _entities.UncachedData.Add(entity.Id, entity);
                            _entities.SetDirty();
                        }
                    }
                }
            }

            private static Entity[] UpdateCachedData(Dictionary<int, Entity> unchacedData)
            {
                return unchacedData.Values.ToArray();
            }
        }
    }
}