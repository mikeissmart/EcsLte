using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_GetComponents : BasePrePostTest
    {
        #region Entities

        [TestMethod]
        public void GetComponents_Entities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponents<TestComponent1>(new[] { Entity.Null }));
        }

        [TestMethod]
        public void GetComponents_Entities_Null()
        {
            Entity[] entities = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetComponents<TestComponent1>(entities));
        }

        [TestMethod]
        public void GetComponents_Entities_NoEntity_All() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.GetComponents<TestComponent1>(new[] { Entity.Null, Entity.Null }));

        [TestMethod]
        public void GetComponents_Entities_NoEntity_Some()
        {
            var entities = new[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())),
                Entity.Null
            };
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.GetComponents<TestComponent1>(entities));
        }

        [TestMethod]
        public void GetComponents_Entities_Normal()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestComponent1());

            UpdateComponent<TestComponent1>(entities);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(entities));
        }

        [TestMethod]
        public void GetComponents_Entities_NormalShared()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestComponent1(),
                new TestSharedComponent1());

            UpdateComponent<TestComponent1>(entities);
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(entities));
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(entities),
                sharedComponent);
        }

        [TestMethod]
        public void GetComponents_Entities_NormalSharedUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestSharedComponent1(),
                new TestUniqueComponent1());

            UpdateComponent<TestComponent1>(entities);
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);
            UpdateComponent<TestUniqueComponent1>(entities);

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(entities));
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(entities),
                sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(entities));
        }

        [TestMethod]
        public void GetComponents_Entities_NormalUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestUniqueComponent1());

            UpdateComponent<TestComponent1>(entities);
            UpdateComponent<TestUniqueComponent1>(entities);

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(entities));
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(entities));
        }

        [TestMethod]
        public void GetComponents_Entities_Shared()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestSharedComponent1());

            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);

            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(entities),
                sharedComponent);
        }

        [TestMethod]
        public void GetComponents_Entities_SharedUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestSharedComponent1(),
                new TestUniqueComponent1());

            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);
            UpdateComponent<TestUniqueComponent1>(entities);

            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(entities),
                sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(entities));
        }

        [TestMethod]
        public void GetComponents_Entities_Unique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestUniqueComponent1());

            UpdateComponent<TestUniqueComponent1>(entities);

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(entities));
        }

        #endregion Entities

        #region EntityArcheType

        [TestMethod]
        public void GetComponents_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponents<TestComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetComponents<TestComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Normal()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestComponent1());

            UpdateComponent<TestComponent1>(entities);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_NormalShared()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestComponent1(),
                new TestSharedComponent1());

            UpdateComponent<TestComponent1>(entities);
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(archeType));
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(archeType),
                sharedComponent);
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_NormalSharedUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestSharedComponent1(),
                new TestUniqueComponent1());

            UpdateComponent<TestComponent1>(entities);
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);
            UpdateComponent<TestUniqueComponent1>(entities);

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(sharedComponent)
                .AddComponentType<TestUniqueComponent1>();
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(archeType));
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(archeType),
                sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_NormalUnique()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>();
            var entities = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestUniqueComponent1());

            UpdateComponent<TestComponent1>(entities);
            UpdateComponent<TestUniqueComponent1>(entities);

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(archeType));
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Shared()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestSharedComponent1());

            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);

            var archeType = new EntityArcheType()
                .AddSharedComponent(sharedComponent);
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(archeType),
                sharedComponent);
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_SharedUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestSharedComponent1(),
                new TestUniqueComponent1());

            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);
            UpdateComponent<TestUniqueComponent1>(entities);

            var archeType = new EntityArcheType()
                .AddSharedComponent(sharedComponent)
                .AddComponentType<TestUniqueComponent1>();
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(archeType),
                sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Unique()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>();
            var entities = TestCreateEntities(Context, 1,
                new TestUniqueComponent1());

            UpdateComponent<TestUniqueComponent1>(entities);

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(archeType));
        }

        #endregion EntityArcheType

        #region EntityQuery

        [TestMethod]
        public void GetComponents_EntityQuery_Destroyed()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponents<TestComponent1>(query));
        }

        /*[TestMethod]
        public void GetComponents_EntityQuery_Different_Context()
        {
            var context2 = EcsContexts.CreateContext("EntityArcheType_Diff");
            var archeType = new EntityArcheType(context2);

            Assert.ThrowsException<EcsContextDifferentException>(() =>
                Context.GetComponents<TestComponent1>(archeType));
        }*/

        [TestMethod]
        public void GetComponents_EntityQuery_Null()
        {
            EntityQuery query = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetComponents<TestComponent1>(query));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_Normal()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestComponent1());

            UpdateComponent<TestComponent1>(entities);

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(query));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_NormalShared()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestComponent1(),
                new TestSharedComponent1());

            UpdateComponent<TestComponent1>(entities);
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>()
                .FilterBy(sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(query));
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(query),
                sharedComponent);
        }

        [TestMethod]
        public void GetComponents_EntityQuery_NormalSharedUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestSharedComponent1(),
                new TestUniqueComponent1());

            UpdateComponent<TestComponent1>(entities);
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);
            UpdateComponent<TestUniqueComponent1>(entities);

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>()
                .FilterBy(sharedComponent)
                .WhereAllOf<TestUniqueComponent1>();
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(query));
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(query),
                sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(query));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_NormalUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestUniqueComponent1());

            UpdateComponent<TestComponent1>(entities);
            UpdateComponent<TestUniqueComponent1>(entities);

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>()
                .WhereAllOf<TestUniqueComponent1>();
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestComponent1>(query));
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(query));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_Shared()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount,
                new TestSharedComponent1());

            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);

            var query = new EntityQuery()
                .FilterBy(sharedComponent);
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(query),
                sharedComponent);
        }

        [TestMethod]
        public void GetComponents_EntityQuery_SharedUnique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestSharedComponent1(),
                new TestUniqueComponent1());

            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            UpdateComponentShared(entities, sharedComponent);
            UpdateComponent<TestUniqueComponent1>(entities);

            var query = new EntityQuery()
                .FilterBy(sharedComponent)
                .WhereAllOf<TestUniqueComponent1>();
            AssertHelper_EqualsSharedComponents(entities, Context.GetComponents<TestSharedComponent1>(query),
                sharedComponent);
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(query));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_Unique()
        {
            var entities = TestCreateEntities(Context, 1,
                new TestUniqueComponent1());

            UpdateComponent<TestUniqueComponent1>(entities);

            var query = new EntityQuery()
                .WhereAllOf<TestUniqueComponent1>();
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestUniqueComponent1>(query));
        }

        #endregion EntityQuery

        private void UpdateComponent<T>(Entity[] entities)
            where T : unmanaged, IComponent, ITestComponent
        {
            if (new T() is ISharedComponent)
                throw new Exception("Use UpdateComponentShared for shared components");

            for (var i = 0; i < entities.Length; i++)
                Context.UpdateComponent(entities[i], new T { Prop = entities[i].Id });
        }

        private void UpdateComponentShared<T>(Entity[] entities, T sharedComponent)
            where T : unmanaged, ISharedComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
                Context.UpdateComponent(entities[i], sharedComponent);
        }

        private void AssertHelper_EqualsComponents<T>(Entity[] entities, T[] components)
            where T : unmanaged, IComponent, ITestComponent
        {
            if (new T() is ISharedComponent)
                throw new Exception("Use UpdateComponentShared for shared components");

            Assert.IsTrue(entities.Length == components.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(components[i].Prop == entities[i].Id,
                    $"Enity.Id {entities[i].Id}, {typeof(T)}");
            }
        }

        private void AssertHelper_EqualsSharedComponents<T>(Entity[] entities, T[] components, T sharedComponent)
            where T : unmanaged, ISharedComponent, ITestComponent
        {
            Assert.IsTrue(entities.Length == components.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(components[i].Prop == sharedComponent.Prop,
                    $"Enity.Id {entities[i].Id}, {typeof(T)}");
            }
        }
    }
}