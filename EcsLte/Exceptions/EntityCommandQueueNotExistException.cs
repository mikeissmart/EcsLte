namespace EcsLte.Exceptions
{
    public class EntityCommandQueueNotExistException : EcsLteException
    {
        public EntityCommandQueueNotExistException(string name)
            : base($"EntityCommandQueue does not exist with name '{name}'.")
        { }
    }
}
