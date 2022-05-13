using System;

namespace EcsLte.Exceptions
{
    public class SystemAttributeException : EcsLteException
    {
        public SystemAttributeException(Type systemType)
            : base($"System '{systemType.Name}' must inherit from EcsLte.SystemBase.")
        { }
    }
}
