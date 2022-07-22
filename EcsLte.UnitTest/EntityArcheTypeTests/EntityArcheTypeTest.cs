using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityArcheTypeTests
{
    [TestClass]
    public class EntityArcheTypeTest : BasePrePostTest
    {
        [TestMethod]
        public void HasComponentType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(archeType.HasComponentType<TestComponent1>());
            Assert.IsFalse(archeType.HasComponentType<TestComponent2>());
        }

        [TestMethod]
        public void HasSharedComponentType()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1());

            Assert.IsTrue(archeType.HasSharedComponentType<TestSharedComponent1>());
            Assert.IsFalse(archeType.HasSharedComponentType<TestSharedComponent2>());
        }

        [TestMethod]
        public void HasUniqueComponentType()
        {
            var archeType = new EntityArcheType()
                .AddUniqueComponentType<TestUniqueComponent1>();

            Assert.IsTrue(archeType.HasUniqueComponentType<TestUniqueComponent1>());
            Assert.IsFalse(archeType.HasUniqueComponentType<TestUniqueComponent2>());
        }

        [TestMethod]
        public void HasSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 1 }));
            Assert.IsFalse(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 0 }));
        }

        [TestMethod]
        public void AddComponentType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(archeType.ComponentTypes.Length == 1);
            Assert.IsTrue(archeType.ComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(archeType.HasComponentType<TestComponent1>());
            Assert.IsTrue(archeType.UniqueComponentTypes.Length == 0);
            Assert.IsTrue(archeType.SharedComponents.Length == 0);

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddComponentType<TestComponent1>());
        }

        [TestMethod]
        public void AddUniqueComponentType()
        {
            var archeType = new EntityArcheType()
                .AddUniqueComponentType<TestUniqueComponent1>();

            Assert.IsTrue(archeType.UniqueComponentTypes.Length == 1);
            Assert.IsTrue(archeType.UniqueComponentTypes[0] == typeof(TestUniqueComponent1));
            Assert.IsTrue(archeType.HasUniqueComponentType<TestUniqueComponent1>());
            Assert.IsTrue(archeType.ComponentTypes.Length == 0);
            Assert.IsTrue(archeType.SharedComponents.Length == 0);

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddUniqueComponentType<TestUniqueComponent1>());
        }

        [TestMethod]
        public void GetSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.GetSharedComponent<TestSharedComponent1>().Prop == 1);

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => archeType
                .GetSharedComponent<TestSharedComponent2>());
        }

        [TestMethod]
        public void AddSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.SharedComponents.Length == 1);
            Assert.IsTrue(archeType.SharedComponents[0].GetType() == typeof(TestSharedComponent1));
            Assert.IsTrue(archeType.HasSharedComponentType<TestSharedComponent1>());
            Assert.IsTrue(archeType.GetSharedComponent<TestSharedComponent1>().Prop == 1);
            Assert.IsTrue(archeType.ComponentTypes.Length == 0);
            Assert.IsTrue(archeType.UniqueComponentTypes.Length == 0);

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddSharedComponent(new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1())
                .UpdateSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 1 }));

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => new EntityArcheType()
                .UpdateSharedComponent(new TestSharedComponent2()));
        }

        [TestMethod]
        public void Equals() =>
           AssertClassEquals(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                new EntityArcheType()
                    .AddComponentType<TestComponent2>(),
                null);
    }
}