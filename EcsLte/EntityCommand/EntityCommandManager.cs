using EcsLte.Exceptions;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityCommandManager
    {
        private readonly Dictionary<string, EntityCommandQueue> _commandQueues;

        public EcsContext Context { get; private set; }

        internal EntityCommandManager(EcsContext context)
        {
            _commandQueues = new Dictionary<string, EntityCommandQueue>();
            Context = context;
        }

        public bool HasCommandQueue(string name)
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _commandQueues.ContainsKey(name);
        }

        public EntityCommandQueue GetCommandQueue(string name)
        {
            if (!HasCommandQueue(name))
                throw new EntityCommandQueueNotExistException(name);

            return _commandQueues[name];
        }

        public EntityCommandQueue CreateCommandQueue(string name)
        {
            if (HasCommandQueue(name))
                throw new EntityCommandQueueAlreadyExistException(name);

            var commandQueue = new EntityCommandQueue(Context, name);
            _commandQueues.Add(name, commandQueue);

            return commandQueue;
        }

        public void InternalDestroy() => _commandQueues.Clear();
    }
}
