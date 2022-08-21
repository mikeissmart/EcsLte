using System;

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
