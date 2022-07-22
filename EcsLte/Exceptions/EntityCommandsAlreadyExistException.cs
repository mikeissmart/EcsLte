namespace EcsLte.Exceptions
{
    public class EntityCommandsAlreadyExistException : EcsLteException
    {
        public EntityCommandsAlreadyExistException(string name)
            : base($"EntityCommands already exist with name '{name}'.")
        { }
    }
}