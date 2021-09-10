using System;

namespace EcsLte.Exceptions
{
    public class SystemAlreadyHasSystemException : EcsLteException
    {
        public SystemAlreadyHasSystemException(World world, Type systemType)
            : base($"World '{world}' already has system '{nameof(systemType)}'.",
                "Check if world already has system before adding it.")
        {
        }
    }
}