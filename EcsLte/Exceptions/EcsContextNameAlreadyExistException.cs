namespace EcsLte.Exceptions
{
    public class EcsContextNameAlreadyExistException : EcsLteException
    {
        public EcsContextNameAlreadyExistException(string name)
            : base($"EcsContext with name '{name}' already exists. Cannot have multiple contexts with same name.",
                "Use a different context name or check if context with name already exists.")
        {
        }
    }
}