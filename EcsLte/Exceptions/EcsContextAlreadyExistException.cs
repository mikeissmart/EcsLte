namespace EcsLte.Exceptions
{
    public class EcsContextAlreadyExistException : EcsLteException
    {
        public EcsContextAlreadyExistException(string name)
            : base($"EcsContext with name '{name}' already exists.")
        { }
    }
}