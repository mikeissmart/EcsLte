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
			var entity = World.CreateWorld().CreateEntity();

			Assert.IsTrue(entity.IsAlive && entity.Id != 0);
		}

		[TestMethod]
		public void CreateAfterWorldDestroy()
		{
			var world = World.CreateWorld();
			world.DestroyWorld();

			Assert.ThrowsException<WorldIsDestroyedException>(() => world.CreateEntity());
		}

		[TestMethod]
		public void CreateNoWorld()
		{
			var entity = new Entity();

			Assert.IsFalse(entity.IsAlive);
		}

		[TestMethod]
		public void CreateMultiple()
		{
			var world = World.CreateWorld();
			var entity1 = world.CreateEntity();
			var entity2 = world.CreateEntity();

			Assert.IsTrue(entity1.IsAlive && entity1.Id != 0);
			Assert.IsTrue(entity2.IsAlive && entity2.Id != 0);
		}

		[TestMethod]
		public void CreateReuse()
		{
			var world = World.CreateWorld();
			var entity1 = world.CreateEntity();
			world.DestroyEntity(entity1);

			var entity2 = world.CreateEntity();

			Assert.IsTrue(entity2.IsAlive && entity2.Generation != 1);
		}

		[TestMethod]
		public void Destroy()
		{
			var world = World.CreateWorld();
			var entity = world.CreateEntity();
			world.DestroyEntity(entity);

			Assert.IsFalse(entity.IsAlive);
		}

		[TestMethod]
		public void DestroyAfterWorldDestroy()
		{
			var world = World.CreateWorld();
			var entity = world.CreateEntity();
			world.DestroyWorld();

			Assert.ThrowsException<WorldIsDestroyedException>(() => world.DestroyEntity(entity));
		}

		[TestMethod]
		public void DestroyMultiple()
		{
			var world = World.CreateWorld();
			var entity1 = world.CreateEntity();
			var entity2 = world.CreateEntity();

			world.DestroyEntity(entity1);
			world.DestroyEntity(entity2);

			Assert.IsFalse(entity1.IsAlive);
			Assert.IsFalse(entity2.IsAlive);
		}
	}
}