using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.ManagedArcheType
{
    internal class EntityBlueprintData_ArcheType_Managed
    {
        public ComponentConfig[] Configs { get; set; }
        public ComponentConfig[] UniqueConfigs { get; set; }
        public IComponent[] Components { get; set; }
        public IComponent[] UniqueComponents { get; set; }
    }
}
