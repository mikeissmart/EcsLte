using System;

namespace EcsLte.Exceptions
{
    public class ComponentNoSharedEquatableHashCodeException : EcsLteException
    {
        public ComponentNoSharedEquatableHashCodeException(Type[] componentTypes)
            : base($"Shared components dont inherit IEquatable or override GetHashCode ({TypesToString(componentTypes)}).")
        {
        }
    }
}
