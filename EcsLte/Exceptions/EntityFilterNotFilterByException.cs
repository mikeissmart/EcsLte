using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityFilterNotFilterByException : EcsLteException
    {
        public EntityFilterNotFilterByException(Type componentType)
            : base($"EntityFilter not filtered by '{componentType.Name}'.")
        {
        }
    }
}
