using System;

namespace EcsLte.Exceptions
{
    public class EntityArcheTypeSharedComponentException : EcsLteException
    {
        public EntityArcheTypeSharedComponentException(Type componentType)
            : base($"EntityArcheType use AddSharedComponent for ISharedComponents '{componentType.Name}'.")
        {
        }
    }
}