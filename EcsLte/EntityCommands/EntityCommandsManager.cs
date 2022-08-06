using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte
{
    public class EntityCommandsManager
    {
        private readonly Dictionary<string, EntityCommands> _commands;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }

        internal EntityCommandsManager(EcsContext context)
        {
            _commands = new Dictionary<string, EntityCommands>();
            _lockObj = new object();

            Context = context;
        }

        public bool HasCommands(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            lock (_lockObj)
            {
                return _commands.ContainsKey(name);
            }
        }

        public EntityCommands[] GetAllCommands()
        {
            Context.AssertContext();

            lock (_lockObj)
            {
                return _commands.Values.ToArray();
            }
        }

        public EntityCommands GetCommands(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertNotExistCommands(name);

                return _commands[name];
            }
        }

        public EntityCommands CreateCommands(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            lock (_lockObj)
            {
                AssertAlreadyHaveCommands(name);

                var commandQueue = new EntityCommands(Context, name);
                _commands.Add(name, commandQueue);

                return commandQueue;
            }
        }

        public void RemoveCommands(EntityCommands commands)
        {
            Context.AssertContext();
            EntityCommands.AssertEntityCommands(commands, Context);

            lock (_lockObj)
            {
                AssertNotExistCommands(commands.Name);

                commands.InternalDestroy();
                _commands.Remove(commands.Name);
            }
        }

        internal void InternalDestroy()
        {
            lock (_lockObj)
            {
                foreach (var commands in _commands.Values)
                    commands.InternalDestroy();

                _commands.Clear();
            }
        }

        private void AssertNotExistCommands(string name)
        {
            if (!_commands.ContainsKey(name))
                throw new EntityCommandsNotExistException(name);
        }

        private void AssertAlreadyHaveCommands(string name)
        {
            if (_commands.ContainsKey(name))
                throw new EntityCommandsAlreadyExistException(name);
        }
    }
}
