using System;

namespace EcsLte.Exceptions
{
    public class EntityAlreadyHasComponentException : EcsLteException
    {
        public EntityAlreadyHasComponentException(EcsContext context, Entity entity, Type componentType)
            : base($"EcsContext '{context}' entity '{entity}' already has component '{componentType.Name}'.",
                "Check if entity has component before adding component or use replace component.")
        {
        }
    }
}