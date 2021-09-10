using System;

namespace EcsLte.Exceptions
{
    public class SystemDoesNotSystemException : EcsLteException
    {
        public SystemDoesNotSystemException(World world, Type systemType)
            : base($"World '{world}' does not have system '{nameof(systemType)}'.",
                "Check if world has system before removing it.")
        {
        }
    }
}