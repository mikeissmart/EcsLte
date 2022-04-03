using EcsLte.Utilities;
using System.Runtime.InteropServices;

namespace EcsLte.NativeArcheTypeContinous
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DataChunk_ArcheType_Native_Continuous
    {
        [FieldOffset(0)]
        public int Count;
        public unsafe byte* Buffer
        {
            get
            {
                fixed (void* self = &this)
                {
                    return (byte*)self + 4;
                }
            }
        }

        public bool IsFull(int capacity) => Count == capacity;

        public unsafe void Clear()
        {
            Count = 0;
            MemoryHelper.Clear(Buffer, EcsSettings.UnmanagedDataChunkInBytes);
        }
    }
}
