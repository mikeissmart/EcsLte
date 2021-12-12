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

			Assert.IsTrue(filter.AllOfIndexes.Length == 1);
		}

		[TestMethod]
		public void AllOfGeneric()
		{
			var filter = Filter
				.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();

			Assert.IsTrue(filter.AllOfIndexes.Length == 4);
		}

		[TestMethod]
		public void AnyOfDistinct()
		{
			var filter = Filter.AnyOf<TestComponent1, TestComponent1>();

			Assert.IsTrue(filter.AnyOfIndexes.Length == 1);
		}

		[TestMethod]
		public void AnyOfGeneric()
		{
			var filter = Filter
				.AnyOf<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();

			Assert.IsTrue(filter.AnyOfIndexes.Length == 4);
		}

		[TestMethod]
		public void Combine()
		{
			var filter = Filter.Combine(
				Filter.AllOf<TestComponent1>(),
				Filter.AnyOf<TestComponent1>(),
				Filter.NoneOf<TestComponent1>());

			Assert.IsTrue(
				filter.AllOfIndexes.Length == 1 &&
				filter.AnyOfIndexes.Length == 1 &&
				filter.NoneOfIndexes.Length == 1);
		}

		[TestMethod]
		public void CombineIndexes()
		{
			var filter = Filter.Combine(
				Filter.AllOf<TestComponent1>(),
				Filter.AnyOf<TestComponent2>(),
				Filter.NoneOf<TestRecordableComponent1>());

			Assert.IsTrue(filter.Indexes.Length == 3);
			Assert.IsTrue(filter.AllOfIndexes.Length == 1);
			Assert.IsTrue(filter.AnyOfIndexes.Length == 1);
			Assert.IsTrue(filter.NoneOfIndexes.Length == 1);
		}

		[TestMethod]
		public void NoneOfDistinct()
		{
			var filter = Filter.NoneOf<TestComponent1, TestComponent1>();

			Assert.IsTrue(filter.NoneOfIndexes.Length == 1);
		}

		[TestMethod]
		public void NoneOfGeneric()
		{
			var filter = Filter
				.NoneOf<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();

			Assert.IsTrue(filter.NoneOfIndexes.Length == 4);
		}
	}
}