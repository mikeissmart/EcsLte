namespace EcsLte.Exceptions
{
    public class EntityQueryUniqueComponentException : EcsLteException
    {
        public EntityQueryUniqueComponentException()
            : base($"EntityQuery cannot use Unique Components.")
        {
        }
    }
}
