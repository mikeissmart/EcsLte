using EcsLte.Utilities;
using System.Runtime.InteropServices;

namespace EcsLte.NativeArcheType
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DataChunk_ArcheType_Native
    {
        [FieldOffset(0)]
        public int ChunkLengthInBytes;
        [FieldOffset(4)]
        public int Count;
        [FieldOffset(8)]
        public unsafe byte* Buffer;

        public bool IsFull(int capacity) => Count == capacity;

        public unsafe void Clear() => MemoryHelper.Clear(Buffer, ChunkLengthInBytes);
    }
}
