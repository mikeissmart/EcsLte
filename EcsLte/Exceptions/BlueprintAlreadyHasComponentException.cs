using System;
namespace EcsLte.Exceptions
{
    public class BlueprintAlreadyHasComponentException : EcsLteException
    {
        public BlueprintAlreadyHasComponentException(Type componentType)
            : base($"Blueprint already has component '{componentType.Name}'.",
                "Check if blueprint has component before adding component or use replace component.")
        {
        }
    }
}