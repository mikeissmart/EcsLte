using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class GroupData
    {
        public HashSet<SubCollector> SubCollectors = new HashSet<SubCollector>();
        public DataCache<Entity[], Entity[]> Entities = new DataCache<Entity[], Entity[]>(new Entity[4],
                UpdateEntitiesCache);

        public GroupData()
        {
        }

        public void Reset()
        {
            Array.Clear(Entities.UncachedData, 0, Entities.UncachedData.Length);
            Entities.IsDirty = true;
            SubCollectors.Clear();
        }

        private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
        {
            return uncachedData
                .Where(x => x != Entity.Null)
                .ToArray();
        }
    }
}