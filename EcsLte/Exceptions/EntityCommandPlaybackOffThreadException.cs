namespace EcsLte.Exceptions
{
    public class EntityCommandPlaybackOffThreadException : EcsLteException
    {
        public EntityCommandPlaybackOffThreadException(string name)
            : base($"EntityCommandPlayback '{name}' can only be created on main thread.",
                "Create EntityCommandPlayback on main thread.")
        {
        }
    }
}