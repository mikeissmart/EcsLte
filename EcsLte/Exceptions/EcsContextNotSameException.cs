namespace EcsLte.Exceptions
{
    public class EcsContextNotSameException : EcsLteException
    {
        public EcsContextNotSameException(EcsContext context1, EcsContext context2)
            : base($"EcsContext must be the same '{context1.Name}', '{context2.Name}'.")
        { }
    }
}
