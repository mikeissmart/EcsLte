using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EcsLte.Data.Unmanaged
{
    public unsafe struct NativeArrayPtr : IDisposable
    {
        private byte* _ptr;

        public int Length { get; private set; }
        public int TypeHash { get; private set; }
        public int ItemSize { get; private set; }

        public static NativeArrayPtr Alloc<T>(int length) where T : unmanaged
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var lengthInBytes = TypeCache<T>.SizeInBytes * length;
            var ptr = (byte*)MemoryHelper.Alloc(lengthInBytes);
            Unsafe.InitBlock(ptr, 0, (uint)lengthInBytes);

            var array = new NativeArrayPtr
            {
                _ptr = ptr,
                Length = length,
                TypeHash = TypeCache<T>.HashCode,
                ItemSize = TypeCache<T>.SizeInBytes,
            };

            return array;
        }

        public static NativeArrayPtr Alloc<T>(IEnumerable<T> items) where T : unmanaged
        {
            var length = items.Count();
            var lengthInBytes = TypeCache<T>.SizeInBytes * length;
            var ptr = (byte*)MemoryHelper.Alloc(lengthInBytes);
            Unsafe.InitBlock(ptr, 0, (uint)lengthInBytes);

            var array = new NativeArrayPtr
            {
                _ptr = ptr,
                Length = length,
                TypeHash = TypeCache<T>.HashCode,
                ItemSize = TypeCache<T>.SizeInBytes,
            };

            fixed (T* itemPtr = items.ToArray())
            {
                Buffer.MemoryCopy(itemPtr, ptr, lengthInBytes, lengthInBytes);
            }

            return array;
        }

        public void Set<T>(int index, T item) where T : unmanaged
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");

            ((T*)_ptr)[index] = item;
        }

        public ref T Get<T>(int index) where T : unmanaged
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");

            return ref ((T*)_ptr)[index];
        }

        public int IndexOf<T>(in T item) where T : unmanaged
        {
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");

            var index = -1;
            if (Length > 0)
            {
                for (var i = 0; i < Length; i++)
                {
                    if (((T*)_ptr)[i].Equals(item))
                        return i;
                }
            }

            return index;
        }

        public void Resize<T>(int newLength) where T : unmanaged
        {
            if (newLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(newLength));
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");
            if (_ptr == null)
                throw new InvalidOperationException("Not created with Alloc or is disposed.");

            var oldLengthInBytes = ItemSize * Length;
            var newLengthInBytes = ItemSize * newLength;
            var newPtr = (byte*)MemoryHelper.Alloc(newLengthInBytes);

            if (newLength > Length)
                Unsafe.InitBlock(newPtr + oldLengthInBytes, 0, (uint)(newLengthInBytes - oldLengthInBytes));

            Buffer.MemoryCopy(_ptr, newPtr, newLengthInBytes, Math.Min(oldLengthInBytes, newLengthInBytes));
            Marshal.FreeHGlobal((IntPtr)_ptr);

            _ptr = newPtr;
            Length = newLength;
        }

        public void CopyTo(int sourceIndex, ref NativeArrayPtr destination, int destinationIndex, int length)
        {
            if (TypeHash != destination.TypeHash)
                throw new InvalidOperationException("Types do not match.");

            if (length <= 0 ||
                sourceIndex < 0 || sourceIndex >= Length || sourceIndex + length > Length ||
                destinationIndex < 0 || destinationIndex >= destination.Length || destinationIndex + length > destination.Length)
            {
                throw new IndexOutOfRangeException();
            }

            var bytesToCopy = length * ItemSize;
            Buffer.MemoryCopy(
                _ptr + (sourceIndex * ItemSize),
                destination._ptr + (destinationIndex * ItemSize),
                destination.Length * ItemSize,
                bytesToCopy);
        }

        public T[] ToManagedArray<T>() where T : unmanaged
        {
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException(typeof(T).FullName);

            var bytesToCopy = Length * ItemSize;
            var array = new T[Length];
            fixed (void* ptr = array)
            {
                Buffer.MemoryCopy(_ptr, ptr, bytesToCopy, bytesToCopy);
            }

            return array;
        }

        public void Clear() => Unsafe.InitBlock(_ptr, 0, (uint)(Length * ItemSize));

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)_ptr);
            Length = 0;
            TypeHash = 0;
            ItemSize = 0;
        }
    }

    public unsafe struct NativeArray : IDisposable
    {
        private NativeDynamicArray _array;

        public byte* Ptr => _array.Ptr;
        public int Length => ItemSize == 0
                ? 0
                : _array.LengthInBytes / ItemSize;
        public int TypeHash { get; private set; }
        public int ItemSize { get; private set; }

        public static NativeArray Alloc<T>(int length) where T : unmanaged
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var array = new NativeArray
            {
                _array = NativeDynamicArray.Alloc(length * TypeCache<T>.SizeInBytes),
                TypeHash = TypeCache<T>.HashCode,
                ItemSize = TypeCache<T>.SizeInBytes,
            };

            return array;
        }

        public static NativeArray Alloc<T>(IEnumerable<T> items) where T : unmanaged
        {
            var length = items.Count();
            var array = new NativeArray
            {
                _array = NativeDynamicArray.Alloc(length * TypeCache<T>.SizeInBytes),
                TypeHash = TypeCache<T>.HashCode,
                ItemSize = TypeCache<T>.SizeInBytes,
            };

            fixed (T* ptr = items.ToArray())
            {
                MemoryHelper.Copy(ptr, array.Ptr, length * TypeCache<T>.SizeInBytes);
            }

            return array;
        }

        public void Set<T>(int index, in T item) where T : unmanaged
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");

            *(T*)(Ptr + (index * ItemSize)) = item;
        }

        public ref T Get<T>(int index) where T : unmanaged
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");

            return ref *(T*)(Ptr + (index * ItemSize));
        }

        public int IndexOf<T>(in T item) where T : unmanaged
        {
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");

            var index = -1;
            if (Length > 0)
            {
                for (var i = 0; i < Length; i++)
                {
                    if ((*(T*)(Ptr + (i * ItemSize))).Equals(item))
                        return i;
                }
            }

            return index;
        }

        public void Resize<T>(int newLength) where T : unmanaged
        {
            if (newLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(newLength));
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException("Types do not match.");
            if (_array.LengthInBytes == 0)
                throw new InvalidOperationException("Not created with Alloc or is disposed.");

            _array.Resize(newLength * ItemSize);
        }

        public void CopyTo(int sourceIndex, ref NativeArray destination, int destinationIndex, int length)
        {
            if (TypeHash != destination.TypeHash)
                throw new InvalidOperationException("Types do not match.");

            if (length <= 0 ||
                sourceIndex < 0 || sourceIndex >= Length || sourceIndex + length > Length ||
                destinationIndex < 0 || destinationIndex >= destination.Length || destinationIndex + length > destination.Length)
            {
                throw new IndexOutOfRangeException();
            }

            MemoryHelper.Copy(
                Ptr + (sourceIndex * ItemSize),
                destination.Ptr + (destinationIndex * ItemSize),
                length * ItemSize);
        }

        public T[] ToManagedArray<T>() where T : unmanaged
        {
            if (TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException(typeof(T).FullName);

            var array = new T[Length];
            fixed (void* ptr = array)
            {
                MemoryHelper.Copy(Ptr, ptr, Length * ItemSize);
            }

            return array;
        }

        public void Clear() => _array.Clear();

        public void Dispose()
        {
            _array.Dispose();
            TypeHash = 0;
            ItemSize = 0;
        }
    }
}
