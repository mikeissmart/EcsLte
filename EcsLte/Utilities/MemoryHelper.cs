using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EcsLte.Utilities
{
    internal static class MemoryHelper
    {

        internal static unsafe T* Alloc<T>(int count, bool clear = true) where T : unmanaged
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var ptr = (T*)Marshal.AllocHGlobal(count * TypeCache<T>.SizeInBytes);
            if (clear)
                Clear(ptr, count);

            return ptr;
        }

        internal static unsafe T* ReallocCopy<T>(T* ptr, int oldCount, int newCount, bool clear = true) where T : unmanaged
        {
            var newPtr = Alloc<T>(newCount);

            if (clear && newCount > oldCount)
                Clear(newPtr, newCount);

            Copy(ptr, newPtr, Math.Min(oldCount, newCount));
            Free(ptr);

            return newPtr;
        }

        internal static unsafe void Free(void* ptr)
            => Marshal.FreeHGlobal((IntPtr)ptr);

        internal static unsafe void Copy<T>(T* sourcePtr, T* destinationPtr, int count) where T : unmanaged
            => Buffer.MemoryCopy(sourcePtr, destinationPtr, count * TypeCache<T>.SizeInBytes, count * TypeCache<T>.SizeInBytes);

        internal static unsafe void Clear<T>(T* ptr, int count) where T : unmanaged
            => Unsafe.InitBlock(ptr, 0, (uint)(count * TypeCache<T>.SizeInBytes));
    }
}