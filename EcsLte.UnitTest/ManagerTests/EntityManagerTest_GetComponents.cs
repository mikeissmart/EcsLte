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
        public void GetComponents_Entities_Blittable_Normal() =>
            Components_Blittable_Normal(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Blittable_Shared() =>
            Components_Blittable_Shared(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Blittable_Unique() =>
            Components_Blittable_Unique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponents<TestComponent1>(new[] { Entity.Null }));
        }

        [TestMethod]
        public void GetComponents_Entities_Manage_Normal() =>
            Components_Manage_Normal(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Manage_NormalShared() =>
            Components_Manage_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Manage_Shared() =>
            Components_Manage_Shared(UnitTestConsts.SmallCount, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_Manage_Unique() =>
            Components_Manage_Unique(1, AssertGetComponents_Entities);

        [TestMethod]
        public void GetComponents_Entities_NoEntity_All() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
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
        public void GetComponents_Entities_Null()
        {
            Entity[] entities = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetComponents<TestComponent1>(entities));
        }

        private void AssertGetComponents_Entities<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new() => AssertGetComponents(
                entityCount,
                c1,
                (entities) => Context.GetComponents<T1>(entities));

        private void AssertGetComponents_Entities<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new() => AssertGetComponents(
                entityCount,
                c1,
                c2,
                (entities) => Context.GetComponents<T1>(entities),
                (entities) => Context.GetComponents<T2>(entities));

        private void AssertGetComponents_Entities<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new() => AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                (entities) => Context.GetComponents<T1>(entities),
                (entities) => Context.GetComponents<T2>(entities),
                (entities) => Context.GetComponents<T3>(entities));

        private void AssertGetComponents_Entities<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new() => AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                c4,
                (entities) => Context.GetComponents<T1>(entities),
                (entities) => Context.GetComponents<T2>(entities),
                (entities) => Context.GetComponents<T3>(entities),
                (entities) => Context.GetComponents<T4>(entities));

        private void AssertGetComponents_Entities<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new() => AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                c4,
                c5,
                c6,
                (entities) => Context.GetComponents<T1>(entities),
                (entities) => Context.GetComponents<T2>(entities),
                (entities) => Context.GetComponents<T3>(entities),
                (entities) => Context.GetComponents<T4>(entities),
                (entities) => Context.GetComponents<T5>(entities),
                (entities) => Context.GetComponents<T6>(entities));

        #endregion Entities

        #region EntityArcheType

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_Normal() =>
            Components_Blittable_Normal(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_Shared() =>
            Components_Blittable_Shared(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Blittable_Unique() =>
            Components_Blittable_Unique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponents<TestComponent1>(new EntityArcheType()
                    .AddComponentType<TestComponent1>()));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_Normal() =>
            Components_Manage_Normal(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_NormalShared() =>
            Components_Manage_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_Shared() =>
            Components_Manage_Shared(UnitTestConsts.SmallCount, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Manage_Unique() =>
            Components_Manage_Unique(1, AssertGetComponents_EntityArcheType);

        [TestMethod]
        public void GetComponents_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetComponents<TestComponent1>(archeType));
        }

        private void AssertGetComponents_EntityArcheType<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1);
            AssertGetComponents(
                entityCount,
                c1,
                (entities) => Context.GetComponents<T1>(archeType));
        }

        private void AssertGetComponents_EntityArcheType<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1)
                .AddComponentTypeOrSharedComponent(c2);
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                (entities) => Context.GetComponents<T1>(archeType),
                (entities) => Context.GetComponents<T2>(archeType));
        }

        private void AssertGetComponents_EntityArcheType<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1)
                .AddComponentTypeOrSharedComponent(c2)
                .AddComponentTypeOrSharedComponent(c3);
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                (entities) => Context.GetComponents<T1>(archeType),
                (entities) => Context.GetComponents<T2>(archeType),
                (entities) => Context.GetComponents<T3>(archeType));
        }

        private void AssertGetComponents_EntityArcheType<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1)
                .AddComponentTypeOrSharedComponent(c2)
                .AddComponentTypeOrSharedComponent(c3)
                .AddComponentTypeOrSharedComponent(c4);
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                c4,
                (entities) => Context.GetComponents<T1>(archeType),
                (entities) => Context.GetComponents<T2>(archeType),
                (entities) => Context.GetComponents<T3>(archeType),
                (entities) => Context.GetComponents<T4>(archeType));
        }

        private void AssertGetComponents_EntityArcheType<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1)
                .AddComponentTypeOrSharedComponent(c2)
                .AddComponentTypeOrSharedComponent(c3)
                .AddComponentTypeOrSharedComponent(c4)
                .AddComponentTypeOrSharedComponent(c5)
                .AddComponentTypeOrSharedComponent(c6);
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                c4,
                c5,
                c6,
                (entities) => Context.GetComponents<T1>(archeType),
                (entities) => Context.GetComponents<T2>(archeType),
                (entities) => Context.GetComponents<T3>(archeType),
                (entities) => Context.GetComponents<T4>(archeType),
                (entities) => Context.GetComponents<T5>(archeType),
                (entities) => Context.GetComponents<T6>(archeType));
        }

        #endregion EntityArcheType

        #region EntityQuery

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_Normal() =>
            Components_Blittable_Normal(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_Shared() =>
            Components_Blittable_Shared(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Blittable_Unique() =>
            Components_Blittable_Unique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponents<TestComponent1>(new EntityQuery()
                    .WhereAllOf<TestComponent1>()));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_Normal() =>
            Components_Manage_Normal(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_NormalShared() =>
            Components_Manage_NormalShared(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_Shared() =>
            Components_Manage_Shared(UnitTestConsts.SmallCount, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Manage_Unique() =>
            Components_Manage_Unique(1, AssertGetComponents_EntityQuery);

        [TestMethod]
        public void GetComponents_EntityQuery_Null()
        {
            EntityQuery query = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetComponents<TestComponent1>(query));
        }

        private void AssertGetComponents_EntityQuery<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>();
            AssertGetComponents(
                entityCount,
                c1,
                (entities) => Context.GetComponents<T1>(query));
        }

        private void AssertGetComponents_EntityQuery<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>()
                .WhereAllOf<T2>();
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                (entities) => Context.GetComponents<T1>(query),
                (entities) => Context.GetComponents<T2>(query));
        }

        private void AssertGetComponents_EntityQuery<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>()
                .WhereAllOf<T2>()
                .WhereAllOf<T3>();
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                (entities) => Context.GetComponents<T1>(query),
                (entities) => Context.GetComponents<T2>(query),
                (entities) => Context.GetComponents<T3>(query));
        }

        private void AssertGetComponents_EntityQuery<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>()
                .WhereAllOf<T2>()
                .WhereAllOf<T3>()
                .WhereAllOf<T4>();
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                c4,
                (entities) => Context.GetComponents<T1>(query),
                (entities) => Context.GetComponents<T2>(query),
                (entities) => Context.GetComponents<T3>(query),
                (entities) => Context.GetComponents<T4>(query));
        }

        private void AssertGetComponents_EntityQuery<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>()
                .WhereAllOf<T2>()
                .WhereAllOf<T3>()
                .WhereAllOf<T4>()
                .WhereAllOf<T5>()
                .WhereAllOf<T6>();
            AssertGetComponents(
                entityCount,
                c1,
                c2,
                c3,
                c4,
                c5,
                c6,
                (entities) => Context.GetComponents<T1>(query),
                (entities) => Context.GetComponents<T2>(query),
                (entities) => Context.GetComponents<T3>(query),
                (entities) => Context.GetComponents<T4>(query),
                (entities) => Context.GetComponents<T5>(query),
                (entities) => Context.GetComponents<T6>(query));
        }

        #endregion EntityQuery

        #region Assert

        private void AssertGetComponents<T1>(int entityCount,
            T1 component1,
            Func<Entity[], T1[]> getAction1)
            where T1 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1));
            AssertHelper_UpdateAndEqualsComponent(entities, getAction1, component1);
        }

        private void AssertGetComponents<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            Func<Entity[], T1[]> getAction1,
            Func<Entity[], T2[]> getAction2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2));
            AssertHelper_UpdateAndEqualsComponent(entities, getAction1, component1);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction2, component2);
        }

        private void AssertGetComponents<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            Func<Entity[], T1[]> getAction1,
            Func<Entity[], T2[]> getAction2,
            Func<Entity[], T3[]> getAction3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3));
            AssertHelper_UpdateAndEqualsComponent(entities, getAction1, component1);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction2, component2);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction3, component3);
        }

        private void AssertGetComponents<T1, T2, T3, T4>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            Func<Entity[], T1[]> getAction1,
            Func<Entity[], T2[]> getAction2,
            Func<Entity[], T3[]> getAction3,
            Func<Entity[], T4[]> getAction4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3)
                .AddComponent(component4));
            AssertHelper_UpdateAndEqualsComponent(entities, getAction1, component1);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction2, component2);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction3, component3);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction4, component4);
        }

        private void AssertGetComponents<T1, T2, T3, T4, T5, T6>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            Func<Entity[], T1[]> getAction1,
            Func<Entity[], T2[]> getAction2,
            Func<Entity[], T3[]> getAction3,
            Func<Entity[], T4[]> getAction4,
            Func<Entity[], T5[]> getAction5,
            Func<Entity[], T6[]> getAction6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3)
                .AddComponent(component4)
                .AddComponent(component5)
                .AddComponent(component6));
            AssertHelper_UpdateAndEqualsComponent(entities, getAction1, component1);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction2, component2);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction3, component3);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction4, component4);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction5, component5);
            AssertHelper_UpdateAndEqualsComponent(entities, getAction6, component6);
        }

        private void AssertHelper_UpdateAndEqualsComponent<T>(Entity[] entities, Func<Entity[], T[]> getAction, T component)
            where T : IComponent, ITestComponent, new()
        {
            if (!(component is ISharedComponent))
            {
                for (var i = 0; i < entities.Length; i++)
                    Context.UpdateComponent(entities[i], new T { Prop = entities[i].Id });
            }
            var components = getAction.Invoke(entities);
            Assert.IsTrue(entities.Length == components.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(
                    component is ISharedComponent
                        ? components[i].Prop == component.Prop
                        : components[i].Prop == entities[i].Id,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Assert

        #region Components

        private void Components_Blittable_Normal(int entityCount, Action<int, TestComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 });

        private void Components_Blittable_NormalShared(int entityCount, Action<int, TestComponent1, TestSharedComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestSharedComponent1 { Prop = 2 });

        private void Components_Blittable_NormalSharedUnique(int entityCount, Action<int, TestComponent1, TestSharedComponent1, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestSharedComponent1 { Prop = 2 }, new TestUniqueComponent1 { Prop = 3 });

        private void Components_Blittable_NormalUnique(int entityCount, Action<int, TestComponent1, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestUniqueComponent1 { Prop = 2 });

        private void Components_Blittable_Shared(int entityCount, Action<int, TestSharedComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestSharedComponent1 { Prop = 1 });

        private void Components_Blittable_SharedUnique(int entityCount, Action<int, TestSharedComponent1, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestSharedComponent1 { Prop = 1 }, new TestUniqueComponent1 { Prop = 2 });

        private void Components_Blittable_Unique(int entityCount, Action<int, TestUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestUniqueComponent1 { Prop = 1 });

        private void Components_BlittableManage_Normal(int entityCount, Action<int, TestComponent1, TestManageComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestManageComponent1 { Prop = 2 });

        private void Components_BlittableManage_NormalShared(int entityCount, Action<int, TestComponent1, TestManageComponent1, TestSharedComponent1, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestManageComponent1 { Prop = 2 }, new TestSharedComponent1 { Prop = 3 }, new TestManageSharedComponent1 { Prop = 4 });

        private void Components_BlittableManage_NormalSharedUnique(int entityCount, Action<int, TestComponent1, TestManageComponent1, TestSharedComponent1, TestManageSharedComponent1, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestManageComponent1 { Prop = 2 }, new TestSharedComponent1 { Prop = 3 }, new TestManageSharedComponent1 { Prop = 4 }, new TestUniqueComponent1 { Prop = 5 }, new TestManageUniqueComponent1 { Prop = 6 });

        private void Components_BlittableManage_NormalUnique(int entityCount, Action<int, TestComponent1, TestManageComponent1, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestComponent1 { Prop = 1 }, new TestManageComponent1 { Prop = 2 }, new TestUniqueComponent1 { Prop = 3 }, new TestManageUniqueComponent1 { Prop = 4 });

        private void Components_BlittableManage_Shared(int entityCount, Action<int, TestSharedComponent1, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestSharedComponent1 { Prop = 1 }, new TestManageSharedComponent1 { Prop = 2 });

        private void Components_BlittableManage_SharedUnique(int entityCount, Action<int, TestSharedComponent1, TestManageSharedComponent1, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestSharedComponent1 { Prop = 1 }, new TestManageSharedComponent1 { Prop = 2 }, new TestUniqueComponent1 { Prop = 3 }, new TestManageUniqueComponent1 { Prop = 4 });

        private void Components_BlittableManage_Unique(int entityCount, Action<int, TestUniqueComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestUniqueComponent1 { Prop = 1 }, new TestManageUniqueComponent1 { Prop = 2 });

        private void Components_Manage_Normal(int entityCount, Action<int, TestManageComponent1> assertAction) =>
                                                                    assertAction.Invoke(entityCount, new TestManageComponent1 { Prop = 1 });

        private void Components_Manage_NormalShared(int entityCount, Action<int, TestManageComponent1, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestManageComponent1 { Prop = 1 }, new TestManageSharedComponent1 { Prop = 2 });

        private void Components_Manage_NormalSharedUnique(int entityCount, Action<int, TestManageComponent1, TestManageSharedComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestManageComponent1 { Prop = 1 }, new TestManageSharedComponent1 { Prop = 2 }, new TestManageUniqueComponent1 { Prop = 3 });

        private void Components_Manage_NormalUnique(int entityCount, Action<int, TestManageComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestManageComponent1 { Prop = 1 }, new TestManageUniqueComponent1 { Prop = 2 });

        private void Components_Manage_Shared(int entityCount, Action<int, TestManageSharedComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestManageSharedComponent1 { Prop = 1 });

        private void Components_Manage_SharedUnique(int entityCount, Action<int, TestManageSharedComponent1, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestManageSharedComponent1 { Prop = 1 }, new TestManageUniqueComponent1 { Prop = 2 });

        private void Components_Manage_Unique(int entityCount, Action<int, TestManageUniqueComponent1> assertAction) =>
            assertAction.Invoke(entityCount, new TestManageUniqueComponent1 { Prop = 1 });

        #endregion Components
    }
}