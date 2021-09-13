using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class Collector
    {
        private CollectorData _data;

        internal Collector(World world)
        {
            CurrentWorld = world;
        }

        public World CurrentWorld { get; private set; }
        public bool IsDestroyed { get; private set; }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            return _data.Entities.CachedData;
        }

        public void ClearEntities()
        {
            lock (_data.Entities)
            {
                Array.Clear(_data.Entities.UncachedData, 0, _data.Entities.UncachedData.Length);
                _data.Entities.IsDirty = true;
            }
        }

        internal void Initialize(List<SubCollector> subCollectors)
        {
            _data = ObjectCache.Pop<CollectorData>();
            _data.SubCollectors = subCollectors;

            foreach (var subCollector in subCollectors)
                subCollector.AddCollector(this);
        }

        internal void OnEntityArrayResize(int newSize)
        {
            lock (_data.Entities)
            {
                if (_data.Entities.UncachedData.Length < newSize)
                    Array.Resize(ref _data.Entities.UncachedData, newSize);
            }
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            _data.Entities.UncachedData[entity.Id] = Entity.Null;
            _data.Entities.IsDirty = true;
        }

        internal void AddedEntity(Group group, Entity entity)
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;
        }

        internal void RemovedEntity(Group group, Entity entity)
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;
        }

        internal void UpdatedEntity(Group group, Entity entity)
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;
        }

        internal void InternalDestroy()
        {
            foreach (var subCollector in _data.SubCollectors)
                subCollector.RemoveCollector(this);

            _data.Reset();
            ObjectCache.Push(_data);

            IsDestroyed = true;
        }
    }
}