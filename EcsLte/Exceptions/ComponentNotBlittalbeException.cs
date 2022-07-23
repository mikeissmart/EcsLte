using System;
using System.Collections.Generic;

namespace EcsLte.Exceptions
{
    public class ComponentNotBlittalbeException : EcsLteException
    {
        public ComponentNotBlittalbeException(List<Type> componentTypes)
            : base($"Components are not blittable ({TypesToString(componentTypes)}).")
        {
        }
    }
}
