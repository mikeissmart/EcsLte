using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityCommandManager
    {
        private readonly Dictionary<string, EntityCommandQueue> _commandQueues;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }

        internal EntityCommandManager(EcsContext context)
        {
            _commandQueues = new Dictionary<string, EntityCommandQueue>();
            _lockObj = new object();
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

            lock (_lockObj)
            {
                var commandQueue = new EntityCommandQueue(Context, name);
                _commandQueues.Add(name, commandQueue);

                return commandQueue;
            }
        }

        public void RemoveCommandQueue(EntityCommandQueue commandQueue)
        {
            if (commandQueue == null)
                throw new ArgumentNullException(nameof(commandQueue));
            if (!HasCommandQueue(commandQueue.Name))
                throw new EntityCommandQueueNotExistException(commandQueue.Name);

            lock (_lockObj)
            {
                _commandQueues.Remove(commandQueue.Name);
            }
        }

        internal void InternalDestroy()
        {
            lock (_lockObj)
            {
                _commandQueues.Clear();
            }
        }
    }
}
