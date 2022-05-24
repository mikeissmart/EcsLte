using System;

namespace EcsLte.Exceptions
{
    public class EntityQueryAlreadyHasWhereException : EcsLteException
    {
        public EntityQueryAlreadyHasWhereException(Type componentType)
            : base($"EntityQuery already added component '{componentType.Name}'.")
        {
        }
    }
}