using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityQueryNothavFilterByException : EcsLteException
    {
        public EntityQueryNothavFilterByException(Type componentType)
            : base($"EntityQuery does not have filtered by component '{componentType.Name}'.")
        {
        }
    }
}
