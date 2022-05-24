using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_GetComponent : BasePrePostTest
    {
        #region GetComponent

        [TestMethod]
        public void GetComponent_Blittable_Normal() =>
            Components_Blittable_Normal(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Blittable_Shared() =>
            Components_Blittable_Shared(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Blittable_Unique() =>
            Components_Blittable_Unique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetComponent_Manage_Normal() =>
            Components_Manage_Normal(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Manage_Shared() =>
            Components_Manage_Shared(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_Manage_Unique() =>
            Components_Manage_Unique(AssertGetComponent);

        [TestMethod]
        public void GetComponent_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.GetComponent<TestComponent1>(Entity.Null));

        private void AssertGetComponent<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1()));
            AssertHelper_Get_UpdateAndEqualsComponent<T1>(entities, 0);
        }

        private void AssertGetComponent<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2()));
            AssertHelper_Get_UpdateAndEqualsComponent<T1>(entities, 0);
            AssertHelper_Get_UpdateAndEqualsComponent<T2>(entities, 1);
        }

        private void AssertGetComponent<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3()));
            AssertHelper_Get_UpdateAndEqualsComponent<T1>(entities, 0);
            AssertHelper_Get_UpdateAndEqualsComponent<T2>(entities, 1);
            AssertHelper_Get_UpdateAndEqualsComponent<T3>(entities, 2);
        }

        private void AssertGetComponent<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4()));
            AssertHelper_Get_UpdateAndEqualsComponent<T1>(entities, 0);
            AssertHelper_Get_UpdateAndEqualsComponent<T2>(entities, 1);
            AssertHelper_Get_UpdateAndEqualsComponent<T3>(entities, 2);
            AssertHelper_Get_UpdateAndEqualsComponent<T4>(entities, 3);
        }

        private void AssertGetComponent<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4())
                .AddComponent(new T5())
                .AddComponent(new T6()));
            AssertHelper_Get_UpdateAndEqualsComponent<T1>(entities, 0);
            AssertHelper_Get_UpdateAndEqualsComponent<T2>(entities, 1);
            AssertHelper_Get_UpdateAndEqualsComponent<T3>(entities, 2);
            AssertHelper_Get_UpdateAndEqualsComponent<T4>(entities, 3);
            AssertHelper_Get_UpdateAndEqualsComponent<T5>(entities, 4);
            AssertHelper_Get_UpdateAndEqualsComponent<T6>(entities, 5);
        }

        private void AssertHelper_Get_UpdateAndEqualsComponent<T>(Entity[] entities, int addNum)
            where T : IComponent, ITestComponent, new()
        {
            for (var i = 0; i < entities.Length; i++)
                Context.UpdateComponent(entities[i], new T() { Prop = entities[i].Id + addNum });
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T>(entities[i]).Prop == entities[i].Id + addNum,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion GetComponent

        #region GetAllComponents

        [TestMethod]
        public void GetAllComponents_Blittable_Normal() =>
            Components_Blittable_Normal(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Blittable_Shared() =>
            Components_Blittable_Shared(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Blittable_Unique() =>
            Components_Blittable_Unique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetAllComponents(Entity.Null));
        }

        [TestMethod]
        public void GetAllComponents_Manage_Normal() =>
            Components_Manage_Normal(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Manage_Shared() =>
            Components_Manage_Shared(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_Manage_Unique() =>
            Components_Manage_Unique(AssertGetAll);

        [TestMethod]
        public void GetAllComponents_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.GetAllComponents(Entity.Null));

        private void AssertGetAll<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1()));
            var componentCount = 1;
            var archeType = Context.GetEntityArcheType(entities[0]);
            AssertHelper_GetAll_UpdateAndEqualsComponent<T1>(entities, 0, componentCount, archeType.ComponentTypes
                .Where(x => x == typeof(T1))
                .Select((x, i) => i)
                .First());
        }

        private void AssertGetAll<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2()));
            var componentCount = 2;
            var archeType = Context.GetEntityArcheType(entities[0]);
            AssertHelper_GetAll_UpdateAndEqualsComponent<T1>(entities, 0, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T1))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T2>(entities, 1, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T2))
                .Select(x => x.i)
                .First());
        }

        private void AssertGetAll<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3()));
            var componentCount = 3;
            var archeType = Context.GetEntityArcheType(entities[0]);
            AssertHelper_GetAll_UpdateAndEqualsComponent<T1>(entities, 0, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T1))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T2>(entities, 1, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T2))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T3>(entities, 2, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T3))
                .Select(x => x.i)
                .First());
        }

        private void AssertGetAll<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4()));
            var componentCount = 4;
            var archeType = Context.GetEntityArcheType(entities[0]);
            AssertHelper_GetAll_UpdateAndEqualsComponent<T1>(entities, 0, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T1))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T2>(entities, 1, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T2))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T3>(entities, 2, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T3))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T4>(entities, 3, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T4))
                .Select(x => x.i)
                .First());
        }

        private void AssertGetAll<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4())
                .AddComponent(new T5())
                .AddComponent(new T6()));
            var componentCount = 6;
            var archeType = Context.GetEntityArcheType(entities[0]);
            AssertHelper_GetAll_UpdateAndEqualsComponent<T1>(entities, 0, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T1))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T2>(entities, 1, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T2))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T3>(entities, 2, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T3))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T4>(entities, 3, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T4))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T5>(entities, 5, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T5))
                .Select(x => x.i)
                .First());
            AssertHelper_GetAll_UpdateAndEqualsComponent<T6>(entities, 6, componentCount, archeType.ComponentTypes
                .Select((x, i) => (x, i))
                .Where(x => x.x == typeof(T6))
                .Select(x => x.i)
                .First());
        }

        private void AssertHelper_GetAll_UpdateAndEqualsComponent<T>(Entity[] entities, int addNum, int componentCount, int componentIndex)
            where T : IComponent, ITestComponent, new()
        {
            for (var i = 0; i < entities.Length; i++)
                Context.UpdateComponent(entities[i], new T() { Prop = entities[i].Id + addNum });
            for (var i = 0; i < entities.Length; i++)
            {
                var components = Context.GetAllComponents(entities[i]);
                Assert.IsTrue(components.Length == componentCount);
                if (components[componentIndex] is T com1)
                    Assert.IsTrue(com1.Prop == entities[i].Id + addNum, $"Enity.Id {entities[i].Id}");
                else
                    Assert.IsTrue(false, $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion GetAllComponents

        #region HasComponent

        [TestMethod]
        public void HasComponent_Blittable_Normal() =>
            Components_Blittable_Normal(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Blittable_Shared() =>
            Components_Blittable_Shared(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Blittable_Unique() =>
            Components_Blittable_Unique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void HasComponent_Manage_Normal() =>
            Components_Manage_Normal(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Manage_Shared() =>
            Components_Manage_Shared(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_Manage_Unique() =>
            Components_Manage_Unique(AssertHasComponent);

        [TestMethod]
        public void HasComponent_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.HasComponent<TestComponent1>(Entity.Null));

        private void AssertHasComponent<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1()));
            AssertHelper_Has_Component<T1>(entities);
        }

        private void AssertHasComponent<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2()));
            AssertHelper_Has_Component<T1>(entities);
            AssertHelper_Has_Component<T2>(entities);
        }

        private void AssertHasComponent<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3()));
            AssertHelper_Has_Component<T1>(entities);
            AssertHelper_Has_Component<T2>(entities);
            AssertHelper_Has_Component<T3>(entities);
        }

        private void AssertHasComponent<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4()));
            AssertHelper_Has_Component<T1>(entities);
            AssertHelper_Has_Component<T2>(entities);
            AssertHelper_Has_Component<T3>(entities);
            AssertHelper_Has_Component<T4>(entities);
        }

        private void AssertHasComponent<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4())
                .AddComponent(new T5())
                .AddComponent(new T6()));
            AssertHelper_Has_Component<T1>(entities);
            AssertHelper_Has_Component<T2>(entities);
            AssertHelper_Has_Component<T3>(entities);
            AssertHelper_Has_Component<T4>(entities);
            AssertHelper_Has_Component<T5>(entities);
            AssertHelper_Has_Component<T6>(entities);
        }

        private void AssertHelper_Has_Component<T>(Entity[] entities)
            where T : IComponent, ITestComponent, new()
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.HasComponent<T>(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion HasComponent

        #region GetUniqueComponent

        [TestMethod]
        public void Unique_GetUniqueComponent_Blittable()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetUniqueComponent<TestUniqueComponent1>().Prop == component.Prop);
        }

        [TestMethod]
        public void Unique_GetUniqueComponent_BlittableManage()
        {
            var component1 = new TestUniqueComponent1 { Prop = 1 };
            var component2 = new TestManageUniqueComponent1 { Prop = 2 };
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2));

            Assert.IsTrue(Context.GetUniqueComponent<TestUniqueComponent1>().Prop == component1.Prop);
            Assert.IsTrue(Context.GetUniqueComponent<TestManageUniqueComponent1>().Prop == component2.Prop);
        }

        [TestMethod]
        public void Unique_GetUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueComponent_Manage()
        {
            var component = new TestManageUniqueComponent1 { Prop = 1 };
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetUniqueComponent<TestManageUniqueComponent1>().Prop == component.Prop);
        }

        #endregion GetUniqueComponent

        #region GetUniqueEntity

        [TestMethod]
        public void Unique_GetUniqueEntity_Blittable()
        {
            ;
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));

            Assert.IsTrue(Context.GetUniqueEntity<TestUniqueComponent1>() == entity);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity_BlittableManage()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1())
                .AddComponent(new TestManageUniqueComponent1()));

            Assert.IsTrue(Context.GetUniqueEntity<TestUniqueComponent1>() == entity);
            Assert.IsTrue(Context.GetUniqueEntity<TestManageUniqueComponent1>() == entity);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueEntity_Manage()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestManageUniqueComponent1()));

            Assert.IsTrue(Context.GetUniqueEntity<TestManageUniqueComponent1>() == entity);
        }

        #endregion GetUniqueEntity

        #region Components

        private void Components_Blittable_Normal(Action<int, TestComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestComponent1());

        private void Components_Blittable_NormalShared(Action<int, TestComponent1, TestSharedComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestComponent1(), new TestSharedComponent1());

        private void Components_Blittable_NormalSharedUnique(Action<int, TestComponent1, TestSharedComponent1, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestComponent1(), new TestSharedComponent1(), new TestUniqueComponent1());

        private void Components_Blittable_NormalUnique(Action<int, TestComponent1, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestComponent1(), new TestUniqueComponent1());

        private void Components_Blittable_Shared(Action<int, TestSharedComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestSharedComponent1());

        private void Components_Blittable_SharedUnique(Action<int, TestSharedComponent1, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestSharedComponent1(), new TestUniqueComponent1());

        private void Components_Blittable_Unique(Action<int, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestUniqueComponent1());

        private void Components_BlittableManage_Normal(Action<int, TestComponent1, TestManageComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestComponent1(), new TestManageComponent1());

        private void Components_BlittableManage_NormalShared(Action<int, TestComponent1, TestManageComponent1, TestSharedComponent1, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestComponent1(), new TestManageComponent1(), new TestSharedComponent1(), new TestManageSharedComponent1());

        private void Components_BlittableManage_NormalSharedUnique(Action<int, TestComponent1, TestManageComponent1, TestSharedComponent1, TestManageSharedComponent1, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestComponent1(), new TestManageComponent1(), new TestSharedComponent1(), new TestManageSharedComponent1(), new TestUniqueComponent1(), new TestManageUniqueComponent1());

        private void Components_BlittableManage_NormalUnique(Action<int, TestComponent1, TestManageComponent1, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestComponent1(), new TestManageComponent1(), new TestUniqueComponent1(), new TestManageUniqueComponent1());

        private void Components_BlittableManage_Shared(Action<int, TestSharedComponent1, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestSharedComponent1(), new TestManageSharedComponent1());

        private void Components_BlittableManage_SharedUnique(Action<int, TestSharedComponent1, TestManageSharedComponent1, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestSharedComponent1(), new TestManageSharedComponent1(), new TestUniqueComponent1(), new TestManageUniqueComponent1());

        private void Components_BlittableManage_Unique(Action<int, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestUniqueComponent1(), new TestManageUniqueComponent1());

        private void Components_Manage_Normal(Action<int, TestManageComponent1> assertAction) =>
                                                                    assertAction.Invoke(UnitTestConsts.SmallCount, new TestManageComponent1());

        private void Components_Manage_NormalShared(Action<int, TestManageComponent1, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestManageComponent1(), new TestManageSharedComponent1());

        private void Components_Manage_NormalSharedUnique(Action<int, TestManageComponent1, TestManageSharedComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestManageComponent1(), new TestManageSharedComponent1(), new TestManageUniqueComponent1());

        private void Components_Manage_NormalUnique(Action<int, TestManageComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestManageComponent1(), new TestManageUniqueComponent1());

        private void Components_Manage_Shared(Action<int, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(UnitTestConsts.SmallCount, new TestManageSharedComponent1());

        private void Components_Manage_SharedUnique(Action<int, TestManageSharedComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestManageSharedComponent1(), new TestManageUniqueComponent1());

        private void Components_Manage_Unique(Action<int, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(1, new TestManageUniqueComponent1());

        #endregion Components
    }
}