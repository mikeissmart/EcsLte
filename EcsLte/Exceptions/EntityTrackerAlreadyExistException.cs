namespace EcsLte.Exceptions
{
    public class EntityTrackerAlreadyExistException : EcsLteException
    {
        public EntityTrackerAlreadyExistException(string name)
            : base($"EntityTracker already exist with name '{name}'.")
        { }
    }
}
