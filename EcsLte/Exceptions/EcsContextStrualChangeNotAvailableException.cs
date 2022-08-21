namespace EcsLte.Exceptions
{
    public class EcsContextStrualChangeNotAvailableException : EcsLteException
    {
        public EcsContextStrualChangeNotAvailableException()
            : base($"Structual change not available during ForEach.")
        {
        }
    }
}
