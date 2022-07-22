namespace EcsLte.Exceptions
{
    public class EntityNotExistException : EcsLteException
    {
        public EntityNotExistException(Entity entity)
            : base($"Entity does not exist '{entity}'.")
        { }
    }
}