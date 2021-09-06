using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class Group
    {
        private GroupData _data;

        internal Group() { }

        internal void Initialize(GroupManager groupManager, Filter filter, Entity[] entities)
        {
            _data = ObjectCache.Pop<GroupData>();

            if (_data.Entities.UncachedData.Length < groupManager.CurrentWorld.EntityManager.EntityArrayLength)
                Array.Resize(ref _data.Entities.UncachedData, groupManager.CurrentWorld.EntityManager.EntityArrayLength);

            foreach (var entity in entities)
                _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;

            CurrentWorld = groupManager.CurrentWorld;
            Filter = filter;
        }

        public World CurrentWorld { get; private set; }
        public Filter Filter { get; private set; }
        public bool IsDestroyed { get; private set; }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);

            return _data.Entities.CachedData;
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

        internal void OnEntityArrayResize(int newSize)
        {
            lock (_data.Entities)
            {
                if (_data.Entities.UncachedData.Length < newSize)
                    Array.Resize(ref _data.Entities.UncachedData, newSize);
            }
            lock (_data.Collectors)
            {
                foreach (var collector in _data.Collectors.Values)
                    collector.OnEntityArrayResize(newSize);
            }
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            lock (_data.Entities)
            {
                if (_data.Entities.UncachedData[entity.Id] == entity)
                {
                    _data.Entities.UncachedData[entity.Id] = Entity.Null;
                    _data.Entities.IsDirty = true;

                    lock (_data.Collectors)
                    {
                        foreach (var collector in _data.Collectors.Values)
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
                lock (_data.Entities)
                {
                    if (_data.Entities.UncachedData[entity.Id] != entity)
                    {
                        _data.Entities.UncachedData[entity.Id] = entity;
                        _data.Entities.IsDirty = true;

                        if (componentPoolIndex != -1)
                            lock (_data.CollectorCommponentIndexes)
                            {
                                foreach (var collector in _data.CollectorCommponentIndexes[componentPoolIndex])
                                    collector.AddedEntity(entity, componentPoolIndex);
                            }
                    }
                }
            else
                lock (_data.Entities)
                {
                    if (_data.Entities.UncachedData[entity.Id] == entity)
                    {
                        _data.Entities.UncachedData[entity.Id] = Entity.Null;
                        _data.Entities.IsDirty = true;

                        if (componentPoolIndex != -1)
                            lock (_data.CollectorCommponentIndexes)
                            {
                                foreach (var collector in _data.CollectorCommponentIndexes[componentPoolIndex])
                                    collector.RemovedEntity(entity, componentPoolIndex);
                            }
                    }
                }
        }

        internal void UpdateEntity(Entity entity, int componentPoolIndex)
        {
            foreach (var collector in _data.CollectorCommponentIndexes[componentPoolIndex])
                collector.UpdatedEntity(entity, componentPoolIndex);
        }

        internal void InternalDestroy()
        {
            foreach (var collector in _data.Collectors.Values)
                collector.InternalDestroy();

            _data.Reset();
            ObjectCache.Push(_data);

            IsDestroyed = true;
        }

        private static Entity[] UpdateEntitiesCache(Dictionary<int, Entity> uncachedData)
        {
            return uncachedData.Values.ToArray();
        }

        #region CollectorLife

        public Collector GetCollector(CollectorTrigger trigger)
        {
            Collector collector;
            lock (_data.Collectors)
            {
                if (!_data.Collectors.TryGetValue(trigger, out collector))
                {
                    collector = new Collector();
                    collector.Initialize(this, trigger);
                    _data.Collectors.Add(trigger, collector);

                    lock (_data.CollectorCommponentIndexes)
                    {
                        foreach (var index in trigger.Indexes)
                            _data.CollectorCommponentIndexes[index].Add(collector);
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

            lock (_data.Collectors)
            {
                _data.Collectors.Remove(collector.CollectorTrigger);
            }

            foreach (var componentIndex in collector.CollectorTrigger.Indexes)
            {
                var collectors = _data.CollectorCommponentIndexes[componentIndex];
                lock (collectors)
                {
                    collectors.Remove(collector);
                }
            }
        }

        #endregion
    }
}