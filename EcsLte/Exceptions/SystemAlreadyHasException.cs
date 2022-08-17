using System;

namespace EcsLte.Exceptions
{
    public class SystemAlreadyHaveException : EcsLteException
    {
        public SystemAlreadyHaveException(Type systemType)
            : base($"Already has system '{systemType.Name}'.")
        {
        }
    }
}