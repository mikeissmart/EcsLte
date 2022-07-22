using System;

namespace EcsLte.Exceptions
{
    public class EntityFilterNotHaveWhereAllOfException : EcsLteException
    {
        public EntityFilterNotHaveWhereAllOfException(Type componentType)
            : base($"EntityFilter not have WhereAllOf component '{componentType.Name}'.")
        {
        }
    }

    public class EntityFilterNotHaveWhereAnyOfException : EcsLteException
    {
        public EntityFilterNotHaveWhereAnyOfException(Type componentType)
            : base($"EntityFilter not have WhereAnyOf component '{componentType.Name}'.")
        {
        }
    }

    public class EntityFilterNotHaveWhereNoneOfException : EcsLteException
    {
        public EntityFilterNotHaveWhereNoneOfException(Type componentType)
            : base($"EntityFilter not have WhereNoneOf component '{componentType.Name}'.")
        {
        }
    }
}