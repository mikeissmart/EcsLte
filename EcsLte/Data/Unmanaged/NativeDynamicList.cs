using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte.Data.Unmanaged
{
	public unsafe struct NativeDynamicList : IDisposable
	{
		private NativeDynamicArray _array;
		private int _countInBytes;
		private int _capacityInBytes;

		public static NativeDynamicList Alloc<T>(int initialCount = 0, int initialCapacity = 4) where T : unmanaged
		{
			return Alloc(TypeCache<T>.SizeInBytes, initialCount, initialCapacity);
		}

		public static NativeDynamicList Alloc(int itemSizeInBytes, int initialCount = 0, int initialCapacity = 4)
		{
			if (initialCount < 0)
				throw new ArgumentOutOfRangeException(nameof(initialCount), "Must be greater than 0.");
			if (initialCapacity <= 0)
				throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Must be greater than 0.");
			if (itemSizeInBytes <= 0)
				throw new ArgumentOutOfRangeException(nameof(itemSizeInBytes), "Must be greater than 0.");
			if (initialCapacity < initialCount)
				throw new ArgumentException("Must Capacity be greater than Count.");

			var list = new NativeDynamicList
			{
				_array = NativeDynamicArray.Alloc(itemSizeInBytes * initialCapacity),
				_countInBytes = initialCount * itemSizeInBytes,
				_capacityInBytes = initialCapacity * itemSizeInBytes
			};

			return list;
		}

		public void Add<T>(ref T item) where T : unmanaged
        {
			var itemSizeInBytes = TypeCache<T>.SizeInBytes;
			if (_countInBytes + itemSizeInBytes > _capacityInBytes)
            {
				var newLengthInBytes = (int)Math.Pow(2, (int)Math.Log(_countInBytes + itemSizeInBytes, 2) + 1);
				_array.Resize(newLengthInBytes);
			}
			_array.Set(_countInBytes, ref item);
			_countInBytes += itemSizeInBytes;
		}

		public T Get<T>(int offsetInBytes) where T : unmanaged
		{
			var itemSizeInBytes = TypeCache<T>.SizeInBytes;
			if (offsetInBytes <= 0)
				throw new ArgumentOutOfRangeException(nameof(offsetInBytes), "Must be greater than 0.");
			if (offsetInBytes + itemSizeInBytes > _countInBytes)
				throw new ArgumentOutOfRangeException(nameof(offsetInBytes));

			return _array.Get<T>(offsetInBytes);
		}

		/*public int IndexOf<T>(ref T item) where T : unmanaged
		{
			var itemSizeInBytes = TypeCache<T>.SizeInBytes;
			var itemCount = Count<T>();
			for (var i = 0; i < itemCount; i++)
            {
				if (_array.Get<T>(i * itemSizeInBytes).Equals(item))
					return i;
            }

			return -1;
		}

		public int IndexOf(byte* ptr, int itemSizeInBytes)
		{
			var index = -1;
			for (var i = 0; i < _countInBytes; i += itemSizeInBytes)
			{
				index = i;
				var checkPtr = _array.Get(i);
				for (var j = 0; j < itemSizeInBytes; j++)
                {
					if (*(checkPtr + j) != *(ptr + j))
                    {
						index = -1;
						break;
                    }
                }

				if (index != -1)
					break;
			}

			return index;
		}

		public bool Remove<T>(ref T item) where T : unmanaged
		{
			if (Count<T>() > 0)
			{
				var index = IndexOf(ref item);
				if (index >= 0)
				{
					RemoveAt<T>(index);
					return true;
				}
			}

			return false;
		}

		public bool Remove(byte* ptr, int itemSizeInBytes)
        {
			if (Count(itemSizeInBytes) > 0)
			{
				var index = IndexOf(ptr, itemSizeInBytes);
				if (index >= 0)
				{
					RemoveAt(index, itemSizeInBytes);
					return true;
				}
			}

			return false;
		}

		public void RemoveAt<T>(int index) where T : unmanaged
		{
			RemoveAtRange<T>(index, 1);
		}

		public void RemoveAt(int index, int itemSizeInBytes)
		{
			RemoveAtRange(index, itemSizeInBytes, 1);
		}

		public void RemoveAtRange<T>(int index, int length) where T : unmanaged
		{
			RemoveAtRange(index, length, TypeCache<T>.SizeInBytes);
		}

		public void RemoveAtRange(int index, int length, int itemSizeInBytes)
        {
			var count = Count(itemSizeInBytes);
			if (index < 0 || index > count ||
				length < 0 || index + length > count)
				throw new IndexOutOfRangeException();

			var bytesToCopy = (Capacity(itemSizeInBytes) - length - index) * itemSizeInBytes;
			if (bytesToCopy > 0)
				MemoryHelper.CopyBlock(ref _array.Ptr, (index + length) * itemSizeInBytes, index * itemSizeInBytes, bytesToCopy);
			_countInBytes -= length * itemSizeInBytes;
		}

		public ref T Pop<T>() where T : unmanaged
		{
			var itemSizeInBytes = TypeCache<T>.SizeInBytes;
			var count = Count<T>();
			if (count == 0)
				throw new IndexOutOfRangeException();

			_countInBytes -= itemSizeInBytes;
			return ref _array.Get<T>(count * itemSizeInBytes);
		}*/

		public int Count<T>() where T : unmanaged
		{
			if (_countInBytes == 0)
				return 0;

			return TypeCache<T>.SizeInBytes / _countInBytes;
		}

		public int Count(int itemSizeInBytes)
		{
			if (_countInBytes == 0)
				return 0;

			return itemSizeInBytes / _countInBytes;
		}

		public int Capacity<T>() where T : unmanaged
		{
			if (_capacityInBytes == 0)
				return 0;

			return TypeCache<T>.SizeInBytes / _capacityInBytes;
		}

		public int Capacity(int itemSizeInBytes)
		{
			if (_capacityInBytes == 0)
				return 0;

			return itemSizeInBytes / _capacityInBytes;
		}

		public void Clear()
        {
			if (_countInBytes > 0)
            {
				_array.Clear();
				_countInBytes = 0;
            }
        }

        public void Dispose()
		{
			_array.Dispose();
			_countInBytes = 0;
			_capacityInBytes = 0;
		}
    }
}
