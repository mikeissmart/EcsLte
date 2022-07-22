using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentHas : BasePrePostTest
    {
        [TestMethod]
        public void HasComponent()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.HasComponent<TestComponent1>(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasSharedComponent()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1()),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.HasSharedComponent<TestSharedComponent1>(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasUniqueComponent()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddUniqueComponentType<TestUniqueComponent1>(),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.HasUniqueComponent<TestUniqueComponent1>(entity));
            Assert.IsTrue(Context.Entities.HasUniqueComponent<TestUniqueComponent1>());
            Assert.IsFalse(Context.Entities.HasUniqueComponent<TestUniqueComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasUniqueComponent<TestUniqueComponent2>());

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.HasUniqueComponent<TestUniqueComponent1>(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasUniqueComponent<TestUniqueComponent1>(entity));
        }
    }
}
