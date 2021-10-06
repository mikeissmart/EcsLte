using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityCommandQueueData
    {
        public EntityCommandQueueData()
        {
            Commands = new List<EntityCommand>();
        }

        internal List<EntityCommand> Commands { get; }
        internal EcsContextData ContextData { get; private set; }
        internal string Name { get; private set; }

        internal static EntityCommandQueueData Initialize(EcsContextData contextData, string name)
        {
            var data = ObjectCache<EntityCommandQueueData>.Pop();

            data.ContextData = contextData;
            data.Name = name;

            return data;
        }

        internal static void Uninitialize(EntityCommandQueueData data)
        {
            data.Commands.Clear();
            // data.ContextData;
            // data.Name;

            ObjectCache<EntityCommandQueueData>.Push(data);
        }
    }
}