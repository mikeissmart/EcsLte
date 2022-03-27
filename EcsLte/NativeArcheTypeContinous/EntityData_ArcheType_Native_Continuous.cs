using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte.NativeArcheTypeContinous
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EntityData_ArcheType_Native_Continuous
    {
        public unsafe ComponentData_ArcheType_Native_Continuous* ComponentArcheTypeData { get; set; }
        public unsafe int DataChunkIndex { get; set; }
        public int Index { get; set; }

        public unsafe void Clear()
        {
            ComponentArcheTypeData = null;
            DataChunkIndex = -1;
            Index = -1;
        }
    }
}
