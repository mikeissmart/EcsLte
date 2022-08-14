using System;
using System.Collections.Generic;

namespace EcsLte.Exceptions
{
    public class ComponentNotBlittableNotManagedException : EcsLteException
    {
        public ComponentNotBlittableNotManagedException(List<Type> componentTypes)
            : base($"Managed components must inherit IManagedComponent ({TypesToString(componentTypes)}).")
        {
        }
    }
}
