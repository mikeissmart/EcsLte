using System;

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
