namespace EcsLte.Exceptions
{
    public class SystemAlreadyRunningException : EcsLteException
    {
        public SystemAlreadyRunningException()
            : base("Systems already running.")
        { }
    }
}
