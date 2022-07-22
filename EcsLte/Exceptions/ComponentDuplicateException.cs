using System;

namespace EcsLte.Exceptions
{
    public class ComponentDuplicateException : EcsLteException
    {
        public ComponentDuplicateException(Type componentType)
            : base($"Attempting to use duplicate component '{componentType.Name}' provided.")
        {
        }
    }
}