namespace EcsLte.Exceptions
{
    public class EntityArcheTypeNoSharedComponentException : EcsLteException
    {
        public EntityArcheTypeNoSharedComponentException()
            : base($"EntityArcheType has no ISharedComponents.")
        {
        }
    }
}