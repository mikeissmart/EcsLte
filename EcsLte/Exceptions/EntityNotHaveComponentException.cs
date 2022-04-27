using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityNotHaveComponentException : EcsLteException
    {
        public EntityNotHaveComponentException(Entity entity, Type componentType)
            : base($"Entity '{entity}' does not have component '{componentType.Name}'.")
        { }
    }
}
