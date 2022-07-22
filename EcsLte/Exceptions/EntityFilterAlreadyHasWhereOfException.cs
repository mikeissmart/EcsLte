using System;

namespace EcsLte.Exceptions
{
    public class EntityFilterAlreadyHasWhereAllOfException : EcsLteException
    {
        public EntityFilterAlreadyHasWhereAllOfException(Type componentType)
            : base($"EntityFilter already has WhereAllOf component '{componentType.Name}'.")
        {
        }
    }

    public class EntityFilterAlreadyHasWhereAnyOfException : EcsLteException
    {
        public EntityFilterAlreadyHasWhereAnyOfException(Type componentType)
            : base($"EntityFilter already has WhereAnyOf component '{componentType.Name}'.")
        {
        }
    }

    public class EntityFilterAlreadyHasWhereNoneOfException : EcsLteException
    {
        public EntityFilterAlreadyHasWhereNoneOfException(Type componentType)
            : base($"EntityFilter already has WhereNoneOf component '{componentType.Name}'.")
        {
        }
    }
}