using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.WorldTests
{
	[TestClass]
	public class WorldLife
	{
		[TestMethod]
		public void DefaultWorld()
		{
			var world = World.DefaultWorld;

			Assert.IsTrue(world != null && !world.IsDestroyed && world == World.GetWorld(world.WorldId));
		}

		[TestMethod]
		public void Create()
		{
			var world = World.CreateWorld();

			Assert.IsTrue(world != null && !world.IsDestroyed && world == World.GetWorld(world.WorldId));
		}

		[TestMethod]
		public void CreateMultiple()
		{
			var world1 = World.CreateWorld();
			var world2 = World.CreateWorld();

			Assert.IsTrue(
				world1 != null && !world1.IsDestroyed && world1 == World.GetWorld(world1.WorldId));
			Assert.IsTrue(
				world2 != null && !world2.IsDestroyed && world2 == World.GetWorld(world2.WorldId));
		}

		[TestMethod]
		public void Destroy()
		{
			var world = World.CreateWorld();
			world.DestroyWorld();

			Assert.IsTrue(world.IsDestroyed && World.GetWorld(world.WorldId) == null);
		}

		[TestMethod]
		public void DestroyAfterDestroy()
		{
			var world = World.CreateWorld();
			world.DestroyWorld();

			Assert.ThrowsException<WorldIsDestroyedException>(() => world.DestroyWorld());
		}
	}
}