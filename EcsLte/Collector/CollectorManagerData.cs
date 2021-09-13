using System.Collections.Generic;

namespace EcsLte
{
    internal class CollectorManagerData
    {
        public Dictionary<CollectorTrigger, SubCollector> SubCollectors = new Dictionary<CollectorTrigger, SubCollector>();

        public CollectorManagerData()
        {

        }

        internal void Reset()
        {
            SubCollectors.Clear();
        }
    }
}