namespace EcsLte.Exceptions
{
    public class WorldNameAlreadyExistException : EcsLteException
    {
        public WorldNameAlreadyExistException(string name)
            : base($"World with name '{name}' already exists. Cannot have multiple worlds with same name.",
                "Use a different world name or check if world with name already exists.")
        {
        }
    }
}