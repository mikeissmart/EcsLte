using System;
using System.Collections.Generic;
using System.Text;

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
