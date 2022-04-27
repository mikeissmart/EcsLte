using System;
using System.Collections.Generic;
using System.Text;

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
