using System;

namespace EcsLte.Exceptions
{
    public class EntityBlueprintNotHaveComponentException : EcsLteException
    {
        public EntityBlueprintNotHaveComponentException(Type componentType)
            : base($"EntityBlueprint does not have component '{componentType.Name}'.")
        {
        }
    }
}
