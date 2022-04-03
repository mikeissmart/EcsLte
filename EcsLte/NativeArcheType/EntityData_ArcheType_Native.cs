using System.Runtime.InteropServices;

namespace EcsLte.NativeArcheType
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EntityData_ArcheType_Native
    {
        public unsafe ComponentData_ArcheType_Native* ComponentArcheTypeData { get; set; }
        public unsafe DataChunk_ArcheType_Native* DataChunk { get; set; }
        public int Index { get; set; }

        public unsafe void Clear()
        {
            ComponentArcheTypeData = null;
            DataChunk = null;
            Index = -1;
        }
    }
}
