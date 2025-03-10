﻿namespace EcsLte.Exceptions
{
    public class EntityCommandsIsDestroyedException : EcsLteException
    {
        public EntityCommandsIsDestroyedException(EntityCommands commands)
            : base($"EntityCommands '{commands.Name}' is destroyed.")
        {
        }
    }
}
