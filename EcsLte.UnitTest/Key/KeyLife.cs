using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityIndex
{
	[TestClass]
	public class KeyLife
	{
		[TestMethod]
		public void GetSharedKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestComponent1>(group);

			Assert.IsTrue(sharedKey != null);
		}

		[TestMethod]
		public void GetSharedKeyGetEntity()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestComponent1>(group);
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();

			world.EntityManager.ReplaceComponent(entity1, new TestComponent1());
			world.EntityManager.ReplaceComponent(entity2, new TestComponent1 { Prop = 1 });

			var keyEntity = sharedKey.GetFirstOrSingleEntity(new TestComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntity == entity2);
		}

		[TestMethod]
		public void GetSharedKeyGetEntities()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestComponent1>(group);
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();
			var entity3 = world.EntityManager.CreateEntity();

			world.EntityManager.ReplaceComponent(entity1, new TestComponent1());
			world.EntityManager.ReplaceComponent(entity2, new TestComponent1 { Prop = 1 });
			world.EntityManager.ReplaceComponent(entity3, new TestComponent1 { Prop = 1 });

			var keyEntities = sharedKey.GetEntities(new TestComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntities.Count == 2);
		}

		[TestMethod]
		public void GetSharedKeySame()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var sharedKey1 = world.KeyManager.GetSharedKey<TestComponent1>(group);
			var sharedKey2 = world.KeyManager.GetSharedKey<TestComponent1>(group);

			Assert.IsTrue(sharedKey1 == sharedKey2);
		}

		[TestMethod]
		public void DestroySharedKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestComponent1>(group);

			world.KeyManager.DestroySharedKey(sharedKey);

			Assert.IsTrue(sharedKey.IsDestroyed);
		}

		[TestMethod]
		public void DestroySharedKeyGetEntity()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestComponent1>(group);

			world.KeyManager.DestroySharedKey(sharedKey);

			Assert.IsTrue(sharedKey.IsDestroyed);
		}
	}
}