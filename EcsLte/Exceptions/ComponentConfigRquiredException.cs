namespace EcsLte.Exceptions
{
    public class ComponentConfigRequiredGeneralException : EcsLteException
    {
        public ComponentConfigRequiredGeneralException()
            : base($"General ComponentConfig is required.")
        {
        }
    }

    public class ComponentConfigRequiredManagedException : EcsLteException
    {
        public ComponentConfigRequiredManagedException()
            : base($"Managed ComponentConfig is required.")
        {
        }
    }

    public class ComponentConfigRequiredSharedException : EcsLteException
    {
        public ComponentConfigRequiredSharedException()
            : base($"Shared ComponentConfig is required.")
        {
        }
    }
}
