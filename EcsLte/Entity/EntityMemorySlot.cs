namespace EcsLte
{
    internal unsafe struct EntityMemorySlot
    {
        internal byte* Buffer;
        internal byte* BlittableBuffer;
        internal int* ManagedBuffer;
    }
}
