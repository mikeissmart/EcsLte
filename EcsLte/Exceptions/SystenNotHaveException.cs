using System;

namespace EcsLte.Exceptions
{
    public class SystenNotHaveException : EcsLteException
    {
        public SystenNotHaveException(Type systemType)
            : base($"Does not have system '{systemType.Name}'.")
        { }
    }
}