using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.Key
{
	[TestClass]
	public class SharedKeyLife
	{
		[TestMethod]
		public void GetSharedKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);

			Assert.IsTrue(sharedKey != null);
		}

		[TestMethod]
		public void GetSharedKeyNoAttribute()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			Assert.ThrowsException<ComponentDoesNotHaveSharedKey>(() =>
			{
				world.KeyManager.GetSharedKey<TestComponent1>(group);
			});
		}

		[TestMethod]
		public void GetSharedKeyNotInGroupAllOf()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());

			Assert.ThrowsException<KeyGroupComponentNotInAllOrAnyException>(() =>
			{
				world.KeyManager.GetSharedKey<TestSharedKeyComponent2>(group);
			});
		}

		[TestMethod]
		public void GetSharedKeyNotInGroupAnyOf()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AnyOf<TestSharedKeyComponent1>());

			Assert.ThrowsException<KeyGroupComponentNotInAllOrAnyException>(() =>
			{
				world.KeyManager.GetSharedKey<TestSharedKeyComponent2>(group);
			});
		}

		[TestMethod]
		public void GetSharedKeySame()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey1 = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);
			var sharedKey2 = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);

			Assert.IsTrue(sharedKey1 == sharedKey2);
		}

		[TestMethod]
		public void DestroySharedKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);

			world.KeyManager.DestroySharedKey(sharedKey);

			Assert.IsTrue(sharedKey.IsDestroyed);
		}
	}
}