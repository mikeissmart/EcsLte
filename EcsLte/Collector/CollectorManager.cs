using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte
{
    public class CollectorManager
    {
        private CollectorManagerData _data;

        internal CollectorManager(World world)
        {
            _data = ObjectCache.Pop<CollectorManagerData>();

            CurrentWorld = world;
        }

        public World CurrentWorld { get; private set; }

        public Collector GetCollector(params CollectorTrigger[] triggers)
        {
            var subCollectors = new List<SubCollector>();
            lock (_data.SubCollectors)
            {
                foreach (var trigger in triggers)
                {
                    if ((trigger.Trigger & CollectorTriggerEvent.Added) == CollectorTriggerEvent.Added)
                        subCollectors
                            .Add(CalculateSubCollector(trigger.Filter, CollectorTriggerEvent.Added));
                    if ((trigger.Trigger & CollectorTriggerEvent.Removed) == CollectorTriggerEvent.Removed)
                        subCollectors
                            .Add(CalculateSubCollector(trigger.Filter, CollectorTriggerEvent.Removed));
                    if ((trigger.Trigger & CollectorTriggerEvent.Updated) == CollectorTriggerEvent.Updated)
                        subCollectors
                            .Add(CalculateSubCollector(trigger.Filter, CollectorTriggerEvent.Updated));
                }
            }

            var collector = new Collector(CurrentWorld);
            collector.Initialize(subCollectors);

            return collector;
        }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);
        }

        private SubCollector CalculateSubCollector(Filter filter, CollectorTriggerEvent triggerEvent)
        {
            var trigger = new CollectorTrigger(filter, triggerEvent);
            if (!_data.SubCollectors
                .TryGetValue(trigger, out var subCollector))
            {
                var group = CurrentWorld.GroupManager.GetGroup(filter);
                subCollector = new SubCollector(group, triggerEvent);
                group.AttachCollector(subCollector);
                _data.SubCollectors.Add(trigger, subCollector);
            }

            return subCollector;
        }
    }
}