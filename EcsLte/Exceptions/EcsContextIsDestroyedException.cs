namespace EcsLte.Exceptions
{
    public class EcsContextIsDestroyedException : EcsLteException
    {
        public EcsContextIsDestroyedException(EcsContext context)
            : base($"EcsContext '{context.Name}' is already destroyed.")
        {
        }
    }
}
