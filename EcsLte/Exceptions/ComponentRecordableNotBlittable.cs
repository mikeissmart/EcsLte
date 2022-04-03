using System;

namespace EcsLte.Exceptions
{
    public class ComponentRecordableNotBlittable : EcsLteException
    {
        public ComponentRecordableNotBlittable(Type[] componentTypes)
            : base($"IRecordableComponents must be blittable ({TypesToString(componentTypes)}).")
        {
        }
    }
}
