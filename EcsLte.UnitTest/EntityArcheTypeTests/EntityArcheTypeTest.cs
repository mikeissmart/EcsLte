using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityArcheTypeTests
{
    [TestClass]
    public class EntityArcheTypeTest : BasePrePostTest
    {
        [TestMethod]
        public void HasComponent()
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(archeType.HasComponent<TestComponent1>());
            Assert.IsFalse(archeType.HasComponent<TestComponent2>());
        }

        [TestMethod]
        public void HasSharedComponent()
        {
            var archeType = Context.ArcheTypes
                .AddSharedComponent(new TestSharedComponent1());

            Assert.IsTrue(archeType.HasSharedComponent<TestSharedComponent1>());
            Assert.IsFalse(archeType.HasSharedComponent<TestSharedComponent2>());
        }

        [TestMethod]
        public void HasManagedComponent()
        {
            var archeType = Context.ArcheTypes
                .AddManagedComponentType<TestManagedComponent1>();

            Assert.IsTrue(archeType.HasManagedComponent<TestManagedComponent1>());
            Assert.IsFalse(archeType.HasManagedComponent<TestManagedComponent2>());
        }

        [TestMethod]
        public void AddComponentType()
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(archeType.ComponentTypes.Length == 1);
            Assert.IsTrue(archeType.ManagedComponentTypes.Length == 0);
            Assert.IsTrue(archeType.SharedComponentTypes.Length == 0);
            Assert.IsTrue(archeType.SharedComponents.Length == 0);

            Assert.ThrowsException<ComponentAlreadyHaveException>(() => archeType
                .AddComponentType<TestComponent1>());
        }

        [TestMethod]
        public void AddManagedComponentType()
        {
            var archeType = Context.ArcheTypes
                .AddManagedComponentType<TestManagedComponent1>();

            Assert.IsTrue(archeType.ComponentTypes.Length == 0);
            Assert.IsTrue(archeType.ManagedComponentTypes.Length == 1);
            Assert.IsTrue(archeType.SharedComponentTypes.Length == 0);
            Assert.IsTrue(archeType.SharedComponents.Length == 0);

            Assert.ThrowsException<ComponentAlreadyHaveException>(() => archeType
                .AddManagedComponentType<TestManagedComponent1>());
        }

        [TestMethod]
        public void GetSharedComponent()
        {
            var archeType = Context.ArcheTypes
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.GetSharedComponent<TestSharedComponent1>().Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() => archeType
                .GetSharedComponent<TestSharedComponent2>());
        }

        [TestMethod]
        public void AddSharedComponent()
        {
            var archeType = Context.ArcheTypes
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.ComponentTypes.Length == 0);
            Assert.IsTrue(archeType.ManagedComponentTypes.Length == 0);
            Assert.IsTrue(archeType.SharedComponentTypes.Length == 1);
            Assert.IsTrue(archeType.SharedComponents.Length == 1);

            Assert.ThrowsException<ComponentAlreadyHaveException>(() => archeType
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }));
        }

        [TestMethod]
        public void UpdateSharedComponent()
        {
            var archeType = Context.ArcheTypes
                .AddSharedComponent(new TestSharedComponent1())
                .UpdateSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.GetSharedComponent<TestSharedComponent1>().Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() => archeType
                .UpdateSharedComponent(new TestSharedComponent2()));
        }

        [TestMethod]
        public void Equals() =>
           AssertClassEquals(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                Context.ArcheTypes
                    .AddComponentType<TestComponent2>(),
                null);
    }
}