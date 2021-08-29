namespace EcsLte.Exceptions
{
    public class CollectorIsDestroyedException : EcsLteException
    {
        public CollectorIsDestroyedException(Collector collector)
            : base($"Collector '{collector}' is already destroyed.",
                "Cannot use collector after its been destroyed.")
        {
        }
    }
}