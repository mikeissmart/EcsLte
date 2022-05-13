using System;

namespace EcsLte.Exceptions
{
    public class SystemAlreadyHasException : EcsLteException
    {
        public SystemAlreadyHasException(Type systemType)
            : base($"Already has system '{systemType.Name}'.")
        {
        }
    }
}
