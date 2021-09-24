using System;
using System.Linq;

namespace EcsLte.Exceptions
{
    public class ComponentNotStructException : EcsLteException
    {
        public ComponentNotStructException(Type[] componentTypes)
            : base("Components must be of type struct.",
                $"{TypesToString(componentTypes)}")
        {
        }

        private static string TypesToString(Type[] componentTypes)
        {
            return string.Join(
                ", ",
                componentTypes
                    .Select(x => $"'{x.Name.RemoveComponentSuffix()}'")
                    .ToArray());
        }
    }
}