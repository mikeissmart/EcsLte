using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public class EcsContext
    {
        private static readonly Dictionary<string, EcsContext> _ecsContexts =
            new Dictionary<string, EcsContext>();
        private static readonly object _lockObj = new object();

        internal EcsContext(string name)
        {
            ArcheTypeManager = new ArcheTypeManager(this);
            EntityManager = new EntityManager(this);
            QueryManager = new EntityQueryManager(this);
            CommandManager = new EntityCommandManager(this);
            SystemManager = new SystemManager(this);

            Name = name;
        }

        public static EcsContext Default { get; set; } = CreateContext("Default");

        public string Name { get; }
        public bool IsDestroyed { get; private set; }

        public EntityManager EntityManager { get; private set; }
        public EntityCommandManager CommandManager { get; private set; }
        public SystemManager SystemManager { get; private set; }
        internal EntityQueryManager QueryManager { get; private set; }
        internal ArcheTypeManager ArcheTypeManager { get; private set; }

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

        internal void InternalDestroy()
        {
            ArcheTypeManager.InternalDestroy();
            EntityManager.InternalDestroy();
            QueryManager.InternalDestroy();
            CommandManager.InternalDestroy();
            SystemManager.InternalDestroy();

            IsDestroyed = true;
        }
    }
}
