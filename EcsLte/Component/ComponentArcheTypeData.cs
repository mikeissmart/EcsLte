using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte
{
    internal delegate void ComponentArcheTypeDataEvent(ComponentArcheTypeData archeTypeData);

    internal class ComponentArcheTypeData : IGetEntity
    {
        private DataCache<Dictionary<int, Entity>, Entity[]> _entities;

        internal ComponentArcheType ArcheType { get; private set; }
        internal int Count { get => _entities.UncachedData.Count; }
        internal event EntityEvent EntityAdded;
        internal event EntityEvent EntityRemoved;
        internal event EntityEvent EntityUpdated;

        public ComponentArcheTypeData()
        {
            _entities = new DataCache<Dictionary<int, Entity>, Entity[]>(
                new Dictionary<int, Entity>(), UpdateCachedData);
        }

        #region ComponentArcheTypeData

        internal void AddEntity(Entity entity)
        {
            lock (_entities)
            {
                _entities.UncachedData.Add(entity.Id, entity);
                _entities.SetDirty();

                if (EntityAdded != null)
                    EntityAdded.Invoke(entity);
            }
        }

        internal void RemoveEntity(Entity entity)
        {
            lock (_entities)
            {
                _entities.UncachedData.Remove(entity.Id);
                _entities.SetDirty();

                if (EntityRemoved != null)
                    EntityRemoved.Invoke(entity);
            }
        }

        internal void UpdateEntity(Entity entity)
        {
            if (EntityUpdated != null)
                EntityUpdated.Invoke(entity);
        }

        private static Entity[] UpdateCachedData(Dictionary<int, Entity> unchacedData)
        {
            return unchacedData.Values.ToArray();
        }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            return _entities.UncachedData.ContainsKey(entity.Id);
        }

        public Entity[] GetEntities()
        {
            return _entities.CachedData;
        }

        #endregion

        #region ObjectCache

        internal void Initialize(ComponentArcheType archeType)
        {
            ArcheType = archeType;
        }

        internal void Reset()
        {
            _entities.UncachedData.Clear();
            _entities.SetDirty();
            EntityAdded = null;
            EntityRemoved = null;
            EntityUpdated = null;
        }

        #endregion
    }
}