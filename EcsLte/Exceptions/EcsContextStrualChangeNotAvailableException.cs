using System;
using System.Collections.Generic;
using System.Text;

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
