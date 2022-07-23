namespace EcsLte.Exceptions
{
    public class EcsContextDifferentException : EcsLteException
    {
        public EcsContextDifferentException(EcsContext context1, EcsContext context2)
            : base($"Cannot use different EcsContext sources '{context1.Name}', '{context2.Name}'.")
        { }
    }
}
