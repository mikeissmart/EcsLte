using EcsLte.Utilities;
using System;

namespace EcsLte.Data.Unmanaged
{
    public unsafe struct NativeDynamicArray : IDisposable
    {
        public byte* Ptr;
        public int LengthInBytes { get; private set; }

        public static NativeDynamicArray Alloc<T>(int length) where T : unmanaged => Alloc(TypeCache<T>.SizeInBytes * length);

        public static NativeDynamicArray Alloc(int lengthInBytes)
        {
            if (lengthInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(lengthInBytes));

            var array = new NativeDynamicArray
            {
                Ptr = (byte*)MemoryHelper.Alloc(lengthInBytes),
                LengthInBytes = lengthInBytes
            };

            return array;
        }

        public void Set<T>(int offsetInBytes, ref T item) where T : unmanaged
        {
            var itemSizeInBytes = TypeCache<T>.SizeInBytes;
            if (offsetInBytes < 0 || offsetInBytes * itemSizeInBytes > LengthInBytes)
                throw new ArgumentOutOfRangeException(nameof(offsetInBytes));

            fixed (T* ptr = &item)
            {
                MemoryHelper.Copy(
                    ptr,
                    Ptr + offsetInBytes,
                    offsetInBytes);
            }
        }

        public T Get<T>(int offsetInBytes) where T : unmanaged
        {
            var itemSizeInBytes = TypeCache<T>.SizeInBytes;
            if (offsetInBytes < 0 || offsetInBytes * itemSizeInBytes > LengthInBytes)
                throw new ArgumentOutOfRangeException(nameof(offsetInBytes));

            return *(T*)(Ptr + offsetInBytes + itemSizeInBytes);
        }

        public void Resize(int newLengthInBytes)
        {
            if (LengthInBytes == 0)
                Ptr = (byte*)MemoryHelper.Alloc(newLengthInBytes);
            else
                Ptr = (byte*)MemoryHelper.Realloc(Ptr, LengthInBytes, newLengthInBytes);
            LengthInBytes = newLengthInBytes;
        }

        public void CopyTo(int sourceOffsetInBytes, ref NativeDynamicArray destination, int destinationOffsetInBytes, int lengthInBytes)
        {
            if (lengthInBytes <= 0 ||
                sourceOffsetInBytes < 0 || sourceOffsetInBytes >= LengthInBytes || sourceOffsetInBytes + lengthInBytes > LengthInBytes ||
                destinationOffsetInBytes < 0 || destinationOffsetInBytes >= destination.LengthInBytes || destinationOffsetInBytes + lengthInBytes > destination.LengthInBytes)
            {
                throw new ArgumentOutOfRangeException();
            }

            MemoryHelper.Copy(Ptr + sourceOffsetInBytes, destination.Ptr + destinationOffsetInBytes, lengthInBytes);
        }

        public void Clear()
        {
            if (LengthInBytes > 0)
            {
                MemoryHelper.Clear(Ptr, LengthInBytes);
            }
        }

        public void Dispose()
        {
            if (LengthInBytes != 0)
            {
                MemoryHelper.Free(Ptr);
                LengthInBytes = 0;
            }
        }
    }
}
