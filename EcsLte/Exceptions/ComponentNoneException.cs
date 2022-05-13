namespace EcsLte.Exceptions
{
    public class ComponentNoneException : EcsLteException
    {
        public ComponentNoneException()
            : base("No components in project.")
        {
        }
    }
}
