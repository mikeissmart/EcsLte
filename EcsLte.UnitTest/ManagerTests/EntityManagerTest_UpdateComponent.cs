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
        public void UpdateComponent_Blittable_Normal() =>
            Components_Blittable_Normal(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Blittable_Shared() =>
            Components_Blittable_Shared(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Blittable_Unique() =>
            Components_Blittable_Unique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponent(Entity.Null, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponent_Manage_Normal() =>
            Components_Manage_Normal(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Manage_Shared() =>
            Components_Manage_Shared(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_Manage_Unique() =>
            Components_Manage_Unique(AssertUpdateComponent_Entity);

        [TestMethod]
        public void UpdateComponent_NoEntity() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.UpdateComponent(Entity.Null, new TestComponent1()));

        private void AssertUpdateComponent_Entity<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                });

        private void AssertUpdateComponent_Entity<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T2 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                });

        private void AssertUpdateComponent_Entity<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T2 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T3 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                });

        private void AssertUpdateComponent_Entity<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T2 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T3 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T4 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                });

        private void AssertUpdateComponent_Entity<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T2 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T3 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T4 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T5 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                },
                (Entity[] entities, T6 component) =>
                {
                    for (var i = 0; i < entityCount; i++)
                        Context.UpdateComponent(entities[i], component);
                });

        #endregion Entity

        #region Entities

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_Normal() =>
            Components_Blittable_Normal(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_Shared() =>
            Components_Blittable_Shared(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Blittable_Unique() =>
            Components_Blittable_Unique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponents(new[] { Entity.Null }, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponent_Entities_Manage_Normal() =>
            Components_Manage_Normal(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Manage_Shared() =>
            Components_Manage_Shared(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_Manage_Unique() =>
            Components_Manage_Unique(AssertUpdateComponent_Entities);

        [TestMethod]
        public void UpdateComponent_Entities_NoEntity_All() =>
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.UpdateComponents(new[] { Entity.Null }, new TestComponent1()));

        [TestMethod]
        public void UpdateComponent_Entities_NoEntity_Some()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.UpdateComponents(new[] { entity, Entity.Null }, new TestComponent1()));
        }

        private void AssertUpdateComponent_Entities<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(entities, component));

        private void AssertUpdateComponent_Entities<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(entities, component));

        private void AssertUpdateComponent_Entities<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(entities, component));

        private void AssertUpdateComponent_Entities<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T4 component) => Context.UpdateComponents(entities, component));

        private void AssertUpdateComponent_Entities<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T4 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T5 component) => Context.UpdateComponents(entities, component),
                (Entity[] entities, T6 component) => Context.UpdateComponents(entities, component));

        #endregion Entities

        #region EntityArcheType

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_Normal() =>
            Components_Blittable_Normal(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_Shared() =>
            Components_Blittable_Shared(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Blittable_Unique() =>
            Components_Blittable_Unique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            var archeType = new EntityArcheType();
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponents(archeType, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_Normal() =>
            Components_Manage_Normal(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_Shared() =>
            Components_Manage_Shared(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Manage_Unique() =>
            Components_Manage_Unique(AssertUpdateComponent_EntityArcheType);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Null()
        {
            EcsContexts.DestroyContext(Context);

            EntityArcheType archeType = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.UpdateComponents(archeType, new TestComponent1()));
        }

        private void AssertUpdateComponent_EntityArcheType<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1);
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component));
        }

        private void AssertUpdateComponent_EntityArcheType<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var archeType = new EntityArcheType()
                .AddComponentTypeOrSharedComponent(c1)
                .AddComponentTypeOrSharedComponent(c2);
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component));
        }

        private void AssertUpdateComponent_EntityArcheType<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component));

        private void AssertUpdateComponent_EntityArcheType<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new() => AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T4 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component));

        private void AssertUpdateComponent_EntityArcheType<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
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
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T4 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T5 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component),
                (Entity[] entities, T6 component) => Context.UpdateComponents(Context.GetEntityArcheType(entities[0]), component));
        }

        #endregion EntityArcheType

        #region EntityQuery

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_Normal() =>
            Components_Blittable_Normal(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_Normal_PrePopulated_Large()
        {
            var sharedComponent1 = new TestSharedComponent1 { Prop = 1 };
            var sharedComponent2 = new TestSharedComponent1 { Prop = 2 };
            var archeType1 = new EntityArcheType()
                .AddSharedComponent(sharedComponent1)
                .AddComponentType<TestComponent1>();
            var archeType2 = new EntityArcheType()
                .AddSharedComponent(sharedComponent2)
                .AddComponentType<TestComponent1>();
            var blueprint1 = new EntityBlueprint()
                .AddComponent(sharedComponent1);
            var blueprint2 = new EntityBlueprint()
                .AddComponent(sharedComponent2);
            for (var i = 0; i < UnitTestConsts.LargeCount / 2; i++)
            {
                blueprint1 = blueprint1.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                Context.CreateEntity(blueprint1);
            }
            for (var i = 0; i < UnitTestConsts.LargeCount / 2; i++)
            {
                blueprint2 = blueprint2.UpdateComponent(new TestComponent1 { Prop = i + 1 + UnitTestConsts.LargeCount / 2 });
                Context.CreateEntity(blueprint2);
            }
            Context.UpdateComponents(archeType1, sharedComponent2);

            var components = Context.GetComponents<TestSharedComponent1>(archeType2);
            var entities = Context.GetEntities(archeType2);
            Assert.IsTrue(components.Length == UnitTestConsts.LargeCount);
            Assert.IsTrue(entities.Length == UnitTestConsts.LargeCount);
            for (var i = 0; i < entities.Length; i++)
            {
                var component = Context.GetComponent<TestComponent1>(entities[i]);
                Assert.IsTrue(component.Prop == entities[i].Id,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_Normal_PrePopulated_Small()
        {
            var sharedComponent1 = new TestSharedComponent1 { Prop = 1 };
            var sharedComponent2 = new TestSharedComponent1 { Prop = 2 };
            var archeType1 = new EntityArcheType()
                .AddSharedComponent(sharedComponent1)
                .AddComponentType<TestComponent1>();
            var archeType2 = new EntityArcheType()
                .AddSharedComponent(sharedComponent2)
                .AddComponentType<TestComponent1>();
            var blueprint1 = new EntityBlueprint()
                .AddComponent(sharedComponent1);
            var blueprint2 = new EntityBlueprint()
                .AddComponent(sharedComponent2);
            for (var i = 0; i < UnitTestConsts.SmallCount / 2; i++)
            {
                blueprint1 = blueprint1.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                Context.CreateEntity(blueprint1);
            }
            for (var i = 0; i < UnitTestConsts.SmallCount / 2; i++)
            {
                blueprint2 = blueprint2.UpdateComponent(new TestComponent1 { Prop = i + 1 + UnitTestConsts.SmallCount / 2 });
                var entity = Context.CreateEntity(blueprint2);
                var comp = Context.GetComponent<TestComponent1>(entity);
                ;
            }
            Context.UpdateComponents(archeType1, sharedComponent2);

            var components = Context.GetComponents<TestSharedComponent1>(archeType2);
            var entities = Context.GetEntities(archeType2);
            Assert.IsTrue(components.Length == UnitTestConsts.SmallCount);
            Assert.IsTrue(entities.Length == UnitTestConsts.SmallCount);
            for (var i = 0; i < entities.Length; i++)
            {
                var component = Context.GetComponent<TestComponent1>(entities[i]);
                Assert.IsTrue(component.Prop == entities[i].Id,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_NormalShared() =>
            Components_Blittable_NormalShared(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_NormalUnique() =>
            Components_Blittable_NormalUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_NormalSharedUnique() =>
            Components_Blittable_NormalSharedUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_Shared() =>
            Components_Blittable_Shared(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_SharedUnique() =>
            Components_Blittable_SharedUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Blittable_Unique() =>
            Components_Blittable_Unique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_Normal() =>
            Components_BlittableManage_Normal(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_NormalShared() =>
            Components_BlittableManage_NormalShared(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_NormalUnique() =>
            Components_BlittableManage_NormalUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_NormalSharedUnique() =>
            Components_BlittableManage_NormalSharedUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_Shared() =>
            Components_BlittableManage_Shared(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_SharedUnique() =>
            Components_BlittableManage_SharedUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_BlittableManage_Unique() =>
            Components_BlittableManage_Unique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            var query = new EntityQuery();
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.UpdateComponents(query, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_Normal() =>
            Components_Manage_Normal(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_NormalShared() =>
            Components_Manage_NormalShared(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_NormalSharedUnique() =>
            Components_Manage_NormalSharedUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_NormalUnique() =>
            Components_Manage_NormalUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_Shared() =>
            Components_Manage_Shared(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_SharedUnique() =>
            Components_Manage_SharedUnique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Manage_Unique() =>
            Components_Manage_Unique(AssertUpdateComponent_EntityQuery);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Null()
        {
            EcsContexts.DestroyContext(Context);

            EntityQuery query = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.UpdateComponents(query, new TestComponent1()));
        }

        private void AssertUpdateComponent_EntityQuery<T1>(int entityCount, T1 c1)
            where T1 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>();
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(query, component));
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2>(int entityCount, T1 c1, T2 c2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>()
                .WhereAllOf<T2>();
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(query, component));
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2, T3>(int entityCount, T1 c1, T2 c2, T3 c3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var query = new EntityQuery()
                .WhereAllOf<T1>()
                .WhereAllOf<T2>()
                .WhereAllOf<T3>();
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(query, component));
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2, T3, T4>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4)
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
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T4 component) => Context.UpdateComponents(query, component));
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2, T3, T4, T5, T6>(int entityCount, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
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
            AssertUpdateComponents(
                entityCount,
                (Entity[] entities, T1 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T2 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T3 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T4 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T5 component) => Context.UpdateComponents(query, component),
                (Entity[] entities, T6 component) => Context.UpdateComponents(query, component));
        }

        #endregion EntityQuery

        #region Assert

        private void AssertUpdateComponents<T1>(int entityCount,
            Action<Entity[], T1> updateAction1)
            where T1 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1()));
            AssertHelper_EqualsComponent(entities, 1, updateAction1);
        }

        private void AssertUpdateComponents<T1, T2>(int entityCount,
            Action<Entity[], T1> updateAction1,
            Action<Entity[], T2> updateAction2)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2()));
            AssertHelper_EqualsComponent(entities, 1, updateAction1);
            AssertHelper_EqualsComponent(entities, 2, updateAction2);
        }

        private void AssertUpdateComponents<T1, T2, T3>(int entityCount,
            Action<Entity[], T1> updateAction1,
            Action<Entity[], T2> updateAction2,
            Action<Entity[], T3> updateAction3)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3());
            var entities = Context.CreateEntities(entityCount, blueprint);

            AssertHelper_EqualsComponent(entities, 1, updateAction1);
            AssertHelper_EqualsComponent(entities, 2, updateAction2);
            AssertHelper_EqualsComponent(entities, 3, updateAction3);
        }

        private void AssertUpdateComponents<T1, T2, T3, T4>(int entityCount,
            Action<Entity[], T1> updateAction1,
            Action<Entity[], T2> updateAction2,
            Action<Entity[], T3> updateAction3,
            Action<Entity[], T4> updateAction4)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4());
            var entities = Context.CreateEntities(entityCount, blueprint);

            AssertHelper_EqualsComponent(entities, 1, updateAction1);
            AssertHelper_EqualsComponent(entities, 2, updateAction2);
            AssertHelper_EqualsComponent(entities, 3, updateAction3);
            AssertHelper_EqualsComponent(entities, 4, updateAction4);
        }

        private void AssertUpdateComponents<T1, T2, T3, T4, T5, T6>(int entityCount,
            Action<Entity[], T1> updateAction1,
            Action<Entity[], T2> updateAction2,
            Action<Entity[], T3> updateAction3,
            Action<Entity[], T4> updateAction4,
            Action<Entity[], T5> updateAction5,
            Action<Entity[], T6> updateAction6)
            where T1 : IComponent, ITestComponent, new()
            where T2 : IComponent, ITestComponent, new()
            where T3 : IComponent, ITestComponent, new()
            where T4 : IComponent, ITestComponent, new()
            where T5 : IComponent, ITestComponent, new()
            where T6 : IComponent, ITestComponent, new()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new T1())
                .AddComponent(new T2())
                .AddComponent(new T3())
                .AddComponent(new T4())
                .AddComponent(new T5())
                .AddComponent(new T6());
            var entities = Context.CreateEntities(entityCount, blueprint);

            AssertHelper_EqualsComponent(entities, 1, updateAction1);
            AssertHelper_EqualsComponent(entities, 2, updateAction2);
            AssertHelper_EqualsComponent(entities, 3, updateAction3);
            AssertHelper_EqualsComponent(entities, 4, updateAction4);
            AssertHelper_EqualsComponent(entities, 5, updateAction5);
            AssertHelper_EqualsComponent(entities, 6, updateAction6);
        }

        private void AssertHelper_EqualsComponent<T>(Entity[] entities, int addNum, Action<Entity[], T> updateAction)
            where T : IComponent, ITestComponent, new()
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T>(entities[i]).Prop == 0,
                    $"Enity.Id {entities[i].Id}");
            }
            updateAction.Invoke(entities, new T { Prop = addNum });
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T>(entities[i]).Prop == addNum,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Assert

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

        #endregion Components
    }
}