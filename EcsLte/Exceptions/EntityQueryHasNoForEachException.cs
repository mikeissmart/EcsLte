using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityQueryHasNoForEachException : EcsLteException
    {
        public EntityQueryHasNoForEachException()
            : base($"EntityQuery has no ForEach action to run.")
        { }
    }
}
