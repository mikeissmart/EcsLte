using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class CollectorData
    {
        public List<SubCollector> SubCollectors;
        public DataCache<Entity[], Entity[]> Entities = new DataCache<Entity[], Entity[]>(new Entity[4],
            UpdateEntitiesCache);

        public CollectorData()
        {

        }

        public void Reset()
        {
            SubCollectors.Clear();
            Array.Clear(Entities.UncachedData, 0, Entities.UncachedData.Length);
            Entities.IsDirty = true;
        }

        private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
        {
            return uncachedData
                .Where(x => x != Entity.Null)
                .ToArray();
        }
    }
}