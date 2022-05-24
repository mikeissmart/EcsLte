using System;

namespace EcsLte.Exceptions
{
    public class EntityQueryAlreadyFilteredByException : EcsLteException
    {
        public EntityQueryAlreadyFilteredByException(Type componentType)
            : base($"EntityQuery already filtered by component '{componentType.Name}'.")
        {
        }
    }
}