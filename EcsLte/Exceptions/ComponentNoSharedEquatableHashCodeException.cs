using System;
using System.Collections.Generic;

namespace EcsLte.Exceptions
{
    public class ComponentNoSharedEquatableHashCodeException : EcsLteException
    {
        public ComponentNoSharedEquatableHashCodeException(List<Type> componentTypes)
            : base($"Shared components dont inherit IEquatable and override GetHashCode ({TypesToString(componentTypes)}).")
        {
        }
    }
}