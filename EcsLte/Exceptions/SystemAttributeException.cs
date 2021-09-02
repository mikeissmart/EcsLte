using System;

namespace EcsLte.Exceptions
{
    public class SystemAttributeException : EcsLteException
    {
        public SystemAttributeException(Type systemType)
            : base($"System '{nameof(systemType)}' must inherit from ISystem.",
                "Inherit ISystem.")
        {
        }
    }
}