namespace EcsLte.Data
{
    internal unsafe struct MemoryPage
    {
        internal static int PageBufferSizeInBytes = 16384;

        internal byte* Buffer;
        internal int SlotSizeInBytes;
        internal int SlotCount;
        internal int SlotCapacity;
        internal bool IsFull => SlotCount == SlotCapacity;

        internal void Reset(int slotSizeInBytes, int slotCapacity)
        {
            SlotSizeInBytes = slotSizeInBytes;
            SlotCount = 0;
            SlotCapacity = slotCapacity;
        }

        internal byte* GetBuffer(int slotIndex) => Buffer + (slotIndex * SlotSizeInBytes);

        internal void SetSlot(int slotIndex, ref EntityData entityData)
        {
            var buffer = GetBuffer(slotIndex);
            entityData.Slot.Buffer = buffer;
            entityData.Slot.BlittableBuffer = buffer;
        }
    }
}
