using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityFilterNotExistException : EcsLteException
    {
        public EntityFilterNotExistException(string name)
            : base($"EntityFilter with name '{name}' does not exists.")
        { }
    }
}
