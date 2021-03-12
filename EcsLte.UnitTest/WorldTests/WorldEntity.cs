using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.WorldTests
{
	[TestClass]
	public class WorldEntity
	{
		[TestMethod]
		public void GetEntity()
		{
			var world = World.CreateWorld();
			var entity1 = world.EntityManager.CreateEntity();
			var entity2 = world.EntityManager.GetEntity(entity1.Id);

			Assert.IsTrue(entity1 == entity2);
		}

		[TestMethod]
		public void GetEntities()
		{
			int createCount = 5;
			var world = World.CreateWorld();
			var createdEntities = new Entity[createCount];

			for (int i = 0; i < createCount; i++)
				createdEntities[i] = world.EntityManager.CreateEntity();

			var getEntities = world.EntityManager.GetEntities();
			bool areSame = true;
			for (int i = 0; i < createCount; i++)
			{
				if (createdEntities[i] != getEntities[i])
					areSame = false;
			}

			Assert.IsTrue(
				createdEntities.Length == getEntities.Length &&
				areSame);
		}

		[TestMethod]
		public void HasEntity()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();

			Assert.IsTrue(world.EntityManager.HasEntity(entity));
		}
	}
}