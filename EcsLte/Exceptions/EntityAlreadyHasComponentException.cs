using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityAlreadyHasComponentException : EcsLteException
    {
        public EntityAlreadyHasComponentException(Entity entity, Type componentType)
            : base($"Entity '{entity}' already has component '{componentType.Name}'.")
        {
        }
    }
}
