using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public class EntityCommands
    {
        public EcsContext Context { get; private set; }
        public string Name { get; private set; }
        public bool IsDestroyed { get; set; }

        internal EntityCommands(EcsContext context, string name)
        {
            Context = context;
            Name = name;
        }

        internal void InternalDestroy()
        {
            IsDestroyed = true;
        }

        #region Assert

        internal static void AssertEntityCommands(EntityCommands commands, EcsContext context)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));
            if (commands.IsDestroyed)
                throw new EntityCommandsIsDestroyedException(commands);
            if (commands.Context != context)
                throw new EcsContextNotSameException(commands.Context, context);
        }

        #endregion
    }
}
