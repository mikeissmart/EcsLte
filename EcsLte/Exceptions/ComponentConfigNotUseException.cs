namespace EcsLte.Exceptions
{
    public class ComponentConfigNotUseGeneralException : EcsLteException
    {
        public ComponentConfigNotUseGeneralException()
            : base($"Cannot use General ComponentConfig.")
        {
        }
    }

    public class ComponentConfigNotUseManagedException : EcsLteException
    {
        public ComponentConfigNotUseManagedException()
            : base($"Cannot use Managed ComponentConfig.")
        {
        }
    }

    public class ComponentConfigNotUseSharedException : EcsLteException
    {
        public ComponentConfigNotUseSharedException()
            : base($"Cannot use Shared ComponentConfig.")
        {
        }
    }
}
