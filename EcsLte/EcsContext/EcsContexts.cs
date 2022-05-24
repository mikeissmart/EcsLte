using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public static class EcsContexts
    {
        private static readonly Dictionary<string, EcsContext> _ecsContexts =
            new Dictionary<string, EcsContext>();

        private static readonly object _lockObj = new object();

        public static EcsContext Default { get; set; } = CreateContext("Default");

        public static bool HasContext(string name)
            => _ecsContexts.ContainsKey(name);

        public static EcsContext[] GetAllContexts() => _ecsContexts.Values.ToArray();

        public static EcsContext GetContext(string name)
        {
            if (!HasContext(name))
                throw new EcsContextDoesNotExistException(name);

            return _ecsContexts[name];
        }

        public static EcsContext CreateContext(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (HasContext(name))
                throw new EcsContextNameAlreadyExistException(name);

            lock (_lockObj)
            {
                var context = new EcsContext(name);
                _ecsContexts.Add(name, context);

                return context;
            }
        }

        public static void DestroyContext(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException();
            if (context.IsDestroyed)
                throw new EcsContextIsDestroyedException(context);

            lock (_lockObj)
            {
                context.InternalDestroy();
                _ecsContexts.Remove(context.Name);
            }
        }
    }
}