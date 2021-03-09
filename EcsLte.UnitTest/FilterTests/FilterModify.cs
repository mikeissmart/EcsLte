using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.FilterTests
{
	[TestClass]
	public class FilterModify
	{
		[TestMethod]
		public void AllOfDistinct()
		{
			var filter = Filter.AllOf<TestComponent1, TestComponent1>();

			Assert.IsTrue(filter.AllOfIndices.Length == 1);
		}

		[TestMethod]
		public void AllOfFilter()
		{
			var filter = Filter.AllOf(Filter.AllOf<TestComponent1>(), Filter.AllOf<TestComponent2>());

			Assert.IsTrue(filter.AllOfIndices.Length == 2);
		}

		[TestMethod]
		public void AllOfGeneric()
		{
			var filter = Filter.AllOf<TestComponent1, TestComponent2, TestComponent3, TestComponent4, TestComponent5>();

			Assert.IsTrue(filter.AllOfIndices.Length == 5);
		}

		[TestMethod]
		public void AnyOfDistinct()
		{
			var filter = Filter.AnyOf<TestComponent1, TestComponent1>();

			Assert.IsTrue(filter.AnyOfIndices.Length == 1);
		}

		[TestMethod]
		public void AnyOfFilter()
		{
			var filter = Filter.AnyOf(Filter.AnyOf<TestComponent1>(), Filter.AnyOf<TestComponent2>());

			Assert.IsTrue(filter.AnyOfIndices.Length == 2);
		}

		[TestMethod]
		public void AnyOfGeneric()
		{
			var filter = Filter.AnyOf<TestComponent1, TestComponent2, TestComponent3, TestComponent4, TestComponent5>();

			Assert.IsTrue(filter.AnyOfIndices.Length == 5);
		}

		[TestMethod]
		public void NoneOfDistinct()
		{
			var filter = Filter.NoneOf<TestComponent1, TestComponent1>();

			Assert.IsTrue(filter.NoneOfIndices.Length == 1);
		}

		[TestMethod]
		public void NoneOfFilter()
		{
			var filter = Filter.NoneOf(Filter.NoneOf<TestComponent1>(), Filter.NoneOf<TestComponent2>());

			Assert.IsTrue(filter.NoneOfIndices.Length == 2);
		}

		[TestMethod]
		public void NoneOfGeneric()
		{
			var filter = Filter.NoneOf<TestComponent1, TestComponent2, TestComponent3, TestComponent4, TestComponent5>();

			Assert.IsTrue(filter.NoneOfIndices.Length == 5);
		}
	}
}