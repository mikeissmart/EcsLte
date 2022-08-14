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

        public EcsContext Context { get; private set; }

        internal EntityCommandsManager(EcsContext context)
        {
            _commands = new Dictionary<string, EntityCommands>();

            Context = context;
        }

        public bool HasCommands(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            return _commands.ContainsKey(name);
        }

        public EntityCommands[] GetAllCommands()
        {
            Context.AssertContext();

            return _commands.Values.ToArray();
        }

        public EntityCommands GetCommands(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            AssertNotExistCommands(name);

            return _commands[name];
        }

        public EntityCommands CreateCommands(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Context.AssertContext();

            AssertAlreadyHaveCommands(name);

            var commands = new EntityCommands(Context, name);
            _commands.Add(name, commands);

            return commands;
        }

        public void RemoveCommands(EntityCommands commands)
        {
            Context.AssertContext();
            EntityCommands.AssertEntityCommands(commands, Context);

            AssertNotExistCommands(commands.Name);

            commands.InternalDestroy();
            _commands.Remove(commands.Name);
        }

        internal void InternalDestroy()
        {
            foreach (var commands in _commands.Values)
                commands.InternalDestroy();

            _commands.Clear();
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
