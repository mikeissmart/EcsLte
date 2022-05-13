using System;

namespace EcsLte.Exceptions
{
    public class EntityArcheTypeNotHaveComponentException : EcsLteException
    {
        public EntityArcheTypeNotHaveComponentException(Type componentType)
            : base($"EntityArcheType does not have component '{componentType.Name}'.")
        {
        }
    }
}
