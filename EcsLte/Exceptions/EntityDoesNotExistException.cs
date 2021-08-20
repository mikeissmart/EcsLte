namespace EcsLte.Exceptions
{
    public class EntityDoesNotExistException : EcsLteException
    {
        public EntityDoesNotExistException(World world, Entity entity)
            : base($"World '{world}' does not have entity '{entity}'.",
                "Use same world that created entity to use entity.")
        {
        }
    }
}