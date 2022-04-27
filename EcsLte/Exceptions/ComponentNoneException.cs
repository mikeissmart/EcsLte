using System;
using System.Collections.Generic;
using System.Text;

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
