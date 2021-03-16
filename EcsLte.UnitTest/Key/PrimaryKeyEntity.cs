using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.Key
{
	[TestClass]
	public class PrimaryKeyEntity
	{
		[TestMethod]
		public void GetEntity()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent(entity1, new TestPrimaryKeyComponent1());
			world.EntityManager.AddComponent(entity2, new TestPrimaryKeyComponent1 { Prop = 1 });

			var keyEntity = primaryKey.GetEntity(new TestPrimaryKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntity == entity2);
		}

		[TestMethod]
		public void GetEntityRemoveComponent()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);
			var entity = world.EntityManager.CreateEntity();

			world.EntityManager.ReplaceComponent(entity, new TestPrimaryKeyComponent1 { Prop = 1 });
			world.EntityManager.RemoveComponent<TestPrimaryKeyComponent1>(entity);

			var keyEntity = primaryKey.GetEntity(new TestPrimaryKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntity == Entity.Null);
		}

		[TestMethod]
		public void GetEntityReplaceComponent()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);
			var entity = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent(entity, new TestPrimaryKeyComponent1 { Prop = 1 });
			var keyEntity1 = primaryKey.GetEntity(new TestPrimaryKeyComponent1 { Prop = 1 });

			world.EntityManager.ReplaceComponent(entity, new TestPrimaryKeyComponent1 { Prop = 2 });
			var keyEntity2 = primaryKey.GetEntity(new TestPrimaryKeyComponent1 { Prop = 1 });

			Assert.IsTrue(keyEntity1 != keyEntity2);
		}

		[TestMethod]
		public void DuplicateKey()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();

			world.EntityManager.ReplaceComponent(entity1, new TestPrimaryKeyComponent1 { Prop = 1 });

			Assert.ThrowsException<PrimaryKeyDuplicateKeyException>(() =>
			{
				world.EntityManager.ReplaceComponent(entity2, new TestPrimaryKeyComponent1 { Prop = 1 });
			});
		}

		[TestMethod]
		public void DestroyGetEntity()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>());
			var primaryKey = world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(group);

			world.KeyManager.DestroyPrimaryKey(primaryKey);

			Assert.ThrowsException<KeyIsDestroyedException>(() => { primaryKey.GetEntity(new TestPrimaryKeyComponent1()); });
		}
	}
}