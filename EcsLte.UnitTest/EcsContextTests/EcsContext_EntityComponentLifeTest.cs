using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_EntityComponentLifeTest : BasePrePostTest, IEntityComponentLifeTest
    {
        private TestComponent1 _testComponent1_1 = new TestComponent1 { Prop = 1 };
        private TestSharedComponent1 _testSharedComponent1_1 = new TestSharedComponent1 { Prop = 2 };
        private TestUniqueComponent1 _testUniqueComponent1_1 = new TestUniqueComponent1 { Prop = 3 };
        private TestComponent1 _testComponent1_2 = new TestComponent1 { Prop = 4 };
        private TestSharedComponent1 _testSharedComponent1_2 = new TestSharedComponent1 { Prop = 5 };
        private TestUniqueComponent1 _testUniqueComponent1_2 = new TestUniqueComponent1 { Prop = 6 };

        [TestMethod]
        public void UpdateComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponent(Entity.Null, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Normal() => AssertUpdateComponent(1,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Normal_Large() => AssertUpdateComponent(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Normal_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.UpdateComponent(entity, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_NormalShared() => AssertUpdateComponent(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_NormalShared_Large() => AssertUpdateComponent(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_NormalSharedUnique() => AssertUpdateComponent(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_NormalUnique() => AssertUpdateComponent(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_NormalUniqueShared() => AssertUpdateComponent(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Shared() => AssertUpdateComponent(1,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Shared_Large() => AssertUpdateComponent(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Shared_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.UpdateComponent(entity, _testSharedComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_SharedNormal() => AssertUpdateComponent(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_SharedNormal_Large() => AssertUpdateComponent(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_SharedNormalUnique() => AssertUpdateComponent(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_SharedUnique() => AssertUpdateComponent(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_SharedUniqueNormal() => AssertUpdateComponent(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Unique() => AssertUpdateComponent(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Unique_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.UpdateComponent(entity, _testUniqueComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_UniqueNormal() => AssertUpdateComponent(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_UniqueNormalShared() => AssertUpdateComponent(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_UniqueShared() => AssertUpdateComponent(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_UniqueSharedNormal() => AssertUpdateComponent(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Normal() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Normal_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalShared() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalShared_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalSharedUnique() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalUnique() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalUniqueShared() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Shared() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Shared_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormal() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormal_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormalUnique() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedUnique() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedUniqueNormal() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Unique() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueNormal() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueNormalShared() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueShared() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueSharedNormal() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        private void AssertUpdateComponent<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
                Context.UpdateComponent(entities[i], updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.UpdateComponent(entities[i], updateComponent1);
                Context.UpdateComponent(entities[i], updateComponent2);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.UpdateComponent(entities[i], updateComponent1);
                Context.UpdateComponent(entities[i], updateComponent2);
                Context.UpdateComponent(entities[i], updateComponent3);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);
            var query = Context.CreateQuery()
                .WhereAllOf<T1>();

            Context.UpdateComponent(query, updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);
            var query = Context.CreateQuery()
                .WhereAllOf<T1, T2>();

            Context.UpdateComponent(query, updateComponent1);
            Context.UpdateComponent(query, updateComponent2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);
            var query = Context.CreateQuery()
                .WhereAllOf<T1, T2, T3>();

            Context.UpdateComponent(query, updateComponent1);
            Context.UpdateComponent(query, updateComponent2);
            Context.UpdateComponent(query, updateComponent3);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.UpdateComponent(entities[i], updateComponent1);
                Context.UpdateComponent(entities[i], updateComponent2);
                Context.UpdateComponent(entities[i], updateComponent3);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }
    }
}
