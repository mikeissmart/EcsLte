namespace EcsLte.Exceptions
{
    public class EntityTransferAlreadyException : EcsLteException
    {
        public EntityTransferAlreadyException(EcsContext context, Entity entity)
            : base($"Entity '{entity}' already transfered to '{context.Name}'.")
        {
        }
    }
}