using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.Data.Unmanaged
{
    public unsafe struct NativeList : IDisposable
    {
        private NativeArray _array;

        public int Count { get; private set; }
        public int Capacity => _array.Length;
        public int TypeHash => _array.TypeHash;
        public int ItemSize => _array.ItemSize;

        public static NativeList Alloc<T>(int initialCapacity = 4) where T : unmanaged => new NativeList
        {
            _array = NativeArray.Alloc<T>(initialCapacity),
            Count = 0
        };

        public static NativeList Alloc<T>(IEnumerable<T> items) where T : unmanaged
        {
            var itemLength = items.Count();
            var length = (int)Math.Pow(2, (int)Math.Log(itemLength, 2) + 1);
            var list = new NativeList
            {
                _array = NativeArray.Alloc<T>(length),
                Count = itemLength
            };

            fixed (T* ptr = items.ToArray())
            {
                MemoryHelper.Copy(ptr, list._array.Ptr, length * TypeCache<T>.SizeInBytes);
            }

            return list;
        }

        public void Add<T>(in T item) where T : unmanaged
        {
            if (_array.Length == 0)
                _array = NativeArray.Alloc<T>(4);
            else if (Count == Capacity)
                _array.Resize<T>(Capacity * 2);

            _array.Set(Count, item);
            Count++;
        }

        public void Set<T>(int index, T item) where T : unmanaged
        {
            if (index >= Count)
                throw new IndexOutOfRangeException(nameof(index));

            _array.Set(index, item);
        }

        public T Get<T>(int index) where T : unmanaged
        {
            if (index >= Count)
                throw new IndexOutOfRangeException(nameof(index));

            return _array.Get<T>(index);
        }

        public int IndexOf<T>(in T item) where T : unmanaged => _array.IndexOf(item);

        public bool Remove<T>(in T item) where T : unmanaged
        {
            if (_array.Length > 0)
            {
                var index = IndexOf(item);
                if (index >= 0)
                {
                    RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        public T Pop<T>() where T : unmanaged
        {
            if (Count == 0)
                throw new IndexOutOfRangeException();

            Count--;
            return _array.Get<T>(Count);
        }

        public void RemoveAt(int index) => RemoveAtRange(index, 1);

        public void RemoveAtRange(int index, int length)
        {
            if (index < 0 || index >= Count ||
                length == 0 || index + length > Count)
            {
                throw new IndexOutOfRangeException();
            }

            var bytesToCopy = (Capacity - length - index) * _array.ItemSize;
            if (bytesToCopy > 0)
                MemoryHelper.CopyBlock(_array.Ptr, (index + length) * _array.ItemSize, index * _array.ItemSize, bytesToCopy);
            Count -= length;
        }

        public NativeArray ToNativeArray<T>() where T : unmanaged
        {
            if (_array.TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException(typeof(T).FullName);

            var array = NativeArray.Alloc<T>(Count);
            _array.CopyTo(0, ref array, 0, Count);

            return array;
        }

        public T[] ToManagedArray<T>() where T : unmanaged
        {
            if (_array.TypeHash != TypeCache<T>.HashCode)
                throw new InvalidOperationException(typeof(T).FullName);

            var array = new T[Count];
            fixed (void* ptr = array)
            {
                MemoryHelper.Copy(_array.Ptr, ptr, Count * _array.ItemSize);
            }

            return array;
        }

        public void Clear()
        {
            _array.Clear();
            Count = 0;
        }

        public void Dispose()
        {
            _array.Dispose();
            Count = 0;
        }
    }
}
