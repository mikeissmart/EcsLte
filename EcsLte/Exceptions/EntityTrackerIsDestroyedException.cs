namespace EcsLte.Exceptions
{
    public class EntityTrackerIsDestroyedException : EcsLteException
    {
        public EntityTrackerIsDestroyedException(EntityTracker tracker)
            : base($"EntityTracker '{tracker.Name}' is already destroyed.")
        {
        }
    }
}
