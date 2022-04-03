namespace EcsLte.Exceptions
{
    public class EntityDoesNotExistException : EcsLteException
    {
        public EntityDoesNotExistException(Entity entity)
            : base($"Entity does not exist '{entity}'.")
        {
        }
    }
}
