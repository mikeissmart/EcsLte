using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_GetComponent : BasePrePostTest
    {
        [TestMethod]
        public void GetComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetComponent_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.GetComponent<TestComponent1>(Entity.Null));

        [TestMethod]
        public void GetComponent_Normal()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == component.Prop);
        }

        [TestMethod]
        public void GetComponent_Shared()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == component.Prop);
        }

        [TestMethod]
        public void GetComponent_Unique()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetComponent<TestUniqueComponent1>(entity).Prop == component.Prop);
        }

        [TestMethod]
        public void GetAllComponents_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetAllComponents(Entity.Null));
        }

        [TestMethod]
        public void GetAllComponents_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.GetAllComponents(Entity.Null));

        [TestMethod]
        public void GetAllComponents()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestSharedComponent1 { Prop = 2 })
                .AddComponent(new TestUniqueComponent1 { Prop = 3 }));

            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == 2);
            Assert.IsTrue(Context.GetComponent<TestUniqueComponent1>(entity).Prop == 3);
        }

        [TestMethod]
        public void HasComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void HasComponent_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.HasComponent<TestComponent1>(Entity.Null));

        [TestMethod]
        public void HasComponent()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestSharedComponent1 { Prop = 2 })
                .AddComponent(new TestUniqueComponent1 { Prop = 3 }));

            Assert.IsTrue(Context.HasComponent<TestComponent1>(entity));
            Assert.IsTrue(Context.HasComponent<TestSharedComponent1>(entity));
            Assert.IsTrue(Context.HasComponent<TestUniqueComponent1>(entity));
            Assert.IsFalse(Context.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.HasComponent<TestSharedComponent2>(entity));
            Assert.IsFalse(Context.HasComponent<TestUniqueComponent2>(entity));
        }

        [TestMethod]
        public void GetUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetUniqueComponent<TestUniqueComponent1>().Prop == component.Prop);
        }

        [TestMethod]
        public void GetUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void GetUniqueEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));

            Assert.IsTrue(Context.GetUniqueEntity<TestUniqueComponent1>() == entity);
        }

        [TestMethod]
        public void GetUniqueEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueEntity<TestUniqueComponent1>());
        }
    }
}