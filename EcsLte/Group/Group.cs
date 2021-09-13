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

        internal void AttachCollector(SubCollector subCollector)
        {
            lock (_data.SubCollectors)
            {
                _data.SubCollectors.Add(subCollector);
            }
        }

        internal void DetachCollector(SubCollector subCollector)
        {
            lock (_data.SubCollectors)
            {
                _data.SubCollectors.Remove(subCollector);
            }
        }

        internal void OnEntityArrayResize(int newSize)
        {
            lock (_data.Entities)
            {
                if (_data.Entities.UncachedData.Length < newSize)
                    Array.Resize(ref _data.Entities.UncachedData, newSize);
            }
            lock (_data.SubCollectors)
            {
                foreach (var subCollector in _data.SubCollectors)
                    subCollector.OnEntityArrayResize(newSize);
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
                }
            }
            lock (_data.SubCollectors)
            {
                foreach (var subCollector in _data.SubCollectors)
                    subCollector.OnEntityWillBeDestroyed(entity);
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

                        lock (_data.SubCollectors)
                        {
                            foreach (var subCollector in _data.SubCollectors)
                                subCollector.AddedEntity(this, entity);
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

                        lock (_data.SubCollectors)
                        {
                            foreach (var subCollector in _data.SubCollectors)
                                subCollector.RemovedEntity(this, entity);
                        }
                    }
                }
        }

        internal void UpdateEntity(Entity entity, int componentPoolIndex)
        {
            lock (_data.SubCollectors)
            {
                foreach (var collector in _data.SubCollectors)
                    collector.UpdatedEntity(this, entity);
            }
        }

        internal void InternalDestroy()
        {
            foreach (var subCollector in _data.SubCollectors)
                subCollector.InternalDestroy();

            _data.Reset();
            ObjectCache.Push(_data);

            IsDestroyed = true;
        }

        private static Entity[] UpdateEntitiesCache(Dictionary<int, Entity> uncachedData)
        {
            return uncachedData.Values.ToArray();
        }
    }
}