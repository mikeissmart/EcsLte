using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class GroupData
    {
        public List<Collector>[] CollectorCommponentIndexes = new List<Collector>[ComponentIndexes.Instance.Count];
        public Dictionary<CollectorTrigger, Collector> Collectors = new Dictionary<CollectorTrigger, Collector>();
        public DataCache<Entity[], Entity[]> Entities = new DataCache<Entity[], Entity[]>(new Entity[4],
                UpdateEntitiesCache);

        public GroupData()
        {
            for (var i = 0; i < CollectorCommponentIndexes.Length; i++)
                CollectorCommponentIndexes[i] = new List<Collector>();
        }

        public void Reset()
        {
            Array.Clear(Entities.UncachedData, 0, Entities.UncachedData.Length);
            Entities.IsDirty = true;
            for (var i = 0; i < CollectorCommponentIndexes.Length; i++)
                CollectorCommponentIndexes[i].Clear();
            Collectors.Clear();
        }

        private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
        {
            return uncachedData
                .Where(x => x != Entity.Null)
                .ToArray();
        }
    }
}