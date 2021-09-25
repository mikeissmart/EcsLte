using System;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityCollection
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
                _entities.SetDirty();
            }
        }
        internal int Length { get => _entities.UncachedData.Length; }

        internal Entity[] GetEntities()
        {
            return _entities.CachedData;
        }

        public void CopyFrom(EntityCollection source)
        {
            Array.Copy(source._entities.UncachedData, source._entities.UncachedData, Length);
            _entities.SetDirty();
        }

        internal void Resize(int newSize)
        {
            if (newSize > _entities.UncachedData.Length)
                Array.Resize(ref _entities.UncachedData, newSize);
        }

        internal void Initialize(int initialEntitySize)
        {
            if (_entities == null)
                _entities = new DataCache<EcsLte.Entity[], EcsLte.Entity[]>(
                    new Entity[initialEntitySize],
                    UpdateEntitiesCache);
            else
                Resize(initialEntitySize);
        }

        internal void Reset()
        {
            Array.Clear(_entities.UncachedData, 0, _entities.UncachedData.Length);
            _entities.SetDirty();
        }

        private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
        {
            return uncachedData
                .Where(x => x != Entity.Null)
                .ToArray();
        }
    }
}