using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.GroupTests
{
	[TestClass]
	public class GroupFilter
	{
		[TestMethod]
		public void AutoRemoveFromGroup()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var entity = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.RemoveComponent<TestComponent1>(entity);

			Assert.IsTrue(group.Entities.Length == 0);
		}

		[TestMethod]
		public void CreateAfterEntityCreate()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			world.EntityManager.AddComponent<TestComponent1>(entity);

			Assert.IsTrue(group.Entities[0] == entity);
		}

		[TestMethod]
		public void CreateBeforeEntityCreate()
		{
			var world = World.CreateWorld();
			var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var entity = world.EntityManager.CreateEntity();

			world.EntityManager.AddComponent<TestComponent1>(entity);

			Assert.IsTrue(group.Entities[0] == entity);
		}

		[TestMethod]
		public void Get()
		{
			var group = World.CreateWorld().GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			Assert.IsTrue(group != null);
		}

		[TestMethod]
		public void GetSame()
		{
			var world = World.CreateWorld();
			var group1 = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var group2 = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			Assert.IsTrue(group1 == group2);
		}
	}
}