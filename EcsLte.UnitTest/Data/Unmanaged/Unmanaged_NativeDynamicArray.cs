using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EcsLte.Data.Unmanaged;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.Data.Unmanaged
{
	/*[TestClass]
	public class Unmanaged_NativeDynamicArray
	{
		private struct Item1
		{
			public int Id { get; set; }
		}

		private struct Item2
		{
			public int Id { get; set; }
			public int Id2 { get; set; }
		}

		private readonly int _arrayLength = 4;
		private readonly int _item1Size = Marshal.SizeOf(typeof(Item1));
		private readonly int _item2Size = Marshal.SizeOf(typeof(Item2));
		private int _lengthInBytes => _item1Size + _item2Size;
		private NativeDynamicArray _array;

		[TestInitialize]
		public void Init()
		{
			_array = NativeDynamicArray.Alloc(_lengthInBytes * _arrayLength);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_array.Dispose();
		}

		private void PopulateNativeArray(ref NativeDynamicArray array)
		{
			var offsetBytes = 0;
			var nextId = 1;
			for (int i = 0; i < _arrayLength; i++)
			{
				var item1 = new Item1 { Id = nextId++ };
				var item2 = new Item2 { Id = nextId, Id2 = nextId + 1 };

				array.Set(offsetBytes, ref item1);
				offsetBytes += _item1Size;
				array.Set(offsetBytes, ref item2);
				offsetBytes += _item2Size;
				nextId += 2;
			}
		}

		private void AreEqualItem1(ref NativeDynamicArray array, ref Item1 item, int index)
		{
			var offsetInBytes = (_item1Size + _item2Size) * index;

			Assert.AreEqual(item, _array.Get<Item1>(offsetInBytes));
		}

		private void AreEqualItem2(ref NativeDynamicArray array, ref Item2 item, int index)
		{
			var offsetInBytes = ((_item1Size + _item2Size) * index) + _item1Size;

			Assert.AreEqual(item, _array.Get<Item2>(offsetInBytes));
		}

		private Item1 GetItem1(ref NativeDynamicArray array, int index)
		{
			var offsetInBytes = (_item1Size + _item2Size) * index;
			return array.Get<Item1>(offsetInBytes);
		}

		private Item2 GetItem2(ref NativeDynamicArray array, int index)
		{
			var offsetInBytes = ((_item1Size + _item2Size) * index) + _item1Size;
			return array.Get<Item2>(offsetInBytes);
		}

		[TestMethod]
		public void Alloc()
		{
			var array = NativeDynamicArray.Alloc(_lengthInBytes * _arrayLength);

			Assert.AreEqual(_lengthInBytes * _arrayLength, array.LengthInBytes);

			array.Dispose();
		}

		[TestMethod]
		public void SetGetItems()
		{
			var item1 = new Item1 { Id = 1 };
			var item2 = new Item2 { Id = 2, Id2 = 3 };

			_array.Set(0, ref item1);
			_array.Set(_item1Size, ref item2);

			AreEqualItem1(ref _array, ref item1, 0);
			AreEqualItem2(ref _array, ref item2, 0);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => _array.Get<Item1>((_lengthInBytes * _arrayLength) + 1));
		}

		[TestMethod]
		public void Resize_Bigger()
		{
			PopulateNativeArray(ref _array);

			_array.Resize(_lengthInBytes * (_arrayLength * 2));

			var nextId = 1;
			for (int i = 0; i < _arrayLength; i++)
			{
				var item1 = new Item1 { Id = nextId };
				var item2 = new Item2 { Id = nextId + 1, Id2 = nextId + 2 };
				nextId += 3;

				var i1 = GetItem1(ref _array, i);
				var i2 = GetItem2(ref _array, i);
				Assert.AreEqual(item1, GetItem1(ref _array, i));
				Assert.AreEqual(item2, GetItem2(ref _array, i));
			}

			var item1Def = new Item1();
			var item2Def = new Item2();
			for (int i = _arrayLength; i < _arrayLength * 2; i++)
			{
				Assert.AreEqual(item1Def, GetItem1(ref _array, i));
				Assert.AreEqual(item2Def, GetItem2(ref _array, i));
			}
		}

		[TestMethod]
		public void Resize_Smaller()
		{
			PopulateNativeArray(ref _array);

			_array.Resize(_lengthInBytes * (_arrayLength / 2));

			var nextId = 1;
			for (int i = 0; i < _arrayLength / 2; i++)
			{
				var item1 = new Item1 { Id = nextId };
				var item2 = new Item2 { Id = nextId + 1, Id2 = nextId + 2 };
				nextId += 3;

				Assert.AreEqual(item1, GetItem1(ref _array, i));
				Assert.AreEqual(item2, GetItem2(ref _array, i));
			}
		}

		[TestMethod]
		public void CopyTo_All()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(0, ref array2, 0, _arrayLength * _lengthInBytes);

			for (int i = 0; i < _arrayLength; i++)
			{
				Assert.AreEqual(GetItem1(ref _array, i), GetItem1(ref array2, i));
				Assert.AreEqual(GetItem2(ref _array, i), GetItem2(ref array2, i));
			}

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Beginning_Beginning()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(0, ref array2, 0, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(GetItem1(ref _array, 0), GetItem1(ref array2, 0));
			Assert.AreEqual(GetItem2(ref _array, 0), GetItem2(ref array2, 0));
			Assert.AreEqual(GetItem1(ref _array, 1), GetItem1(ref array2, 1));
			Assert.AreEqual(GetItem2(ref _array, 1), GetItem2(ref array2, 1));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 2));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 2));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 3));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Middle_Beginning()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(1 * _lengthInBytes, ref array2, 0, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(GetItem1(ref _array, 1), GetItem1(ref array2, 0));
			Assert.AreEqual(GetItem2(ref _array, 1), GetItem2(ref array2, 0));
			Assert.AreEqual(GetItem1(ref _array, 2), GetItem1(ref array2, 1));
			Assert.AreEqual(GetItem2(ref _array, 2), GetItem2(ref array2, 1));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 2));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 2));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 3));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_End_Beginning()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(2 * _lengthInBytes, ref array2, 0, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(GetItem1(ref _array, 2), GetItem1(ref array2, 0));
			Assert.AreEqual(GetItem2(ref _array, 2), GetItem2(ref array2, 0));
			Assert.AreEqual(GetItem1(ref _array, 3), GetItem1(ref array2, 1));
			Assert.AreEqual(GetItem2(ref _array, 3), GetItem2(ref array2, 1));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 2));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 2));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 3));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Beginning_Middle()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(0, ref array2, 1 * _lengthInBytes, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(item1Def, GetItem1(ref array2, 0));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 0));
			Assert.AreEqual(GetItem1(ref _array, 0), GetItem1(ref array2, 1));
			Assert.AreEqual(GetItem2(ref _array, 0), GetItem2(ref array2, 1));
			Assert.AreEqual(GetItem1(ref _array, 1), GetItem1(ref array2, 2));
			Assert.AreEqual(GetItem2(ref _array, 1), GetItem2(ref array2, 2));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 3));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Middle_Middle()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(1 * _lengthInBytes, ref array2, 1 * _lengthInBytes, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(item1Def, GetItem1(ref array2, 0));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 0));
			Assert.AreEqual(GetItem1(ref _array, 1), GetItem1(ref array2, 1));
			Assert.AreEqual(GetItem2(ref _array, 1), GetItem2(ref array2, 1));
			Assert.AreEqual(GetItem1(ref _array, 2), GetItem1(ref array2, 2));
			Assert.AreEqual(GetItem2(ref _array, 2), GetItem2(ref array2, 2));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 3));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_End_Middle()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(2 * _lengthInBytes, ref array2, 1 * _lengthInBytes, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(item1Def, GetItem1(ref array2, 0));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 0));
			Assert.AreEqual(GetItem1(ref _array, 2), GetItem1(ref array2, 1));
			Assert.AreEqual(GetItem2(ref _array, 2), GetItem2(ref array2, 1));
			Assert.AreEqual(GetItem1(ref _array, 3), GetItem1(ref array2, 2));
			Assert.AreEqual(GetItem2(ref _array, 3), GetItem2(ref array2, 2));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 3));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Beginning_End()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(0, ref array2, 2 * _lengthInBytes, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(item1Def, GetItem1(ref array2, 0));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 0));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 1));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 1));
			Assert.AreEqual(GetItem1(ref _array, 0), GetItem1(ref array2, 2));
			Assert.AreEqual(GetItem2(ref _array, 0), GetItem2(ref array2, 2));
			Assert.AreEqual(GetItem1(ref _array, 1), GetItem1(ref array2, 3));
			Assert.AreEqual(GetItem2(ref _array, 1), GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Middle_End()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(1 * _lengthInBytes, ref array2, 2 * _lengthInBytes, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(item1Def, GetItem1(ref array2, 0));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 0));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 1));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 1));
			Assert.AreEqual(GetItem1(ref _array, 1), GetItem1(ref array2, 2));
			Assert.AreEqual(GetItem2(ref _array, 1), GetItem2(ref array2, 2));
			Assert.AreEqual(GetItem1(ref _array, 2), GetItem1(ref array2, 3));
			Assert.AreEqual(GetItem2(ref _array, 2), GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_End_End()
		{
			PopulateNativeArray(ref _array);

			var array2 = NativeDynamicArray.Alloc(_arrayLength * _lengthInBytes);
			_array.CopyTo(2 * _lengthInBytes, ref array2, 2 * _lengthInBytes, 2 * _lengthInBytes);

			var item1Def = new Item1();
			var item2Def = new Item2();
			Assert.AreEqual(item1Def, GetItem1(ref array2, 0));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 0));
			Assert.AreEqual(item1Def, GetItem1(ref array2, 1));
			Assert.AreEqual(item2Def, GetItem2(ref array2, 1));
			Assert.AreEqual(GetItem1(ref _array, 2), GetItem1(ref array2, 2));
			Assert.AreEqual(GetItem2(ref _array, 2), GetItem2(ref array2, 2));
			Assert.AreEqual(GetItem1(ref _array, 3), GetItem1(ref array2, 3));
			Assert.AreEqual(GetItem2(ref _array, 3), GetItem2(ref array2, 3));

			array2.Dispose();
		}

		[TestMethod]
		public void Clear()
		{
			PopulateNativeArray(ref _array);

			_array.Clear();

			var item1Def = new Item1();
			var item2Def = new Item2();
			for (int i = 0; i < _arrayLength; i++)
			{
				Assert.AreEqual(item1Def, GetItem1(ref _array, i));
				Assert.AreEqual(item2Def, GetItem2(ref _array, i));
			}
		}
	}*/
}
