using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.Key
{
	[TestClass]
	public class SharedKeyEntity
	{
		[TestMethod]
		public void GetFirstOrSingleEntity()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent(entity1, new TestSharedKeyComponent1());
			world.EntityManager.AddComponent(entity2, new TestSharedKeyComponent1 { Prop = 1 });

			var keyEntity = sharedKey.GetFirstOrSingleEntity(new TestSharedKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntity == entity2);
		}

		[TestMethod]
		public void GetEntities()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();
			var entity3 = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent(entity1, new TestSharedKeyComponent1());
			world.EntityManager.AddComponent(entity2, new TestSharedKeyComponent1 { Prop = 1 });
			world.EntityManager.AddComponent(entity3, new TestSharedKeyComponent1 { Prop = 1 });

			var keyEntities = sharedKey.GetEntities(new TestSharedKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntities.Count == 2);
		}

		[TestMethod]
		public void GetEntitiesRemoveComponent()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);
			var entity = world.EntityManager.CreateEntity();

			world.EntityManager.ReplaceComponent(entity, new TestSharedKeyComponent1 { Prop = 1 });
			world.EntityManager.RemoveComponent<TestSharedKeyComponent1>(entity);

			var keyEntities = sharedKey.GetEntities(new TestSharedKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntities.Count == 0);
		}

		[TestMethod]
		public void GetEntitiesReplaceComponent()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);
			var entity = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent(entity, new TestSharedKeyComponent1 { Prop = 1 });
			world.EntityManager.ReplaceComponent(entity, new TestSharedKeyComponent1 { Prop = 2 });

			var keyEntities2 = sharedKey.GetEntities(new TestSharedKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntities2.Count == 0);
		}

		[TestMethod]
		public void DestroyGetEntity()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			var sharedKey = world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);

			world.KeyManager.DestroySharedKey(sharedKey);

			Assert.ThrowsException<KeyIsDestroyedException>(() =>
			{
				sharedKey.GetEntities(new TestSharedKeyComponent1());
			});
		}
	}
}