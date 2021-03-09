using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.FilterTests
{
	[TestClass]
	public class FilterAllOfEquallity
	{
		[TestMethod]
		public void Equals()
		{
			var filter1 = Filter.AllOf<TestComponent1>();
			var filter2 = Filter.AllOf<TestComponent1>();

			Assert.IsTrue(filter1.Equals(filter2));
		}

		[TestMethod]
		public void EqualsNull()
		{
			var filter = Filter.AllOf<TestComponent1>();

			Assert.IsFalse(filter.Equals(null));
		}

		[TestMethod]
		public void HashCode()
		{
			var filter1 = Filter.AllOf<TestComponent1, TestComponent2>();
			var filter2 = Filter.AllOf<TestComponent2, TestComponent1>();

			Assert.IsTrue(filter1.GetHashCode() == filter2.GetHashCode());
		}
	}
}