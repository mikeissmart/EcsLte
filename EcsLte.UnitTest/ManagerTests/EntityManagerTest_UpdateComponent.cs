using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_UpdateComponent : BasePrePostTest
    {
        #region Entity

        [TestMethod]
        public void UpdateComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponent(Entity.Null, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponent_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.UpdateComponent(Entity.Null, new TestComponent1()));

        [TestMethod]
        public void UpdateComponent_NotHaveComponent()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.UpdateComponent(entity, new TestComponent2()));
        }

        [TestMethod]
        public void UpdateComponent_Normal()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));

            Context.UpdateComponent(entity, new TestComponent1 { Prop = 2 });

            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == 2);
        }

        [TestMethod]
        public void UpdateComponent_NormalShared()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestSharedComponent1 { Prop = 2 }));

            Context.UpdateComponent(entity, new TestComponent1 { Prop = 2 });
            Context.UpdateComponent(entity, new TestSharedComponent1 { Prop = 3 });

            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == 2);
            Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == 3);
        }

        [TestMethod]
        public void UpdateComponent_NormalSharedUnique()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestSharedComponent1 { Prop = 2 })
                .AddComponent(new TestUniqueComponent1 { Prop = 3 }));

            Context.UpdateComponent(entity, new TestComponent1 { Prop = 2 });
            Context.UpdateComponent(entity, new TestSharedComponent1 { Prop = 3 });
            Context.UpdateComponent(entity, new TestUniqueComponent1 { Prop = 4 });

            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == 2);
            Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == 3);
            Assert.IsTrue(Context.GetComponent<TestUniqueComponent1>(entity).Prop == 4);
        }

        [TestMethod]
        public void UpdateComponent_NormalUnique()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestUniqueComponent1 { Prop = 2 }));

            Context.UpdateComponent(entity, new TestComponent1 { Prop = 2 });
            Context.UpdateComponent(entity, new TestUniqueComponent1 { Prop = 3 });

            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == 2);
            Assert.IsTrue(Context.GetComponent<TestUniqueComponent1>(entity).Prop == 3);
        }

        [TestMethod]
        public void UpdateComponent_Shared()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestSharedComponent1 { Prop = 1 }));

            Context.UpdateComponent(entity, new TestSharedComponent1 { Prop = 2 });

            Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == 2);
        }

        [TestMethod]
        public void UpdateComponent_SharedUnique()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestSharedComponent1 { Prop = 1 })
                .AddComponent(new TestUniqueComponent1 { Prop = 2 }));

            Context.UpdateComponent(entity, new TestSharedComponent1 { Prop = 2 });
            Context.UpdateComponent(entity, new TestUniqueComponent1 { Prop = 3 });

            Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == 2);
            Assert.IsTrue(Context.GetComponent<TestUniqueComponent1>(entity).Prop == 3);
        }

        [TestMethod]
        public void UpdateComponent_Unique()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            Context.UpdateComponent(entity, new TestUniqueComponent1 { Prop = 2 });

            Assert.IsTrue(Context.GetComponent<TestUniqueComponent1>(entity).Prop == 2);
        }

        #endregion Entity

        #region Entities

        [TestMethod]
        public void UpdateSharedComponent_Entities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponent(Entity.Null, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_Entities_NoEntity_All() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.UpdateSharedComponent(new[] { Entity.Null, Entity.Null }, new TestSharedComponent1()));

        [TestMethod]
        public void UpdateSharedComponent_Entities_NoEntity_Some()
        {
            var entities = new[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestSharedComponent1())),
                Entity.Null
            };
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.UpdateSharedComponent(entities, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_Entities_NotHaveComponent()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestSharedComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.UpdateSharedComponent(entities, new TestSharedComponent2()));
        }

        [TestMethod]
        public void UpdateSharedComponent_Entities()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestSharedComponent1()));

            Context.UpdateSharedComponent(entities, new TestSharedComponent1 { Prop = 2 });

            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestSharedComponent1>(entities),
                new TestSharedComponent1 { Prop = 2 });
        }

        #endregion Entities

        #region EntityArcheType

        [TestMethod]
        public void UpdateSharedComponent_EntityArcheType_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateSharedComponent(new EntityArcheType(), new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.UpdateSharedComponent(archeType, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_EntityArcheType_NotHaveComponent()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1());

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() =>
                Context.UpdateSharedComponent(archeType, new TestSharedComponent2()));
        }

        [TestMethod]
        public void UpdateSharedComponent_EntityArcheType()
        {
            var entities = Context.CreateEntities(UnitTestConsts.SmallCount, new EntityBlueprint()
                .AddComponent(new TestSharedComponent1 { Prop = 1 }));

            var archeType1 = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });
            Context.UpdateSharedComponent(archeType1, new TestSharedComponent1 { Prop = 2 });

            var archeType2 = archeType1.ReplaceSharedComponent(new TestSharedComponent1 { Prop = 2 });
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestSharedComponent1>(archeType2),
                new TestSharedComponent1 { Prop = 2 });
        }

        #endregion EntityArcheType

        #region EntityQuery

        [TestMethod]
        public void UpdateSharedComponent_EntityQuery_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateSharedComponent(new EntityQuery(), new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_EntityQuery_Null()
        {
            EntityQuery query = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.UpdateSharedComponent(query, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_EntityQuery_NotHaveComponent()
        {
            var query = new EntityQuery()
                .FilterBy(new TestSharedComponent1());

            Assert.ThrowsException<EntityQueryNotHaveWhereOfAllException>(() =>
                Context.UpdateSharedComponent(query, new TestSharedComponent2()));
        }

        [TestMethod]
        public void UpdateSharedComponent_EntityQuery()
        {
            var entities = Context.CreateEntities(UnitTestConsts.SmallCount, new EntityBlueprint()
                .AddComponent(new TestSharedComponent1 { Prop = 1 }));

            var query1 = new EntityQuery()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });
            Context.UpdateSharedComponent(query1, new TestSharedComponent1 { Prop = 2 });

            var query2 = query1.FilterByReplace(new TestSharedComponent1 { Prop = 2 });
            AssertHelper_EqualsComponents(entities, Context.GetComponents<TestSharedComponent1>(query2),
                new TestSharedComponent1 { Prop = 2 });
        }

        #endregion EntityQuery

        private void AssertHelper_EqualsComponents<T>(Entity[] entities, T[] components, T component)
            where T : unmanaged, IComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(components[i].Prop == component.Prop,
                    $"Enity.Id {entities[i].Id}, {typeof(T)}");
            }
        }
    }
}