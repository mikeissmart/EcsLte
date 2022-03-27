using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcsLte.Data.Unmanaged;

namespace EcsLte.UnitTest.Data.Unmanaged
{
	/*[TestClass]
	public class Unmanaged_NativeDynamicList
	{
		private struct Item
		{
			public int Id { get; set; }
		}

		private NativeDynamicList _list;

		[TestInitialize]
		public void Init()
		{
			_list = NativeDynamicList.Alloc<Item>();
		}

		[TestCleanup]
		public void Cleanup()
		{
			_list.Dispose();
		}

		[TestMethod]
		public void AddItem()
		{
			_list.Add(new Item { Id = 1 });

			Assert.AreEqual(1, _list.Count);
			Assert.AreEqual(1, _list[0].Id);
		}

		[TestMethod]
		public void AddItem_IncreaseCapacity()
		{
			// Capacity set after first add
			Assert.AreEqual(0, _list.Capacity);
			_list.Add(new Item { Id = 0 });
			Assert.AreEqual(4, _list.Capacity);

			for (int i = 1; i < 5; i++)
				_list.Add(new Item { Id = i });

			Assert.AreEqual(8, _list.Capacity);
			for (int i = 0; i < _list.Count; i++)
				Assert.AreEqual(i, _list[i].Id);
		}

		[TestMethod]
		public void GetItem()
		{
			_list.Add(new Item { Id = 1 });

			Assert.AreEqual(1, _list[0].Id);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list[1]);
		}

		[TestMethod]
		public void SetItem()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });

			_list[0].Id = 11;
			_list[1] = new Item { Id = 22 };

			Assert.AreEqual(11, _list[0].Id);
			Assert.AreEqual(22, _list[1].Id);
		}

		[TestMethod]
		public void IndexOf()
		{
			_list.Add(new Item { Id = 1 });

			Assert.AreEqual(0, _list.IndexOf(new Item { Id = 1 }));
			Assert.AreEqual(-1, _list.IndexOf(new Item { Id = 2 }));
		}

		[TestMethod]
		public void Remove()
		{
			_list.Add(new Item { Id = 1 });

			Assert.IsTrue(_list.Remove(new Item { Id = 1 }));
			Assert.IsFalse(_list.Remove(new Item { Id = 1 }));
		}

		[TestMethod]
		public void RemoveAt_Single()
		{
			_list.Add(new Item { Id = 1 });

			_list.RemoveAt(0);

			Assert.AreEqual(0, _list.Count);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list.RemoveAt(0));
		}

		[TestMethod]
		public void RemoveAt_Beginning()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });

			_list.RemoveAt(0);

			Assert.AreEqual(2, _list[0].Id);
			Assert.AreEqual(3, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list.RemoveAt(2));
		}

		[TestMethod]
		public void RemoveAt_Middle()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });

			_list.RemoveAt(1);

			Assert.AreEqual(1, _list[0].Id);
			Assert.AreEqual(3, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list.RemoveAt(2));
		}

		[TestMethod]
		public void RemoveAt_End()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });

			_list.RemoveAt(2);

			Assert.AreEqual(1, _list[0].Id);
			Assert.AreEqual(2, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list.RemoveAt(2));
		}

		[TestMethod]
		public void RemoveAtRange_Single_Beginning()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });

			_list.RemoveAtRange(0, 1);

			Assert.AreEqual(2, _list[0].Id);
			Assert.AreEqual(3, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list.RemoveAtRange(2, 1));
		}

		[TestMethod]
		public void RemoveAtRange_Single_Middle()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });

			_list.RemoveAtRange(1, 1);

			Assert.AreEqual(1, _list[0].Id);
			Assert.AreEqual(3, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
		}

		[TestMethod]
		public void RemoveAtRange_Single_End()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });

			_list.RemoveAtRange(2, 1);

			Assert.AreEqual(1, _list[0].Id);
			Assert.AreEqual(2, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
		}

		[TestMethod]
		public void RemoveAtRange_Multiple_Beginning()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });
			_list.Add(new Item { Id = 4 });

			_list.RemoveAtRange(0, 2);

			Assert.AreEqual(3, _list[0].Id);
			Assert.AreEqual(4, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
			Assert.ThrowsException<IndexOutOfRangeException>(() => _list.RemoveAtRange(1, 2));
		}

		[TestMethod]
		public void RemoveAtRange_Multiple_Middle()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });
			_list.Add(new Item { Id = 4 });

			_list.RemoveAtRange(1, 2);

			Assert.AreEqual(1, _list[0].Id);
			Assert.AreEqual(4, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
		}

		[TestMethod]
		public void RemoveAtRange_Multiple_End()
		{
			_list.Add(new Item { Id = 1 });
			_list.Add(new Item { Id = 2 });
			_list.Add(new Item { Id = 3 });
			_list.Add(new Item { Id = 4 });

			_list.RemoveAtRange(2, 2);

			Assert.AreEqual(1, _list[0].Id);
			Assert.AreEqual(2, _list[1].Id);
			Assert.AreEqual(2, _list.Count);
		}

		[TestMethod]
		public void ToNativeArray()
		{
			_list.Add(new Item { Id = 1 });

			var array = _list.ToNativeArray();

			Assert.AreEqual(1, array[0].Id);
			Assert.AreEqual(_list.Count, array.Length);

			array.Dispose();
		}

		[TestMethod]
		public void ToManagedArray()
		{
			_list.Add(new Item { Id = 1 });

			var array = _list.ToManagedArray();

			Assert.AreEqual(1, array[0].Id);
			Assert.AreEqual(_list.Count, array.Length);
		}

		[TestMethod]
		public void Clear()
		{
			_list.Add(new Item { Id = 1 });

			_list.Clear();

			Assert.AreEqual(0, _list.Count);
		}
	}*/
}
