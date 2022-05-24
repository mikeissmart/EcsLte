using System;

namespace EcsLte.Exceptions
{
    public class EntityArcheTypeAlreadyHasComponentException : EcsLteException
    {
        public EntityArcheTypeAlreadyHasComponentException(Type componentType)
            : base($"EntityArcheType already has component '{componentType.Name}'.")
        {
        }
    }
}