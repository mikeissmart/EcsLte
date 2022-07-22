namespace EcsLte.Exceptions
{
    public class EntityCommandsNotExistException : EcsLteException
    {
        public EntityCommandsNotExistException(string name)
            : base($"EntityCommands does not exist with name '{name}'.")
        { }
    }
}