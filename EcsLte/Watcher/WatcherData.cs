using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class WatcherData
    {
        private readonly DataCache<Dictionary<int, Entity>, Entity[]> _entities;

        public WatcherData()
        {
            _entities = new DataCache<Dictionary<int, Entity>, Entity[]>(
                new Dictionary<int, Entity>(), UpdateCachedData);
        }

        internal bool HasEntity(Entity entity)
        {
            return _entities.UncachedData.ContainsKey(entity.Id);
        }

        internal Entity[] GetEntities()
        {
            return _entities.CachedData;
        }

        internal void AddEntity(Entity entity)
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

        internal void ClearEntities()
        {
            lock (_entities)
            {
                _entities.UncachedData.Clear();
                _entities.SetDirty();
            }
        }

        private static Entity[] UpdateCachedData(Dictionary<int, Entity> unchacedData)
        {
            return unchacedData.Values.ToArray();
        }

        #region ObjectCache

        internal void Initialize()
        {
        }

        internal void Reset()
        {
            _entities.UncachedData.Clear();
            _entities.SetDirty();
        }

        #endregion
    }
}