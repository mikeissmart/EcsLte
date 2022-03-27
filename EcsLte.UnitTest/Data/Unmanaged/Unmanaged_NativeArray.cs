using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcsLte.Data.Unmanaged;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.Data.Unmanaged
{
	/*[TestClass]
	public class Unmanaged_NativeArray
	{
		private struct Item
		{
			public int Id { get; set; }
		}
		private NativeArrayPtr _array;

		[TestInitialize]
		public void Init()
		{
			_array = NativeArrayPtr.Alloc<Item>(4);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_array.Dispose();
		}

		[TestMethod]
		public void Alloc()
		{
			var array = NativeArrayPtr.Alloc<Item>(4);

			Assert.AreEqual(4, array.Length);

			array.Dispose();
		}

		[TestMethod]
		public void SetGetItems()
		{
			_array.Set(0, new Item { Id = 1 });

			Assert.AreEqual(1, _array.Get<Item>(0).Id);
			for (int i = 1; i < _array.Length; i++)
				Assert.AreEqual(0, _array.Get<Item>(i).Id);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _array.Get<Item>(4));
		}

		[TestMethod]
		public void IndexOf()
		{
			_array.Set(0, new Item { Id = 1 });

			Assert.AreEqual(0, _array.IndexOf(new Item { Id = 1 }));
			Assert.AreEqual(-1, _array.IndexOf(new Item { Id = 2 }));
		}

		[TestMethod]
		public void Resize_Bigger()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			_array.Resize<Item>(4 * 2);

			for (int i = 0; i < 4; i++)
				Assert.AreEqual(i + 1, _array.Get<Item>(i).Id);
			for (int i = 4; i < 4 * 2; i++)
				Assert.AreEqual(0, _array.Get<Item>(i).Id);
		}

		[TestMethod]
		public void Resize_Smaller()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			_array.Resize<Item>(4 / 2);

			for (int i = 0; i < _array.Length; i++)
				Assert.AreEqual(i + 1, _array.Get<Item>(i).Id);
		}

		[TestMethod]
		public void CopyTo_All()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(0, ref array2, 0, 4);

			for (int i = 0; i < array2.Length; i++)
				Assert.AreEqual(i + 1, array2.Get<Item>(i).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Beginning_Beginning()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(0, ref array2, 0, 2);

			Assert.AreEqual(1, array2.Get<Item>(0).Id);
			Assert.AreEqual(2, array2.Get<Item>(1).Id);
			Assert.AreEqual(0, array2.Get<Item>(2).Id);
			Assert.AreEqual(0, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Middle_Beginning()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(1, ref array2, 0, 2);

			Assert.AreEqual(2, array2.Get<Item>(0).Id);
			Assert.AreEqual(3, array2.Get<Item>(1).Id);
			Assert.AreEqual(0, array2.Get<Item>(2).Id);
			Assert.AreEqual(0, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_End_Beginning()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(2, ref array2, 0, 2);

			Assert.AreEqual(3, array2.Get<Item>(0).Id);
			Assert.AreEqual(4, array2.Get<Item>(1).Id);
			Assert.AreEqual(0, array2.Get<Item>(2).Id);
			Assert.AreEqual(0, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Beginning_Middle()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(0, ref array2, 1, 2);

			Assert.AreEqual(0, array2.Get<Item>(0).Id);
			Assert.AreEqual(1, array2.Get<Item>(1).Id);
			Assert.AreEqual(2, array2.Get<Item>(2).Id);
			Assert.AreEqual(0, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Middle_Middle()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(1, ref array2, 1, 2);

			Assert.AreEqual(0, array2.Get<Item>(0).Id);
			Assert.AreEqual(2, array2.Get<Item>(1).Id);
			Assert.AreEqual(3, array2.Get<Item>(2).Id);
			Assert.AreEqual(0, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_End_Middle()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(2, ref array2, 1, 2);

			Assert.AreEqual(0, array2.Get<Item>(0).Id);
			Assert.AreEqual(3, array2.Get<Item>(1).Id);
			Assert.AreEqual(4, array2.Get<Item>(2).Id);
			Assert.AreEqual(0, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Beginning_End()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(0, ref array2, 2, 2);

			Assert.AreEqual(0, array2.Get<Item>(0).Id);
			Assert.AreEqual(0, array2.Get<Item>(1).Id);
			Assert.AreEqual(1, array2.Get<Item>(2).Id);
			Assert.AreEqual(2, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_Middle_End()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(1, ref array2, 2, 2);

			Assert.AreEqual(0, array2.Get<Item>(0).Id);
			Assert.AreEqual(0, array2.Get<Item>(1).Id);
			Assert.AreEqual(2, array2.Get<Item>(2).Id);
			Assert.AreEqual(3, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void CopyTo_End_End()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array2 = NativeArrayPtr.Alloc<Item>(4);
			_array.CopyTo(2, ref array2, 2, 2);

			Assert.AreEqual(0, array2.Get<Item>(0).Id);
			Assert.AreEqual(0, array2.Get<Item>(1).Id);
			Assert.AreEqual(3, array2.Get<Item>(2).Id);
			Assert.AreEqual(4, array2.Get<Item>(3).Id);

			array2.Dispose();
		}

		[TestMethod]
		public void ToManagedArray()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			var array = _array.ToManagedArray<Item>();

			for (int i = 0; i < array.Length; i++)
				Assert.AreEqual(i + 1, array[i].Id);
			Assert.AreEqual(_array.Length, array.Length);
		}

		[TestMethod]
		public void Clear()
		{
			for (int i = 0; i < _array.Length; i++)
				_array.Set(i, new Item { Id = i + 1 });

			_array.Clear();

			for (int i = 0; i < _array.Length; i++)
				Assert.AreNotEqual(i + 1, _array.Get<Item>(i).Id);
		}
	}*/
}
