namespace EcsLte.Exceptions
{
    public class ComponentsNoneException : EcsLteException
    {
        public ComponentsNoneException()
            : base($"No components.")
        {
        }
    }
}