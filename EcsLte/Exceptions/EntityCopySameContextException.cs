namespace EcsLte.Exceptions
{
    public class EntityCopySameContextException : EcsLteException
    {
        public EntityCopySameContextException()
            : base($"Cant copy entities from same EcsContext.")
        {
        }
    }
}