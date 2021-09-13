using System.Collections.Generic;

namespace EcsLte
{
    internal class SubCollector
    {
        private List<Collector> _collectors;

        internal SubCollector(Group group, CollectorTriggerEvent triggerEvent)
        {
            _collectors = new List<Collector>();
            Group = group;
            TriggerEvent = triggerEvent;
        }

        internal Group Group { get; private set; }
        internal CollectorTriggerEvent TriggerEvent { get; private set; }

        internal void AddCollector(Collector collector)
        {
            _collectors.Add(collector);
        }

        internal void RemoveCollector(Collector collector)
        {
            _collectors.Remove(collector);
        }

        internal void OnEntityArrayResize(int newSize)
        {
            lock (_collectors)
            {
                foreach (var subCollector in _collectors)
                    subCollector.OnEntityArrayResize(newSize);
            }
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            lock (_collectors)
            {
                foreach (var collector in _collectors)
                    collector.OnEntityWillBeDestroyed(entity);
            }
        }

        internal void AddedEntity(Group group, Entity entity)
        {
            if ((TriggerEvent & CollectorTriggerEvent.Added) == CollectorTriggerEvent.Added)
            {
                lock (_collectors)
                {
                    foreach (var collector in _collectors)
                        collector.AddedEntity(group, entity);
                }
            }
        }

        internal void RemovedEntity(Group group, Entity entity)
        {
            if ((TriggerEvent & CollectorTriggerEvent.Removed) == CollectorTriggerEvent.Removed)
            {
                lock (_collectors)
                {
                    foreach (var collector in _collectors)
                        collector.RemovedEntity(group, entity);
                }
            }
        }

        internal void UpdatedEntity(Group group, Entity entity)
        {
            if ((TriggerEvent & CollectorTriggerEvent.Updated) == CollectorTriggerEvent.Updated)
            {
                lock (_collectors)
                {
                    foreach (var collector in _collectors)
                        collector.UpdatedEntity(group, entity);
                }
            }
        }

        internal void InternalDestroy()
        {
            _collectors.Clear();
        }
    }
}
