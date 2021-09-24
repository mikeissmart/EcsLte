namespace EcsLte.Exceptions
{
    public class EcsContextDoesNotExistException : EcsLteException
    {
        public EcsContextDoesNotExistException(string name)
            : base($"EcsContext with name '{name}' does not exists.",
                "Create context with name before using it.")
        {
        }
    }
}