using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal class EntityFilterContextData
    {
        internal ArcheTypeData[] ArcheTypeDatas { get; set; } = new ArcheTypeData[0];
        internal int? ArcheTypeChangeVersion { get; set; }
    }
}
