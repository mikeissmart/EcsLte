using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityBlueprintAlreadyHasComponentException : EcsLteException
    {
        public EntityBlueprintAlreadyHasComponentException(Type componentType)
            : base($"EntityBlueprint already has component '{componentType.Name}'.")
        {
        }
    }
}
