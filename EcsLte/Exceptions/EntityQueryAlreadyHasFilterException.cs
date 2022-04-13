using System;

namespace EcsLte.Exceptions
{
    public class EntityQueryAlreadyHasFilterException : EcsLteException
    {
        public EntityQueryAlreadyHasFilterException(Type componentType)
            : base($"EntityQuery already added shared component '{componentType.Name}'.")
        {
        }
    }
}
