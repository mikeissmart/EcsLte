namespace EcsLte.Exceptions
{
    public class EntityQueryAlreadyRunningException : EcsLteException
    {
        public EntityQueryAlreadyRunningException()
            : base($"Cannot run multiple EntityQueries at the same time.")
        { }
    }
}
