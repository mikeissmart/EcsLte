namespace EcsLte.Exceptions
{
    public class EntityCommandQueueAlreadyExistException : EcsLteException
    {
        public EntityCommandQueueAlreadyExistException(string name)
            : base($"EntityCommandQueue already exist with name '{name}'.")
        { }
    }
}
