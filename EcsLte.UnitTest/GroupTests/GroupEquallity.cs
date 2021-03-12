using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.GroupTests
{
	[TestClass]
	public class GroupEquallity
	{
		[TestMethod]
		public void EqualsSameWorld()
		{
			var world = World.CreateWorld();
			var group1 = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var group2 = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			Assert.IsTrue(group1.Equals(group2));
		}

		[TestMethod]
		public void EqualsDifferentWorld()
		{
			var group1 = World.CreateWorld().GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
			var group2 = World.CreateWorld().GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

			Assert.IsFalse(group1.Equals(group2));
		}
	}
}