namespace EcsLte.Exceptions
{
    public class EntityDoesNotExistException : EcsLteException
    {
        public EntityDoesNotExistException(EcsContext context, Entity entity)
            : base($"EcsContext '{context}' does not have entity '{entity}'.",
                "Use same context that created entity to use entity.")
        {
        }
    }
}