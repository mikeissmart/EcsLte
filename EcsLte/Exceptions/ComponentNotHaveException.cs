using System;

namespace EcsLte.Exceptions
{
    public class ComponentNotHaveException : EcsLteException
    {
        public ComponentNotHaveException(Type componentType)
            : base($"Does not have component '{componentType.Name}'.")
        {
        }
    }
}