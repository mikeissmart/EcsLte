using System;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    /*internal class EntityCollection
    {
        private DataCache<Entity[], Entity[]> _entities;

        internal Entity this[int index]
        {
            get
            {
                return _entities.UncachedData[index];
            }
            set
            {
                _entities.UncachedData[index] = value;
                _entities.IsDirty = true;
            }
        }

        internal Entity[] GetAllEntities()
        {
            return _entities.CachedData;
        }

        internal EntityCollection(int initialEntitySize)
        {
            _entities = new DataCache<EcsLte.Entity[], EcsLte.Entity[]>(
                new Entity[initialEntitySize],
                UpdateEntitiesCache);
        }

        internal void Resize(int newSize)
        {
            if (newSize > _entities.UncachedData.Length)
                Array.Resize(ref _entities.UncachedData, newSize);
        }

        internal void Reset()
        {
            Array.Clear(_entities.UncachedData, 0, _entities.UncachedData.Length);
            _entities.IsDirty = true;
        }

        private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
        {
            return uncachedData
                .Where(x => x != Entity.Null)
                .ToArray();
        }
    }*/
}