using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentAlreadyHaveException : EcsLteException
    {
        public ComponentAlreadyHaveException(Type componentType)
            : base($"Already has component '{componentType.Name}'.")
        {
        }
    }
}
