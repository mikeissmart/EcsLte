using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTriggerTests
{
    [TestClass]
    public class CollectionTriggerAddedEquality
    {
        [TestMethod]
        public void DoubleEquals()
        {
            var collectorTrigger1 = CollectorTrigger.Added<TestComponent1>();
            var collectorTrigger2 = CollectorTrigger.Added<TestComponent1>();

            Assert.IsTrue(collectorTrigger1 == collectorTrigger2);
        }

        [TestMethod]
        public void DoubleNotEquals()
        {
            var collectorTrigger1 = CollectorTrigger.Added<TestComponent1>();
            var collectorTrigger2 = CollectorTrigger.Added<TestComponent1>();

            Assert.IsFalse(collectorTrigger1 != collectorTrigger2);
        }

        [TestMethod]
        public void Equals()
        {
            var collectorTrigger1 = CollectorTrigger.Added<TestComponent1>();
            var collectorTrigger2 = CollectorTrigger.Added<TestComponent1>();

            Assert.IsTrue(collectorTrigger1.Equals(collectorTrigger2));
        }

        [TestMethod]
        public void EqualsNull()
        {
            var collectorTrigger = CollectorTrigger.Added<TestComponent1>();

            Assert.IsFalse(collectorTrigger.Equals(null));
        }

        [TestMethod]
        public void HashCode()
        {
            var collectorTrigger1 = CollectorTrigger.Added<TestComponent1, TestComponent2>();
            var collectorTrigger2 = CollectorTrigger.Added<TestComponent2, TestComponent1>();

            Assert.IsTrue(collectorTrigger1.GetHashCode() == collectorTrigger2.GetHashCode());
        }
    }
}