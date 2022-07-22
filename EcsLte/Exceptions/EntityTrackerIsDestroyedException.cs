using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityTrackerIsDestroyedException : EcsLteException
    {
        public EntityTrackerIsDestroyedException(EntityTracker tracker)
            : base($"EntityTracker '{tracker.Name}' is already destroyed.")
        {
        }
    }
}
