using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class EntityCommandsIsDestroyedException : EcsLteException
    {
        public EntityCommandsIsDestroyedException(EntityCommands commands)
            : base($"EntityCommands '{commands.Name}' is already destroyed.")
        {
        }
    }
}
