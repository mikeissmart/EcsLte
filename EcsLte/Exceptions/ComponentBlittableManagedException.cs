using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentBlittableManagedException : EcsLteException
    {
        public ComponentBlittableManagedException(List<Type> componentTypes)
            : base($"Blittable components cant inherit IManagedComponent ({TypesToString(componentTypes)}).")
        {
        }
    }
}
