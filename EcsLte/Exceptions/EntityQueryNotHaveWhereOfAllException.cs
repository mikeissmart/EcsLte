using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityQueryNotHaveWhereOfAllException : EcsLteException
    {
        public EntityQueryNotHaveWhereOfAllException(Type componentType)
            : base($"EntityQuery does not have component '{componentType.Name}' added by WhereAllOf.")
        {
        }
    }
}
