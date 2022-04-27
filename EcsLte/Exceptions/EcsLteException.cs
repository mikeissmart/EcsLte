using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EcsLteException : Exception
    {
        public EcsLteException(string message) : base(message)
        {
        }

        protected static string TypesToString(List<Type> types)
            => string.Join(", ", types.Select(x => x.Name));
    }
}
