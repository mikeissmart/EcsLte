using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class Collector
    {
        private readonly DataCache<Dictionary<int, Entity>, Entity[]> _entities;

        internal Collector(Group group, CollectorTrigger trigger)
        {
            _entities = new DataCache<Dictionary<int, Entity>, Entity[]>(new Dictionary<int, Entity>(),
                UpdateEntitiesCache);

            Group = group;
            CurrentWorld = Group.CurrentWorld;
            CollectorTrigger = trigger;

            foreach (var index in trigger.Indexes)
                if (!@group.Filter.AllOfIndexes.Contains(index) || @group.Filter.AnyOfIndexes.Contains(index))
                    throw new CollectorGroupMissingComponent();
        }

        public Group Group { get; }
        public World CurrentWorld { get; }
        public CollectorTrigger CollectorTrigger { get; }
        public bool IsDestroyed { get; private set; }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new CollectorIsDestroyedException(this);

            return _entities.CachedData;
        }

        public void ClearEntities()
        {
            lock (_entities)
            {
                _entities.UncachedData.Clear();
                _entities.IsDirty = true;
            }
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            lock (_entities)
            {
                _entities.UncachedData.Remove(entity.Id);
                _entities.IsDirty = true;
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
            _entities.UncachedData.Clear();
            _entities.IsDirty = true;

            IsDestroyed = true;
        }

        private void AppendEntity(Entity entity, int componentPoolIndex, int[] indexes)
        {
            if (indexes.Contains(componentPoolIndex))
                lock (_entities)
                {
                    if (!_entities.UncachedData.ContainsKey(entity.Id))
                    {
                        _entities.UncachedData.Add(entity.Id, entity);
                        _entities.IsDirty = true;
                    }
                }
        }

        private Entity[] UpdateEntitiesCache()
        {
            lock (_entities)
            {
                return _entities.UncachedData.Values.ToArray();
            }
        }
    }
}