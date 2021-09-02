using System;

namespace EcsLte.Exceptions
{
    public class EntityAlreadyHasComponentUniqueException : EcsLteException
    {
        public EntityAlreadyHasComponentUniqueException(Type componentType)
            : base($"An entity already has unique component '{componentType.Name}'.",
                "Check if an entity has unique component before adding unique component or use replace component.")
        {
        }
    }
}