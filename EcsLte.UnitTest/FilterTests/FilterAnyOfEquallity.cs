using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.FilterTests
{
	[TestClass]
	public class FilterAnyOfEquallity
	{
		[TestMethod]
		public void DoubleEquals()
		{
			var filter1 = Filter.AnyOf<TestComponent1>();
			var filter2 = Filter.AnyOf<TestComponent1>();

			Assert.IsTrue(filter1 == filter2);
		}

		[TestMethod]
		public void DoubleNotEquals()
		{
			var filter1 = Filter.AnyOf<TestComponent1>();
			var filter2 = Filter.AnyOf<TestComponent1>();

			Assert.IsFalse(filter1 != filter2);
		}

		[TestMethod]
		public void Equals()
		{
			var filter1 = Filter.AnyOf<TestComponent1>();
			var filter2 = Filter.AnyOf<TestComponent1>();

			Assert.IsTrue(filter1.Equals(filter2));
		}

		[TestMethod]
		public void EqualsNull()
		{
			var filter = Filter.AnyOf<TestComponent1>();

			Assert.IsFalse(filter.Equals(null));
		}

		[TestMethod]
		public void HashCode()
		{
			var filter1 = Filter.AnyOf<TestComponent1, TestComponent2>();
			var filter2 = Filter.AnyOf<TestComponent2, TestComponent1>();

			Assert.IsTrue(filter1.GetHashCode() == filter2.GetHashCode());
		}
	}
}