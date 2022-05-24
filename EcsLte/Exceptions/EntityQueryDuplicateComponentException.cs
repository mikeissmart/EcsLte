using System;

namespace EcsLte.Exceptions
{
    public class EntityQueryDuplicateComponentException : EcsLteException
    {
        public EntityQueryDuplicateComponentException(Type componentType)
            : base($"EntityQuery duplicate component '{componentType.Name}' provided.")
        {
        }
    }
}