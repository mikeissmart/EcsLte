using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EcsContextIsDestroyedException : EcsLteException
    {
        public EcsContextIsDestroyedException(EcsContext context)
            : base($"EcsContext '{context.Name}' is already destroyed.")
        {
        }
    }
}
