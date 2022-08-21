using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentHas : BasePrePostTest
    {
        [TestMethod]
        public void HasComponent()
        {
            var entity = Context.Entities.CreateEntity(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>());

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
                Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1()));

            Assert.IsTrue(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.HasSharedComponent<TestSharedComponent1>(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasManagedComponent()
        {
            var entity = Context.Entities.CreateEntity(
                Context.ArcheTypes
                    .AddManagedComponentType<TestManagedComponent1>());

            Assert.IsTrue(Context.Entities.HasManagedComponent<TestManagedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasManagedComponent<TestManagedComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.HasManagedComponent<TestManagedComponent1>(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasManagedComponent<TestManagedComponent1>(entity));
        }
    }
}
