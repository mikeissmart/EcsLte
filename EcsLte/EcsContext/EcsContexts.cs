using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public static class EcsContexts
    {
        private static readonly Dictionary<string, EcsContext> _contexts =
            new Dictionary<string, EcsContext>();
        private static readonly object _lockObj = new object();
        private static EcsContext _default;
        private static bool _defaultInit;

        public static EcsContext Default
        {
            get
            {
                if (!_defaultInit)
                {
                    _default = CreateContext("Default");
                    _defaultInit = true;
                }

                return _default;
            }
            set
            {
                _defaultInit = true;
                _default = value;
            }
        }

        public static bool HasContext(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            lock (_lockObj)
            {
                return _contexts.ContainsKey(name);
            }
        }

        public static EcsContext[] GetAllContexts()
        {
            lock (_lockObj)
            {
                return _contexts.Values.ToArray();
            }
        }

        public static EcsContext GetContext(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            lock (_lockObj)
            {
                AssertNotExistContext(name);

                return _contexts[name];
            }
        }

        public static EcsContext CreateContext(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            lock (_lockObj)
            {
                AssertAlreadyHaveContext(name);

                SystemConfigs.Initialize();

                var context = new EcsContext(name);
                _contexts.Add(name, context);

                return context;
            }
        }

        public static void DestroyContext(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException();

            lock (_lockObj)
            {
                context.AssertContext();
                AssertNotExistContext(context.Name);

                context.InternalDestroy();
                _contexts.Remove(context.Name);
            }
        }

        private static void AssertNotExistContext(string name)
        {
            if (!_contexts.ContainsKey(name))
                throw new EcsContextNotExistException(name);
        }

        private static void AssertAlreadyHaveContext(string name)
        {
            if (_contexts.ContainsKey(name))
                throw new EcsContextAlreadyExistException(name);
        }
    }
}