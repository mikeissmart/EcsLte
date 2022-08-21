namespace EcsLte.Exceptions
{
    public class EntityCopyToSameContextException : EcsLteException
    {
        public EntityCopyToSameContextException()
            : base($"Cant copy entities from same EcsContext.")
        {
        }
    }
}