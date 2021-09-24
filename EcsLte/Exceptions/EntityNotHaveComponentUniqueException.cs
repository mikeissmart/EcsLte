using System;

namespace EcsLte.Exceptions
{
    public class EntityNotHaveComponentUniqueException : EcsLteException
    {
        public EntityNotHaveComponentUniqueException(EcsContext context, Type componentType)
            : base($"EcsContext '{context}' no entity does not have unique component '{componentType.Name}'.",
                "Check if entity has unique component before get or remove unique component.")
        {
        }
    }
}