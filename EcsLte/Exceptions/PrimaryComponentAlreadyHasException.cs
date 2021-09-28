using System;

namespace EcsLte.Exceptions
{
    public class PrimaryComponentAlreadyHasException : EcsLteException
    {
        public PrimaryComponentAlreadyHasException(EcsContext context, IPrimaryComponent primaryComponent)
            : base($"EcsContext '{context}' already has primary key component '{primaryComponent.ToString()}'.",
                @"Cannot have duplicate primary key components.
                Change primary key component to shared key component or remove existing primary key component.")
        {
        }
    }
}