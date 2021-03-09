using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityTests
{
	[TestClass]
	public class EntityEquallity
	{
		[TestMethod]
		public void DoubleEquals()
		{
			Assert.IsTrue(new Entity() == Entity.Null);
		}

		[TestMethod]
		public void DoubleNotEquals()
		{
			Assert.IsFalse(new Entity() != Entity.Null);
		}

		[TestMethod]
		public void Equals()
		{
			Assert.IsTrue(new Entity().Equals(Entity.Null));
		}

		[TestMethod]
		public void CompareTo()
		{
			Assert.IsTrue(new Entity().CompareTo(Entity.Null) == 0);
		}
	}
}