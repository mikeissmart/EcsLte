using System;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    public enum CollectorTriggerEvent
    {
        None = 0,
        Added = 1,
        Removed = 2,
        Updated = 4
    }

    public struct CollectorTrigger : IEquatable<CollectorTrigger>
    {
        public static CollectorTrigger Added(Filter filter)
        {
            return new CollectorTrigger(filter, CollectorTriggerEvent.Added);
        }

        public static CollectorTrigger Removed(Filter filter)
        {
            return new CollectorTrigger(filter, CollectorTriggerEvent.Removed);
        }

        public static CollectorTrigger Updated(Filter filter)
        {
            return new CollectorTrigger(filter, CollectorTriggerEvent.Updated);
        }

        public static CollectorTrigger AddedOrRemoved(Filter filter)
        {
            return new CollectorTrigger(filter, CollectorTriggerEvent.Added | CollectorTriggerEvent.Removed);
        }

        public static CollectorTrigger AddedOrUpdated(Filter filter)
        {
            return new CollectorTrigger(filter, CollectorTriggerEvent.Added | CollectorTriggerEvent.Updated);
        }

        public Filter Filter { get; private set; }
        public CollectorTriggerEvent Trigger { get; private set; }

        public CollectorTrigger(Filter filter, CollectorTriggerEvent triggerEvent)
        {
            Filter = filter;
            Trigger = triggerEvent;
        }

        public static bool operator !=(CollectorTrigger lhs, CollectorTrigger rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(CollectorTrigger lhs, CollectorTrigger rhs)
        {
            return lhs.Trigger == rhs.Trigger && lhs.Filter == rhs.Filter;
        }

        public bool Equals(CollectorTrigger other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is CollectorTrigger other && this == other;
        }

        public override int GetHashCode()
        {
            int hashCode = -1663471673;
            hashCode = hashCode * -1521134295 + Filter.GetHashCode();
            hashCode = hashCode * -1521134295 + Trigger.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Trigger.ToString()},  {Filter.ToString()}";
        }
    }
}