using System;
using System.Collections.Generic;

namespace EcsLte.Exceptions
{
    public class SystemNoSystemInterfaceException : EcsLteException
    {
        public SystemNoSystemInterfaceException(List<Type> systemTypes)
            : base($"Systems must inherit from at least one of these (EcsLte.IInitializeSystem, EcsLte.IExecuteSystem, EcsLte.ICleanupSystem, EcsLte.ITearDownSystem)  ({TypesToString(systemTypes)}).")
        { }
    }
}
