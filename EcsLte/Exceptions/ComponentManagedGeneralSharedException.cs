using System;
using System.Collections.Generic;

namespace EcsLte.Exceptions
{
    public class ComponentManagedGeneralSharedException : EcsLteException
    {
        public ComponentManagedGeneralSharedException(List<Type> componentTypes)
            : base($"Managed components cant inherit IGeneralComponent and ISharedComponent ({TypesToString(componentTypes)}).")
        {
        }
    }
}
