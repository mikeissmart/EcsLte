namespace EcsLte.Exceptions
{
    public class WorldDoesNotExistException : EcsLteException
    {
        public WorldDoesNotExistException(string name)
            : base($"World with name '{name}' does not exists.",
                "Create world with name before using it.")
        {
        }
    }
}