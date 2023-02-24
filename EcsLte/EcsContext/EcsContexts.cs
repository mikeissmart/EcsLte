using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EcsContexts
    {
        private static EcsContexts _instance;

        public static EcsContexts Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EcsContexts();
                return _instance;
            }
        }

        private readonly Dictionary<string, EcsContext> _contexts =
            new Dictionary<string, EcsContext>();
        private readonly object _lockObj = new object();
        private EcsContext _default;
        private bool _defaultInit;

        public EcsContext Default
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

        private EcsContexts() { }

        public bool HasContext(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            lock (_lockObj)
            {
                return _contexts.ContainsKey(name);
            }
        }

        public EcsContext[] GetAllContexts()
        {
            lock (_lockObj)
            {
                return _contexts.Values.ToArray();
            }
        }

        public EcsContext GetContext(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            lock (_lockObj)
            {
                AssertNotExistContext(name);

                return _contexts[name];
            }
        }

        public EcsContext CreateContext(string name)
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

        public void DestroyContext(EcsContext context)
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

        private void AssertNotExistContext(string name)
        {
            if (!_contexts.ContainsKey(name))
                throw new EcsContextNotExistException(name);
        }

        private void AssertAlreadyHaveContext(string name)
        {
            if (_contexts.ContainsKey(name))
                throw new EcsContextAlreadyExistException(name);
        }
    }
}