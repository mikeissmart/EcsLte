using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.Key
{
	[TestClass]
	public class PrimaryKeyLife
	{
		[TestMethod]
		public void GetPrimaryKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);

			Assert.IsTrue(primaryKey != null);
		}

		[TestMethod]
		public void GetPrimaryKeyNoAttribute()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			Assert.ThrowsException<ComponentDoesNotHavePrimaryKey>(() =>
			{
				world.KeyManager.GetPrimaryKey<TestComponent1>(group);
			});
		}

		[TestMethod]
		public void GetPrimaryKeyNotInGroupAllOf()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());

			Assert.ThrowsException<KeyGroupComponentNotInAllOrAnyException>(() =>
			{
				world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent2>(group);
			});
		}

		[TestMethod]
		public void GetPrimaryKeyNotInGroupAnyOf()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AnyOf<TestPrimaryKeyComponent1>());

			Assert.ThrowsException<KeyGroupComponentNotInAllOrAnyException>(() =>
			{
				world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent2>(group);
			});
		}

		[TestMethod]
		public void GetPrimaryKeySame()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey1 = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);
			var primaryKey2 = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);

			Assert.IsTrue(primaryKey1 == primaryKey2);
		}

		[TestMethod]
		public void DestroyPrimaryKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);

			world.KeyManager.DestroyPrimaryKey(primaryKey);

			Assert.IsTrue(primaryKey.IsDestroyed);
		}
	}
}