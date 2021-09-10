namespace EcsLte.Exceptions
{
    public class SystemSortException : EcsLteException
    {
        public SystemSortException(string sortError)
            : base($"Error while sorting systems: '{sortError}'.",
                "Resolve sort system errors.")
        {
        }
    }
}