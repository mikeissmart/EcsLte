using System;

namespace EcsLte.Exceptions
{
    public class EntityUniqueComponentExistsException : EcsLteException
    {
        public EntityUniqueComponentExistsException(Entity entity, Type componentType)
            : base($"Entity '{entity}' already has unique component '{componentType.Name}'.")
        {
        }
    }
}