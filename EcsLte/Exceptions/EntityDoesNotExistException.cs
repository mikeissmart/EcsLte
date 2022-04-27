using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityDoesNotExistException : EcsLteException
    {
        public EntityDoesNotExistException(Entity entity)
            : base($"Entity does not exist '{entity}'.")
        {}
    }
}
