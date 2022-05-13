using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EcsLte.Utilities
{
    internal static class MemoryHelper
    {
        internal static unsafe void* Alloc(int lengthInBytes, bool clear = true)
        {
            if (lengthInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(lengthInBytes));

            var ptr = (void*)Marshal.AllocHGlobal(lengthInBytes);
            if (clear)
                Clear(ptr, lengthInBytes);

            return ptr;
        }

        internal static unsafe T* Alloc<T>(int count, bool clear = true) where T : unmanaged
            => (T*)Alloc(count * TypeCache<T>.SizeInBytes, clear);

        internal static unsafe void* Realloc(void* ptr, int newSizeInBytes, bool clear = true)
        {
            var newPtr = Alloc(newSizeInBytes);

            if (clear)
                Clear(newPtr, newSizeInBytes);

            Free(ptr);

            return newPtr;
        }

        internal static unsafe void* ReallocCopy(void* ptr, int oldSizeInBytes, int newSizeInBytes, bool clear = true)
        {
            var newPtr = Alloc(newSizeInBytes);

            if (clear && newSizeInBytes > oldSizeInBytes)
                Clear(newPtr, newSizeInBytes);

            Copy(ptr, newPtr, Math.Min(oldSizeInBytes, newSizeInBytes));
            Free(ptr);

            return newPtr;
        }


        internal static unsafe void Free(void* ptr)
           => Marshal.FreeHGlobal((IntPtr)ptr);

        internal static unsafe void Copy(void* sourcePtr, void* destinationPtr, int sizeInBytes)
            => Buffer.MemoryCopy(sourcePtr, destinationPtr, sizeInBytes, sizeInBytes);

        internal static unsafe void Clear(void* ptr, int lengthInBytes)
            => Unsafe.InitBlock(ptr, 0, (uint)lengthInBytes);
    }
}
