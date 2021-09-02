using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTriggerTests
{
    [TestClass]
    public class CollectionTriggerModify
    {
        [TestMethod]
        public void AddedDistinct()
        {
            var filter = CollectorTrigger.Added<TestComponent1, TestComponent1>();

            Assert.IsTrue(filter.AddedIndexes.Length == 1);
        }

        [TestMethod]
        public void AddedGeneric()
        {
            var filter = CollectorTrigger
                .Added<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();

            Assert.IsTrue(filter.AddedIndexes.Length == 4);
        }

        [TestMethod]
        public void RemovedDistinct()
        {
            var filter = CollectorTrigger.Removed<TestComponent1, TestComponent1>();

            Assert.IsTrue(filter.RemovedIndexes.Length == 1);
        }

        [TestMethod]
        public void RemovedGeneric()
        {
            var filter = CollectorTrigger
                .Removed<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();

            Assert.IsTrue(filter.RemovedIndexes.Length == 4);
        }

        [TestMethod]
        public void Combine()
        {
            var filter = CollectorTrigger.Combine(
                CollectorTrigger.Added<TestComponent1>(),
                CollectorTrigger.Removed<TestComponent1>(),
                CollectorTrigger.Replaced<TestComponent1>());

            Assert.IsTrue(
                filter.AddedIndexes.Length == 1 &&
                filter.RemovedIndexes.Length == 1 &&
                filter.ReplacedIndexes.Length == 1);
        }

        [TestMethod]
        public void CombineIndexes()
        {
            var filter = CollectorTrigger.Combine(
                CollectorTrigger.Added<TestComponent1>(),
                CollectorTrigger.Removed<TestComponent2>(),
                CollectorTrigger.Replaced<TestRecordableComponent1>());

            Assert.IsTrue(filter.Indexes.Length == 3);
            Assert.IsTrue(filter.AddedIndexes.Length == 1);
            Assert.IsTrue(filter.RemovedIndexes.Length == 1);
            Assert.IsTrue(filter.ReplacedIndexes.Length == 1);
        }

        [TestMethod]
        public void ReplacedDistinct()
        {
            var filter = CollectorTrigger.Replaced<TestComponent1, TestComponent1>();

            Assert.IsTrue(filter.ReplacedIndexes.Length == 1);
        }

        [TestMethod]
        public void ReplacedGeneric()
        {
            var filter = CollectorTrigger
                .Replaced<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();

            Assert.IsTrue(filter.ReplacedIndexes.Length == 4);
        }
    }
}