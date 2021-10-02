using System.Collections.Generic;

namespace EcsLte
{
    internal class EntityCommandQueueData
    {
        public EntityCommandQueueData()
        {
            Commands = new List<EntityCommand>();
        }

        public List<EntityCommand> Commands { get; }

        internal void Initialize()
        {
        }

        internal void Reset()
        {
            Commands.Clear();
        }
    }
}