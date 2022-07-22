using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityUniqueComponentNotExistsException : EcsLteException
    {
        public EntityUniqueComponentNotExistsException(Type componentType)
            : base($"Entity with unique component '{componentType.Name}' does not exist.")
        {
        }
    }
}
