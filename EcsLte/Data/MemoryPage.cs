namespace EcsLte.Data
{
    internal unsafe struct MemoryPage
    {
        internal static int PageBufferSizeInBytes = 16384;

        internal byte* Buffer;
        internal int SlotSizeInBytes;
        internal bool HasManaged;
        internal int ManagedOffsetInBytes;
        internal int SlotCount;
        internal int SlotCapacity;
        internal bool IsFull => SlotCount == SlotCapacity;

        internal void Reset(int slotSizeInBytes, int slotCapacity)
        {
            SlotSizeInBytes = slotSizeInBytes;
            HasManaged = false;
            ManagedOffsetInBytes = 0;
            SlotCount = 0;
            SlotCapacity = slotCapacity;
        }

        internal void Reset(int slotSizeInBytes, int slotCapacity, int managedOffset)
        {
            Reset(slotSizeInBytes, slotCapacity);
            HasManaged = true;
            ManagedOffsetInBytes = managedOffset;
        }

        internal byte* GetBuffer(int slotIndex) => Buffer + (slotIndex * SlotSizeInBytes);

        internal void SetSlot(int slotIndex, ref EntityData entityData)
        {
            var buffer = GetBuffer(slotIndex);
            entityData.Slot.Buffer = buffer;
            entityData.Slot.BlittableBuffer = buffer;
            entityData.Slot.ManagedBuffer = HasManaged
                    ? (int*)(buffer + ManagedOffsetInBytes)
                    : null;
        }
    }
}
