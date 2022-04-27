using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentNoSharedEquatableHashCodeException : EcsLteException
    {
        public ComponentNoSharedEquatableHashCodeException(List<Type> componentTypes)
            : base($"Shared components dont inherit IEquatable or override GetHashCode ({TypesToString(componentTypes)}).")
        {
        }
    }
}
