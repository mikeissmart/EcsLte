using System;
using System.Collections.Generic;
using System.Text;

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
