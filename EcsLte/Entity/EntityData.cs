using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal struct EntityData
    {
        internal unsafe ArcheTypeData* ArcheTypeData { get; set; }
        internal int EntityIndex { get; set; }
    }
}
