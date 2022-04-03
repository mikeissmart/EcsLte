using System;
using System.Linq;

namespace EcsLte.Exceptions
{
    public class EcsLteException : Exception
    {
        public EcsLteException(string message) : base(message)
        {
        }

        protected static string TypesToString(Type[] types)
            => string.Join(", ", types.Select(x => x.Name));
    }
}
