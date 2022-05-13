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
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType()
                .AddComponent<TestComponent1>();

            Assert.IsTrue(archeType.HasComponent<TestComponent1>());
            Assert.IsFalse(archeType.HasComponent<TestComponent2>());
        }

        [TestMethod]
        public void HasSharedComponent()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 1 }));
            Assert.IsFalse(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 0 }));
        }

        [TestMethod]
        public void AddComponent_Duplicate()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType()
                .AddComponent<TestComponent1>();

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddComponent<TestComponent1>());
        }

        [TestMethod]
        public void AddComponent_SharedComponent()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType();

            Assert.ThrowsException<EntityArcheTypeSharedComponentException>(() => archeType
                .AddComponent<TestSharedComponent1>());
        }

        [TestMethod]
        public void AddSharedComponent_Duplicate()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType()
                .AddSharedComponent(new TestSharedComponent1());

            Assert.ThrowsException<EntityArcheTypeAlreadyHasComponentException>(() => archeType
                .AddSharedComponent(new TestSharedComponent1()));
        }

        [TestMethod]
        public void ReplaceSharedComponent()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType()
                .AddSharedComponent(new TestSharedComponent1())
                .ReplaceSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 1 }));
        }

        [TestMethod]
        public void ReplaceSharedComponent_New()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType()
                .ReplaceSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(archeType.HasSharedComponent(new TestSharedComponent1 { Prop = 1 }));
        }
    }
}
