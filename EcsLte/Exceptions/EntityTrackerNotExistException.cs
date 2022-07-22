using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityTrackerNotExistException : EcsLteException
    {
        public EntityTrackerNotExistException(string name)
            : base($"EntityTracker does not exist with name '{name}'.")
        { }
    }
}
