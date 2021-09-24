using System;

namespace EcsLte.Exceptions
{
    public class EntityAlreadyHasComponentUniqueException : EcsLteException
    {
        public EntityAlreadyHasComponentUniqueException(EcsContext context, Type componentType)
            : base($"EcsContext '{context}' an entity already has unique component '{componentType.Name}'.",
                "Check if an entity has unique component before adding unique component or use replace component.")
        {
        }
    }
}