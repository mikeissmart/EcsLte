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
        public void HasSharedComponentData()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasComponentType<TestSharedComponent1>());
            Assert.IsTrue(archeType.HasSharedComponentData(new TestSharedComponent1 { Prop = 1 }));
            Assert.IsFalse(archeType.HasSharedComponentData(new TestSharedComponent1 { Prop = 0 }));
        }

        [TestMethod]
        public void AddComponentType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(archeType.ComponentTypes.Length == 1);
            Assert.IsTrue(archeType.ComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(archeType.SharedComponents.Length == 0);
        }

        [TestMethod]
        public void AddComponentType_Duplicate()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddComponentType<TestComponent1>());
        }

        [TestMethod]
        public void AddComponentType_SharedComponent()
        {
            var archeType = new EntityArcheType();

            Assert.ThrowsException<EntityArcheTypeSharedComponentException>(() => archeType
                .AddComponentType<TestSharedComponent1>());
        }

        [TestMethod]
        public void RemoveComponentType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .RemoveComponentType<TestComponent1>();

            Assert.IsFalse(archeType.HasComponentType<TestComponent1>());
        }

        [TestMethod]
        public void RemoveComponentType_Never()
        {
            var archeType = new EntityArcheType();

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => archeType
                .RemoveComponentType<TestComponent1>());
        }

        [TestMethod]
        public void RemoveComponentType_Duplicate()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .RemoveComponentType<TestComponent1>();

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => archeType
                .RemoveComponentType<TestComponent1>());
        }

        [TestMethod]
        public void RemoveComponentType_SharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1());

            Assert.ThrowsException<EntityArcheTypeSharedComponentException>(() => archeType
                .RemoveComponentType<TestSharedComponent1>());
        }

        [TestMethod]
        public void GetSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.GetSharedComponent<TestSharedComponent1>().Prop == 1);
        }

        [TestMethod]
        public void GetSharedComponent_Never()
        {
            var archeType = new EntityArcheType();

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => archeType
                .GetSharedComponent<TestSharedComponent1>());
        }

        [TestMethod]
        public void AddSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.ComponentTypes.Length == 1);
            Assert.IsTrue(archeType.ComponentTypes[0] == typeof(TestSharedComponent1));
            Assert.IsTrue(archeType.SharedComponents.Length == 1);
            Assert.IsTrue(((TestSharedComponent1)archeType.SharedComponents[0]).Prop == 1);
        }

        [TestMethod]
        public void AddSharedComponent_Duplicate()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1());

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddSharedComponent(new TestSharedComponent1()));
        }

        [TestMethod]
        public void ReplaceSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1())
                .ReplaceSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponentData(new TestSharedComponent1 { Prop = 1 }));
        }

        [TestMethod]
        public void ReplaceSharedComponent_New()
        {
            var archeType = new EntityArcheType()
                .ReplaceSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponentData(new TestSharedComponent1 { Prop = 1 }));
        }

        [TestMethod]
        public void RemoveSharedComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1())
                .RemoveSharedComponent<TestSharedComponent1>();

            Assert.IsFalse(archeType.HasComponentType<TestSharedComponent1>());
        }

        [TestMethod]
        public void RemoveSharedComponent_Duplicate()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1())
                .RemoveSharedComponent<TestSharedComponent1>();

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => archeType
                .RemoveSharedComponent<TestSharedComponent1>());
        }

        [TestMethod]
        public void RemoveSharedComponent_Never()
        {
            var archeType = new EntityArcheType();

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() => archeType
                .RemoveSharedComponent<TestSharedComponent1>());
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