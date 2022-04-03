using EcsLte.Data.Unmanaged;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EcsLte.Utilities
{
    public static class MemoryHelper
    {
        public static unsafe void* Alloc(int lengthInBytes, bool clear = true)
        {
            if (lengthInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(lengthInBytes));

            var ptr = (void*)Marshal.AllocHGlobal(lengthInBytes);
            if (clear)
                Clear(ptr, lengthInBytes);

            return ptr;
        }


        public static unsafe T* Alloc<T>(int count, bool clear = true) where T : unmanaged => (T*)Alloc(count * TypeCache<T>.SizeInBytes, clear);


        public static unsafe void* Realloc(void* ptr, int oldLenthInBytes, int newLengthInBytes, bool clearBeforeCopy = true)
        {
            var newPtr = Alloc(newLengthInBytes);

            if (clearBeforeCopy && newLengthInBytes > oldLenthInBytes)
                Clear(newPtr, newLengthInBytes);

            Copy(ptr, newPtr, Math.Min(oldLenthInBytes, newLengthInBytes));
            Free(ptr);

            return newPtr;
        }


        public static unsafe void Free(void* ptr) => Marshal.FreeHGlobal((IntPtr)ptr);


        public static unsafe void CopyBlock(void* ptr, int sourceOffset, int destinationOffset, int sizeInBytes) => Unsafe.CopyBlock((byte*)ptr + destinationOffset, (byte*)ptr + sourceOffset, (uint)sizeInBytes);

        public static unsafe void Copy(void* sourcePtr, void* destinationPtr, int sizeInBytes) => Buffer.MemoryCopy(sourcePtr, destinationPtr, sizeInBytes, sizeInBytes);

        public static unsafe void Clear(void* ptr, int lengthInBytes) => Unsafe.InitBlock(ptr, 0, (uint)lengthInBytes);
    }

    public static class MemoryHelperSafePtr
    {
        public static unsafe SafePtr Alloc(int lengthInBytes, bool clear = true)
        {
            if (lengthInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(lengthInBytes));

            var ptr = new SafePtr
            {
                Ptr = (void*)Marshal.AllocHGlobal(lengthInBytes),
                LengthInBytes = lengthInBytes
            };
            if (clear)
                Clear(ptr, lengthInBytes);

            return ptr;
        }


        public static unsafe SafePtr Alloc<T>(int count, bool clear = true) where T : unmanaged => Alloc(count * TypeCache<T>.SizeInBytes, clear);


        public static unsafe SafePtr Realloc(ref SafePtr ptr, int oldLenthInBytes, int newLengthInBytes, bool clearBeforeCopy = true)
        {
            var newPtr = Alloc(newLengthInBytes);

            if (clearBeforeCopy && newLengthInBytes > oldLenthInBytes)
                Clear(newPtr, newLengthInBytes);

            Copy(ptr, newPtr, Math.Min(oldLenthInBytes, newLengthInBytes));
            Free(ref ptr);

            return newPtr;
        }


        public static unsafe void Free(ref SafePtr ptr)
        {
            if (ptr.Ptr == null || ptr.LengthInBytes == 0)
                throw new ArgumentNullException(nameof(ptr));

            Marshal.FreeHGlobal((IntPtr)ptr.Ptr);
            ptr.LengthInBytes = 0;
        }


        public static unsafe void CopyBlock(SafePtr ptr, int sourceOffset, int destinationOffset, int sizeInBytes)
        {
            if (ptr.Ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (sourceOffset + sizeInBytes > ptr.LengthInBytes ||
                destinationOffset + sizeInBytes > ptr.LengthInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(ptr));
            }

            Unsafe.CopyBlock((byte*)ptr.Ptr + destinationOffset, (byte*)ptr.Ptr + sourceOffset, (uint)sizeInBytes);
        }

        public static unsafe void Copy(SafePtr sourcePtr, SafePtr destinationPtr, int sizeInBytes)
        {
            if (sourcePtr.Ptr == null)
                throw new ArgumentNullException(nameof(sourcePtr));
            if (destinationPtr.Ptr == null)
                throw new ArgumentNullException(nameof(destinationPtr));
            if (sizeInBytes > sourcePtr.LengthInBytes)
                throw new ArgumentOutOfRangeException(nameof(sourcePtr));
            if (sizeInBytes > destinationPtr.LengthInBytes)
                throw new ArgumentOutOfRangeException(nameof(destinationPtr));

            Buffer.MemoryCopy(sourcePtr.Ptr, destinationPtr.Ptr, sizeInBytes, sizeInBytes);
        }

        public static unsafe void Clear(SafePtr ptr, int lengthInBytes)
        {
            if (ptr.Ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (lengthInBytes > ptr.LengthInBytes)
                throw new ArgumentOutOfRangeException(nameof(ptr));
            Unsafe.InitBlock(ptr.Ptr, 0, (uint)lengthInBytes);
        }
    }
}
