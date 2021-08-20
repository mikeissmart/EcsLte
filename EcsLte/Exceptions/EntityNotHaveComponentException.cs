using System;

namespace EcsLte.Exceptions
{
    public class EntityNotHaveComponentException : EcsLteException
    {
        public EntityNotHaveComponentException(Entity entity, Type componentType)
            : base($"Entity '{entity}' does not have component '{componentType.Name}'.",
                "Check if entity has component before get or remove component.")
        {
        }
    }
}