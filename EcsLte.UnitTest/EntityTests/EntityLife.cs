using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityTests
{
	[TestClass]
	public class EntityLife
	{
		[TestMethod]
		public void Create()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();

			Assert.IsTrue(world.EntityManager.HasEntity(entity) && entity.Id != 0);
		}

		[TestMethod]
		public void CreateAfterWorldDestroy()
		{
			var world = World.CreateWorld();
			World.DestroyWorld(world);

			Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.CreateEntity());
		}

		[TestMethod]
		public void CreateNoWorld()
		{
			var world = World.CreateWorld();
			var entity = new Entity();

			Assert.IsFalse(world.EntityManager.HasEntity(entity));
		}

		[TestMethod]
		public void CreateMultiple()
		{
			var world = World.CreateWorld();
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();

			Assert.IsTrue(world.EntityManager.HasEntity(entity1) && entity1.Id != 0);
			Assert.IsTrue(world.EntityManager.HasEntity(entity2) && entity2.Id != 0);
		}

		[TestMethod]
		public void CreateReuse()
		{
			var world = World.CreateWorld();
			var entity1 = world.EntityManager.CreateEntity();
			world.EntityManager.DestroyEntity(entity1);

			var entity2 = world.EntityManager.CreateEntity();

			Assert.IsTrue(world.EntityManager.HasEntity(entity2) && entity2.Generation != 1);
		}

		[TestMethod]
		public void Destroy()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.DestroyEntity(entity);

			Assert.IsFalse(world.EntityManager.HasEntity(entity));
		}

		[TestMethod]
		public void DestroyAfterWorldDestroy()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			World.DestroyWorld(world);

			Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.DestroyEntity(entity));
		}

		[TestMethod]
		public void DestroyMultiple()
		{
			var world = World.CreateWorld();
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.CreateEntity();

			world.EntityManager.DestroyEntity(entity1);
			world.EntityManager.DestroyEntity(entity2);

			Assert.IsFalse(world.EntityManager.HasEntity(entity1));
			Assert.IsFalse(world.EntityManager.HasEntity(entity2));
		}
	}
}