using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class Group
    {
        private readonly List<Collector>[] _collectorCommponentIndexes;
        private readonly Dictionary<CollectorTrigger, Collector> _collectors;
        private readonly DataCache<Dictionary<int, Entity>, Entity[]> _entities;

        internal Group(GroupManager groupManager, Filter filter)
        {
            _entities = new DataCache<Dictionary<int, Entity>, Entity[]>(new Dictionary<int, Entity>(),
                UpdateEntitiesCache);
            _collectors = new Dictionary<CollectorTrigger, Collector>();
            _collectorCommponentIndexes = new List<Collector>[ComponentIndexes.Instance.Count];

            CurrentWorld = groupManager.CurrentWorld;
            Filter = filter;

            for (var i = 0; i < ComponentIndexes.Instance.Count; i++)
                _collectorCommponentIndexes[i] = new List<Collector>();
        }

        public World CurrentWorld { get; }
        public Filter Filter { get; }
        public bool IsDestroyed { get; private set; }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);

            return _entities.CachedData;
        }

        public bool Equals(Group other)
        {
            if (other == null)
                return false;
            return Filter == other.Filter && CurrentWorld == other.CurrentWorld;
        }

        public override string ToString()
        {
            return Filter.ToString();
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            lock (_entities)
            {
                if (_entities.UncachedData.ContainsKey(entity.Id))
                {
                    _entities.UncachedData.Remove(entity.Id);
                    _entities.IsDirty = true;

                    lock (_collectors)
                    {
                        foreach (var collector in _collectors.Values)
                            collector.OnEntityWillBeDestroyed(entity);
                    }
                }
            }
        }

        internal void FilterEntity(Entity entity, int componentPoolIndex)
        {
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);

            if (CurrentWorld.EntityManager.EntityIsFiltered(entity, Filter))
                lock (_entities)
                {
                    if (!_entities.UncachedData.ContainsKey(entity.Id))
                    {
                        _entities.UncachedData.Add(entity.Id, entity);
                        _entities.IsDirty = true;

                        if (componentPoolIndex != -1)
                            lock (_collectorCommponentIndexes)
                            {
                                foreach (var collector in _collectorCommponentIndexes[componentPoolIndex])
                                    collector.AddedEntity(entity, componentPoolIndex);
                            }
                    }
                }
            else
                lock (_entities)
                {
                    if (_entities.UncachedData.ContainsKey(entity.Id))
                    {
                        _entities.UncachedData.Remove(entity.Id);
                        _entities.IsDirty = true;

                        if (componentPoolIndex != -1)
                            lock (_collectorCommponentIndexes)
                            {
                                foreach (var collector in _collectorCommponentIndexes[componentPoolIndex])
                                    collector.RemovedEntity(entity, componentPoolIndex);
                            }
                    }
                }
        }

        internal void UpdateEntity(Entity entity, int componentPoolIndex)
        {
            foreach (var collector in _collectorCommponentIndexes[componentPoolIndex])
                collector.UpdatedEntity(entity, componentPoolIndex);
        }

        internal void InternalDestroy()
        {
            _entities.UncachedData.Clear();
            _entities.IsDirty = true;
            foreach (var collector in _collectors.Values)
                collector.InternalDestroy();
            _collectors.Clear();

            IsDestroyed = true;
        }

        private Entity[] UpdateEntitiesCache()
        {
            lock (_entities)
            {
                return _entities.UncachedData.Values.ToArray();
            }
        }

        #region CollectorLife

        public Collector GetCollector(CollectorTrigger trigger)
        {
            Collector collector;
            lock (_collectors)
            {
                if (!_collectors.TryGetValue(trigger, out collector))
                {
                    collector = new Collector(this, trigger);
                    _collectors.Add(trigger, collector);

                    lock (_collectorCommponentIndexes)
                    {
                        foreach (var index in trigger.Indexes)
                            _collectorCommponentIndexes[index].Add(collector);
                    }
                }
            }

            return collector;
        }

        public void RemoveCollector(Collector collector)
        {
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);
            if (collector == null)
                throw new ArgumentNullException();
            if (collector.IsDestroyed)
                throw new CollectorIsDestroyedException(collector);

            collector.InternalDestroy();

            lock (_collectors)
            {
                _collectors.Remove(collector.CollectorTrigger);
            }

            foreach (var componentIndex in collector.CollectorTrigger.Indexes)
            {
                var collectors = _collectorCommponentIndexes[componentIndex];
                lock (collectors)
                {
                    collectors.Remove(collector);
                }
            }
        }

        #endregion
    }
}