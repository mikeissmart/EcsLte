namespace EcsLte.Exceptions
{
    public class EcsContextNotExistException : EcsLteException
    {
        public EcsContextNotExistException(string name)
            : base($"EcsContext with name '{name}' does not exists.")
        { }
    }
}