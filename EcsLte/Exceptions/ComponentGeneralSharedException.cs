using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentGeneralSharedException : EcsLteException
    {
        public ComponentGeneralSharedException(List<Type> componentTypes)
            : base($"Components cant inherit both IGeneralComponent and ISharedComponent ({TypesToString(componentTypes)}).")
        {
        }
    }
}
