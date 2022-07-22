using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityTrackerNotTrackingComponentException : EcsLteException
    {
        public EntityTrackerNotTrackingComponentException(EntityTracker tracker, Type componentType)
            : base($"EntityTracker '{tracker.Name}' does not have component '{componentType.Name}'.")
        { }
    }
}
