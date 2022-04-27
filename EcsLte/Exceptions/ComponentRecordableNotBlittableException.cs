using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentRecordableNotBlittableException : EcsLteException
    {
        public ComponentRecordableNotBlittableException(List<Type> componentTypes)
            : base($"IRecordableComponents must be blittable ({TypesToString(componentTypes)}).")
        {
        }
    }
}
