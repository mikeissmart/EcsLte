using System.Runtime.InteropServices;

namespace EcsLte.NativeArcheTypeContinous
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EntityData_ArcheType_Native_Continuous
    {
        public unsafe ComponentData_ArcheType_Native_Continuous* ComponentArcheTypeData { get; set; }
        public unsafe int ChunkIndex { get; set; }
        public int DataChunkIndex { get; set; }
        public int EntityIndex { get; set; }

        public unsafe void Clear()
        {
            ComponentArcheTypeData = null;
            ChunkIndex = -1;
            DataChunkIndex = -1;
            EntityIndex = -1;
        }
    }
}
