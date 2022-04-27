using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentSharedUniqueException : EcsLteException
    {
        public ComponentSharedUniqueException(List<Type> componentTypes)
            : base($"Components cannot be ISharedComponent and IUniqueComponent : ({TypesToString(componentTypes)}).")
        {
        }
    }
}
