using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public static class EcsContexts
    {
        private static readonly Dictionary<string, EcsContext> _ecsContexts =
            new Dictionary<string, EcsContext>();

        public static EcsContext Default { get; set; } = CreateContext("Default");

        public static bool HasContext(string name)
            => _ecsContexts.ContainsKey(name);

        public static EcsContext GetContext(string name)
        {
            if (!HasContext(name))
                throw new EcsContextDoesNotExistException(name);

            return _ecsContexts[name];
        }

        public static EcsContext CreateContext(string name)
        {
            if (HasContext(name))
                throw new EcsContextNameAlreadyExistException(name);

            var context = new EcsContext(name);
            _ecsContexts.Add(name, context);
            return context;
        }

        public static void DestroyContext(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException();
            if (context.IsDestroyed)
                throw new EcsContextIsDestroyedException(context);

            context.InternalDestroy();

            _ecsContexts.Remove(context.Name);
        }
    }
}
