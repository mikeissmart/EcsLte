namespace EcsLte.Exceptions
{
    public class EntityTransferSameEcsContextException : EcsLteException
    {
        public EntityTransferSameEcsContextException(EcsContext context)
            : base($"Cant transfer Entity to same EcsContext '{context.Name}'.")
        {
        }
    }
}