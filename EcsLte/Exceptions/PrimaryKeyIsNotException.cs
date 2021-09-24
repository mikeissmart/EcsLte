using System;

namespace EcsLte.Exceptions
{
    public class PrimaryKeyAlreadyHasException : EcsLteException
    {
        public PrimaryKeyAlreadyHasException(EcsContext context, IComponentPrimaryKey key)
            : base($"EcsContext '{context}' already has primary key component '{key.ToString()}'.",
                @"Cannot have duplicate primary key components.
                Change primary key component to shared key component or remove existing primary key component.")
        {
        }
    }
}