using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal struct EntityData
    {
        public static readonly EntityData Null = new EntityData();

        internal ArcheTypeIndex ArcheTypeIndex;
        internal int EntityIndex;
    }
}
