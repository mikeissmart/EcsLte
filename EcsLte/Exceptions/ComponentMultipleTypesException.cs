using System;
using System.Collections.Generic;

namespace EcsLte.Exceptions
{
    public class ComponentMultipleTypesException : EcsLteException
    {
        public ComponentMultipleTypesException(List<Type> componentTypes)
            : base($"Components cannot be more than one IGeneralComponent, ISharedComponent, and IUniqueComponent : ({TypesToString(componentTypes)}).")
        {
        }
    }
}