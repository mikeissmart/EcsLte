using System.Collections.Generic;

namespace EcsLte
{
    internal class EntityCommandQueueData
    {
        public List<EntityCommand> Commands { get; private set; }

        public EntityCommandQueueData()
        {
            Commands = new List<EntityCommand>();
        }

        internal void Initialize()
        {

        }

        internal void Reset()
        {
            Commands.Clear();
        }
    }
}