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

        internal Collector() { }

        internal void Initialize(Group group, CollectorTrigger trigger)
        {
            _data = ObjectCache.Pop<CollectorData>();

            if (_data.Entities.UncachedData.Length < group.CurrentWorld.EntityManager.EntityArrayLength)
                Array.Resize(ref _data.Entities.UncachedData, group.CurrentWorld.EntityManager.EntityArrayLength);

            Group = group;
            CurrentWorld = Group.CurrentWorld;
            CollectorTrigger = trigger;

            foreach (var index in trigger.Indexes)
                if (!group.Filter.AllOfIndexes.Contains(index) || group.Filter.AnyOfIndexes.Contains(index))
                    throw new CollectorGroupMissingComponent();
        }

        public Group Group { get; private set; }
        public World CurrentWorld { get; private set; }
        public CollectorTrigger CollectorTrigger { get; private set; }
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
            lock (_data.Entities)
            {
                _data.Entities.UncachedData[entity.Id] = Entity.Null;
                _data.Entities.IsDirty = true;
            }
        }

        internal void AddedEntity(Entity entity, int componentPoolIndex)
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            AppendEntity(entity, componentPoolIndex, CollectorTrigger.AddedIndexes);
        }

        internal void RemovedEntity(Entity entity, int componentPoolIndex)
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            AppendEntity(entity, componentPoolIndex, CollectorTrigger.RemovedIndexes);
        }

        internal void UpdatedEntity(Entity entity, int componentPoolIndex)
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            AppendEntity(entity, componentPoolIndex, CollectorTrigger.ReplacedIndexes);
        }

        internal void InternalDestroy()
        {
            Array.Clear(_data.Entities.UncachedData, 0, _data.Entities.UncachedData.Length);
            _data.Entities.IsDirty = true;

            IsDestroyed = true;
        }

        private void AppendEntity(Entity entity, int componentPoolIndex, int[] indexes)
        {
            if (indexes.Contains(componentPoolIndex))
                lock (_data.Entities)
                {
                    if (_data.Entities.UncachedData[entity.Id] != entity)
                    {
                        _data.Entities.UncachedData[entity.Id] = entity;
                        _data.Entities.IsDirty = true;
                    }
                }
        }
    }
}