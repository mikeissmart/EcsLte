using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_TransferEntity : BasePrePostTest
    {
        private delegate void ArgumentsDelegate<T1, T2>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent;

        private delegate void ArgumentsDelegate<T1, T2, T3>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 component2,
            out T3 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent;

        [TestMethod]
        public void TransferEntities_Blittable() => Components_Blittable(AssertTransfer_Entities, Arguments);

        [TestMethod]
        public void TransferEntities_Blittable_DestroyAfterTransfer() => Components_Blittable(AssertTransfer_Entities, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_Blittable_Unique() => Components_Blittable_Unique(AssertTransfer_Entities, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_Blittable_Unique_DestroyAfterTransfer() => Components_Blittable_Unique(AssertTransfer_Entities, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_BlittableManage() => Components_BlittableManage(AssertTransfer_Entities, Arguments);

        [TestMethod]
        public void TransferEntities_BlittableManage_Unique() => Components_BlittableManage_Unique(AssertTransfer_Entities, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_BlittableMange_DestroyAfterTransfer() => Components_BlittableManage(AssertTransfer_Entities, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_BlittableMange_Unique_DestroyAfterTransfer() => Components_BlittableManage_Unique(AssertTransfer_Entities, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_Destroyed() => AssertTransfer_Destroy(
            (dest, source, entity) => dest.TransferEntities(source, new[] { entity }, false));

        [TestMethod]
        public void TransferEntities_Duplicate()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var context2 = EcsContexts.CreateContext("TransferTest");

            Assert.ThrowsException<EntityTransferAlreadyException>(() =>
                context2.TransferEntities(Context, new[] { entity, entity }, false));

            EcsContexts.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_DuplicateUniqueComponent() => AssertTransfer_DuplicateUnique(
            false,
            (dest, source, entity) => dest.TransferEntities(source, new[] { entity }, false));

        [TestMethod]
        public void TransferEntities_DuplicateUniqueComponent_Manage() => AssertTransfer_DuplicateUnique(
            true,
            (dest, source, entity) => dest.TransferEntities(source, new[] { entity }, false));

        [TestMethod]
        public void TransferEntities_EntityArcheType_Blittable() => Components_Blittable(AssertTransfer_EntityArcheType, Arguments);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Blittable_DestroyAfterTransfer() => Components_Blittable(AssertTransfer_EntityArcheType, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Blittable_Unique() => Components_Blittable_Unique(AssertTransfer_EntityArcheType, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Blittable_Unique_DestroyAfterTransfer() => Components_Blittable_Unique(AssertTransfer_EntityArcheType, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_EntityArcheType_BlittableManage() => Components_BlittableManage(AssertTransfer_EntityArcheType, Arguments);

        [TestMethod]
        public void TransferEntities_EntityArcheType_BlittableManage_Unique() => Components_BlittableManage_Unique(AssertTransfer_EntityArcheType, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_EntityArcheType_BlittableMange_DestroyAfterTransfer() => Components_BlittableManage(AssertTransfer_EntityArcheType, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_EntityArcheType_BlittableMange_Unique_DestroyAfterTransfer() => Components_BlittableManage_Unique(AssertTransfer_EntityArcheType, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Destroyed() => AssertTransfer_Destroy(
            (dest, source, entity) =>
            {
                var archeType = new EntityArcheType()
                    .AddComponentType<TestComponent1>();
                dest.TransferEntities(source, archeType, false);
            });

        [TestMethod]
        public void TransferEntities_EntityArcheType_DuplicateUniqueComponent() => AssertTransfer_DuplicateUnique(
            false,
            (dest, source, entity) =>
            {
                var archeType = new EntityArcheType()
                    .AddComponentType<TestUniqueComponent1>();
                dest.TransferEntities(source, archeType, false);
            });

        [TestMethod]
        public void TransferEntities_EntityArcheType_DuplicateUniqueComponent_Manage() => AssertTransfer_DuplicateUnique(
            true,
            (dest, source, entity) =>
            {
                var archeType = new EntityArcheType()
                    .AddComponentType<TestManageUniqueComponent1>();
                dest.TransferEntities(source, archeType, false);
            });

        [TestMethod]
        public void TransferEntities_EntityArcheType_Manage() => Components_Manage(AssertTransfer_EntityArcheType, Arguments);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Manage_DestroyAfterTransfer() => Components_Manage(AssertTransfer_EntityArcheType, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Manage_Unique() => Components_Manage_Unique(AssertTransfer_EntityArcheType, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Manage_Unique_DestroyAfterTransfer() => Components_Manage_Unique(AssertTransfer_EntityArcheType, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_EntityArcheType_Null() => AssertTransfer_Argument_Null(
            (dest, source, entity) =>
            {
                EntityArcheType archeType = null;
                dest.TransferEntities(source, archeType, false);
            });

        [TestMethod]
        public void TransferEntities_EntityArcheType_Source_Null() => AssertTransfer_Source_Null(
            (dest, source, entity) =>
            {
                var archeType = new EntityArcheType()
                    .AddComponentType<TestManageUniqueComponent1>();
                dest.TransferEntities(source, archeType, false);
            });

        [TestMethod]
        public void TransferEntities_EntityQuery_Blittable() => Components_Blittable(AssertTransfer_EntityQuery, Arguments);

        [TestMethod]
        public void TransferEntities_EntityQuery_Blittable_DestroyAfterTransfer() => Components_Blittable(AssertTransfer_EntityQuery, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_EntityQuery_Blittable_Unique() => Components_Blittable_Unique(AssertTransfer_EntityQuery, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_EntityQuery_Blittable_Unique_DestroyAfterTransfer() => Components_Blittable_Unique(AssertTransfer_EntityQuery, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_EntityQuery_BlittableManage() => Components_BlittableManage(AssertTransfer_EntityQuery, Arguments);

        [TestMethod]
        public void TransferEntities_EntityQuery_BlittableManage_Unique() => Components_BlittableManage_Unique(AssertTransfer_EntityQuery, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_EntityQuery_BlittableMange_DestroyAfterTransfer() => Components_BlittableManage(AssertTransfer_EntityQuery, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_EntityQuery_BlittableMange_Unique_DestroyAfterTransfer() => Components_BlittableManage_Unique(AssertTransfer_EntityQuery, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_EntityQuery_Destroyed() => AssertTransfer_Destroy(
            (dest, source, entity) =>
            {
                var query = new EntityQuery()
                    .WhereAllOf<TestComponent1>();
                dest.TransferEntities(source, query, false);
            });

        [TestMethod]
        public void TransferEntities_EntityQuery_DuplicateUniqueComponent() => AssertTransfer_DuplicateUnique(
            false,
            (dest, source, entity) =>
            {
                var query = new EntityQuery()
                    .WhereAllOf<TestUniqueComponent1>();
                dest.TransferEntities(source, query, false);
            });

        [TestMethod]
        public void TransferEntities_EntityQuery_DuplicateUniqueComponent_Manage() => AssertTransfer_DuplicateUnique(
            true,
            (dest, source, entity) =>
            {
                var query = new EntityQuery()
                    .WhereAllOf<TestManageUniqueComponent1>();
                dest.TransferEntities(source, query, false);
            });

        [TestMethod]
        public void TransferEntities_EntityQuery_Manage() => Components_Manage(AssertTransfer_EntityQuery, Arguments);

        [TestMethod]
        public void TransferEntities_EntityQuery_Manage_DestroyAfterTransfer() => Components_Manage(AssertTransfer_EntityQuery, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_EntityQuery_Manage_Unique() => Components_Manage_Unique(AssertTransfer_EntityQuery, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_EntityQuery_Manage_Unique_DestroyAfterTransfer() => Components_Manage_Unique(AssertTransfer_EntityQuery, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_EntityQuery_Null() => AssertTransfer_Argument_Null(
            (dest, source, entity) =>
            {
                EntityQuery query = null;
                dest.TransferEntities(source, query, false);
            });

        [TestMethod]
        public void TransferEntities_EntityQuery_Source_Null() => AssertTransfer_Source_Null(
            (dest, source, entity) =>
            {
                var query = new EntityQuery()
                    .WhereAllOf<TestManageUniqueComponent1>();
                dest.TransferEntities(source, query, false);
            });

        [TestMethod]
        public void TransferEntities_Manage() => Components_Manage(AssertTransfer_Entities, Arguments);

        [TestMethod]
        public void TransferEntities_Manage_DestroyAfterTransfer() => Components_Manage(AssertTransfer_Entities, Arguments_Destroy);

        [TestMethod]
        public void TransferEntities_Manage_Unique() => Components_Manage_Unique(AssertTransfer_Entities, Arguments_Unique);

        [TestMethod]
        public void TransferEntities_Manage_Unique_DestroyAfterTransfer() => Components_Manage_Unique(AssertTransfer_Entities, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntities_NoEntity_All() => AssertTransfer_NoEntity(
            (dest, source) => dest.TransferEntities(source, new[] { Entity.Null, Entity.Null }, false));

        [TestMethod]
        public void TransferEntities_NoEntity_Some() => AssertTransfer_NoEntity(
            (dest, source) =>
            {
                var entity = source.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1()));
                dest.TransferEntities(source, new[] { entity, Entity.Null }, false);
            });

        [TestMethod]
        public void TransferEntities_Null() => AssertTransfer_Argument_Null(
            (dest, source, entity) =>
            {
                Entity[] entities = null;
                dest.TransferEntities(source, entities, false);
            });

        [TestMethod]
        public void TransferEntities_Source_Null() => AssertTransfer_Source_Null(
            (dest, source, entity) => dest.TransferEntities(source, new[] { entity }, false));

        [TestMethod]
        public void TransferEntity_Blittable() => Components_Blittable(AssertTransfer_Entity, Arguments);

        [TestMethod]
        public void TransferEntity_Blittable_DestroyAfterTransfer() => Components_Blittable(AssertTransfer_Entity, Arguments_Destroy);

        [TestMethod]
        public void TransferEntity_Blittable_Unique() => Components_Blittable_Unique(AssertTransfer_Entity, Arguments_Unique);

        [TestMethod]
        public void TransferEntity_Blittable_Unique_DestroyAfterTransfer() => Components_Blittable_Unique(AssertTransfer_Entity, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntity_BlittableManage() => Components_BlittableManage(AssertTransfer_Entity, Arguments);

        [TestMethod]
        public void TransferEntity_BlittableManage_Unique() => Components_BlittableManage_Unique(AssertTransfer_Entity, Arguments_Unique);

        [TestMethod]
        public void TransferEntity_BlittableMange_DestroyAfterTransfer() => Components_BlittableManage(AssertTransfer_Entity, Arguments_Destroy);

        [TestMethod]
        public void TransferEntity_BlittableMange_Unique_DestroyAfterTransfer() => Components_BlittableManage_Unique(AssertTransfer_Entity, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntity_Destroyed() => AssertTransfer_Destroy(
            (dest, source, entity) => dest.TransferEntity(source, entity, false));

        [TestMethod]
        public void TransferEntity_DuplicateUniqueComponent() => AssertTransfer_DuplicateUnique(
            false,
            (dest, source, entity) => dest.TransferEntity(source, entity, false));

        [TestMethod]
        public void TransferEntity_DuplicateUniqueComponent_Manage() => AssertTransfer_DuplicateUnique(
            true,
            (dest, source, entity) => dest.TransferEntity(source, entity, false));

        [TestMethod]
        public void TransferEntity_Manage() => Components_Manage(AssertTransfer_Entity, Arguments);

        [TestMethod]
        public void TransferEntity_Manage_DestroyAfterTransfer() => Components_Manage(AssertTransfer_Entity, Arguments_Destroy);

        [TestMethod]
        public void TransferEntity_Manage_Unique() => Components_Manage_Unique(AssertTransfer_Entity, Arguments_Unique);

        [TestMethod]
        public void TransferEntity_Manage_Unique_DestroyAfterTransfer() => Components_Manage_Unique(AssertTransfer_Entity, Arguments_Destroy_Unique);

        [TestMethod]
        public void TransferEntity_NoEntity() => AssertTransfer_NoEntity(
            (dest, source) => dest.TransferEntity(source, Entity.Null, false));

        [TestMethod]
        public void TransferEntity_SameEcsContext() => AssertTransfer_SameEcsContext(
            (dest, source, entity) => dest.TransferEntity(source, entity, false));

        [TestMethod]
        public void TransferEntity_Source_Null() => AssertTransfer_Source_Null(
            (dest, source, entity) => dest.TransferEntity(source, entity, false));

        private void Arguments<T1, T2>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            entityCount = UnitTestConsts.SmallCount;
            keepEntityCount = UnitTestConsts.SmallCount;
            destroyEntities = false;
            component1 = new T1 { Prop = 1 };
            keepEntityComponent1 = new T2 { Prop = 2 };

            if (component1 is IUniqueComponent)
                throw new Exception();
        }

        private void Arguments<T1, T2, T3>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 component2,
            out T3 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            entityCount = UnitTestConsts.SmallCount;
            keepEntityCount = UnitTestConsts.SmallCount;
            destroyEntities = false;
            component1 = new T1 { Prop = 1 };
            component2 = new T2 { Prop = 2 };
            keepEntityComponent1 = new T3 { Prop = 3 };

            if (component1 is IUniqueComponent)
                throw new Exception();
        }

        private void Arguments_Destroy<T1, T2>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            entityCount = UnitTestConsts.SmallCount;
            keepEntityCount = 0;
            destroyEntities = true;
            component1 = new T1 { Prop = 1 };
            keepEntityComponent1 = new T2 { Prop = 2 };

            if (component1 is IUniqueComponent)
                throw new Exception();
        }

        private void Arguments_Destroy<T1, T2, T3>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 component2,
            out T3 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            entityCount = UnitTestConsts.SmallCount;
            keepEntityCount = 0;
            destroyEntities = true;
            component1 = new T1 { Prop = 1 };
            component2 = new T2 { Prop = 2 };
            keepEntityComponent1 = new T3 { Prop = 3 };

            if (component1 is IUniqueComponent)
                throw new Exception();
        }

        private void Arguments_Destroy_Unique<T1, T2>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            entityCount = 1;
            keepEntityCount = 0;
            destroyEntities = true;
            component1 = new T1 { Prop = 1 };
            keepEntityComponent1 = new T2 { Prop = 2 };

            if (!(component1 is IUniqueComponent))
                throw new Exception();
        }

        private void Arguments_Destroy_Unique<T1, T2, T3>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 component2,
            out T3 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            entityCount = 1;
            keepEntityCount = 0;
            destroyEntities = true;
            component1 = new T1 { Prop = 1 };
            component2 = new T2 { Prop = 2 };
            keepEntityComponent1 = new T3 { Prop = 3 };

            if (!(component1 is IUniqueComponent))
                throw new Exception();
        }

        private void Arguments_Unique<T1, T2>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            entityCount = 1;
            keepEntityCount = 1;
            destroyEntities = false;
            component1 = new T1 { Prop = 1 };
            keepEntityComponent1 = new T2 { Prop = 2 };

            if (!(component1 is IUniqueComponent))
                throw new Exception();
        }

        private void Arguments_Unique<T1, T2, T3>(out int entityCount, out int keepEntityCount, out bool destroyEntities,
            out T1 component1,
            out T2 component2,
            out T3 keepEntityComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            entityCount = 1;
            keepEntityCount = 1;
            destroyEntities = false;
            component1 = new T1 { Prop = 1 };
            component2 = new T2 { Prop = 2 };
            keepEntityComponent1 = new T3 { Prop = 3 };

            if (!(component1 is IUniqueComponent))
                throw new Exception();
        }

        private void AssertTransfer<T1, T2>(
            ArgumentsDelegate<T1, T2> del,
            Func<EcsContext, EcsContext, Entity[], Entity[]> transferAction)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            if (typeof(T1) == typeof(T2))
                throw new Exception();

            del.Invoke(out var entityCount, out var keepEntityCount, out var destroyEntities,
                out var component1,
                out var keepEntityComponent1);

            var context2 = EcsContexts.CreateContext("TransferTest");
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1));
            var keepEntity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(keepEntityComponent1));
            var transfered = transferAction.Invoke(context2, Context, entities);

            Assert.IsTrue(Context.EntityCount() == keepEntityCount + 1);
            Assert.IsTrue(Context.GetComponent<T2>(keepEntity).Prop == keepEntityComponent1.Prop);
            if (!destroyEntities)
            {
                for (var i = 0; i < entities.Length; i++)
                {
                    Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                        $"Enity.Id {entities[i].Id}");
                }
            }
            Assert.IsTrue(context2.EntityCount() == entityCount);
            for (var i = 0; i < transfered.Length; i++)
            {
                Assert.IsTrue(context2.GetComponent<T1>(transfered[i]).Prop == component1.Prop,
                    $"Enity.Id {transfered[i].Id}");
            }

            EcsContexts.DestroyContext(context2);
        }

        private void AssertTransfer<T1, T2, T3>(
            ArgumentsDelegate<T1, T2, T3> del,
            Func<EcsContext, EcsContext, Entity[], Entity[]> transferAction)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            if (typeof(T1) == typeof(T2))
                throw new Exception();
            if (typeof(T1) == typeof(T3))
                throw new Exception();
            if (typeof(T2) == typeof(T3))
                throw new Exception();

            del.Invoke(out var entityCount, out var keepEntityCount, out var destroyEntities,
                out var component1,
                out var component2,
                out var keepEntityComponent1);

            var context2 = EcsContexts.CreateContext("TransferTest");
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2));
            var keepEntity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(keepEntityComponent1));
            var transfered = transferAction.Invoke(context2, Context, entities);

            Assert.IsTrue(Context.EntityCount() == keepEntityCount + 1);
            Assert.IsTrue(Context.GetComponent<T3>(keepEntity).Prop == keepEntityComponent1.Prop);
            if (!destroyEntities)
            {
                for (var i = 0; i < entities.Length; i++)
                {
                    Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                        $"Enity.Id {entities[i].Id}");
                    Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                        $"Enity.Id {entities[i].Id}");
                }
            }
            Assert.IsTrue(context2.EntityCount() == entityCount);
            for (var i = 0; i < transfered.Length; i++)
            {
                Assert.IsTrue(context2.GetComponent<T1>(transfered[i]).Prop == component1.Prop,
                    $"Enity.Id {transfered[i].Id}");
                Assert.IsTrue(context2.GetComponent<T2>(transfered[i]).Prop == component2.Prop,
                    $"Enity.Id {transfered[i].Id}");
            }

            EcsContexts.DestroyContext(context2);
        }

        private void AssertTransfer_Argument_Null(Action<EcsContext, EcsContext, Entity> assertAction)
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var context2 = EcsContexts.CreateContext("TransferTest");

            Assert.ThrowsException<ArgumentNullException>(() =>
                assertAction.Invoke(Context, context2, entity));

            EcsContexts.DestroyContext(context2);
        }

        private void AssertTransfer_Destroy(Action<EcsContext, EcsContext, Entity> assertAction)
        {
            var component = new TestComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));
            var destroyedContext = EcsContexts.CreateContext("TransferTestDestroyed");
            EcsContexts.DestroyContext(destroyedContext);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                assertAction.Invoke(destroyedContext, Context, entity));
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                assertAction.Invoke(Context, destroyedContext, entity));
        }

        private void AssertTransfer_DuplicateUnique(
            bool isManage,
            Action<EcsContext, EcsContext, Entity> assertAction)
        {
            var blueprint = new EntityBlueprint();
            if (isManage)
                blueprint = blueprint.AddComponent(new TestManageUniqueComponent1());
            else
                blueprint = blueprint.AddComponent(new TestUniqueComponent1());
            var entity = Context.CreateEntity(blueprint);
            var context2 = EcsContexts.CreateContext("TransferTest");
            context2.CreateEntity(blueprint);

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                assertAction.Invoke(context2, Context, entity));

            EcsContexts.DestroyContext(context2);
        }

        private void AssertTransfer_Entities<T1, T2>(ArgumentsDelegate<T1, T2> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntities, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) => dest.TransferEntities(source, entities, destroyEntities));
        }

        private void AssertTransfer_Entities<T1, T2, T3>(ArgumentsDelegate<T1, T2, T3> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntities, out var _, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) => dest.TransferEntities(source, entities, destroyEntities));
        }

        private void AssertTransfer_Entity<T1, T2>(ArgumentsDelegate<T1, T2> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntity, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) =>
                {
                    var transfered = new Entity[entities.Length];
                    for (var i = 0; i < entities.Length; i++)
                        transfered[i] = dest.TransferEntity(source, entities[i], destroyEntity);
                    return transfered;
                });
        }

        private void AssertTransfer_Entity<T1, T2, T3>(ArgumentsDelegate<T1, T2, T3> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntity, out var _, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) =>
                {
                    var transfered = new Entity[entities.Length];
                    for (var i = 0; i < entities.Length; i++)
                        transfered[i] = dest.TransferEntity(source, entities[i], destroyEntity);
                    return transfered;
                });
        }

        private void AssertTransfer_EntityArcheType<T1, T2>(ArgumentsDelegate<T1, T2> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntities, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) =>
                {
                    var archeType = new EntityArcheType()
                        .AddComponentType<T1>();
                    return dest.TransferEntities(source, archeType, destroyEntities);
                });
        }

        private void AssertTransfer_EntityArcheType<T1, T2, T3>(ArgumentsDelegate<T1, T2, T3> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntities, out var _, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) =>
                {
                    var archeType = new EntityArcheType()
                        .AddComponentType<T1>()
                        .AddComponentType<T2>();
                    return dest.TransferEntities(source, archeType, destroyEntities);
                });
        }

        private void AssertTransfer_EntityQuery<T1, T2>(ArgumentsDelegate<T1, T2> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntities, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) =>
                {
                    var query = new EntityQuery()
                        .WhereAllOf<T1>();
                    return dest.TransferEntities(source, query, destroyEntities);
                });
        }

        private void AssertTransfer_EntityQuery<T1, T2, T3>(ArgumentsDelegate<T1, T2, T3> del)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            del.Invoke(out var _, out var _, out var destroyEntities, out var _, out var _, out var _);
            AssertTransfer(
                del,
                (dest, source, entities) =>
                {
                    var query = new EntityQuery()
                        .WhereAllOf<T1>()
                        .WhereAllOf<T2>();
                    return dest.TransferEntities(source, query, destroyEntities);
                });
        }

        private void AssertTransfer_NoEntity(Action<EcsContext, EcsContext> assertAction)
        {
            var context2 = EcsContexts.CreateContext("TransferTest");

            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                assertAction.Invoke(Context, context2));

            EcsContexts.DestroyContext(context2);
        }

        private void AssertTransfer_SameEcsContext(Action<EcsContext, EcsContext, Entity> assertAction)
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Assert.ThrowsException<EntityTransferSameEcsContextException>(() =>
                assertAction.Invoke(Context, Context, entity));
        }

        private void AssertTransfer_Source_Null(Action<EcsContext, EcsContext, Entity> assertAction)
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            EcsContext context2 = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                assertAction.Invoke(Context, context2, entity));
        }

        private void Components_Blittable(Action<ArgumentsDelegate<TestComponent1, TestComponent2>> assertAction,
            ArgumentsDelegate<TestComponent1, TestComponent2> arguments) =>
            assertAction.Invoke(arguments);

        private void Components_Blittable_Unique(Action<ArgumentsDelegate<TestUniqueComponent1, TestComponent1>> assertAction,
            ArgumentsDelegate<TestUniqueComponent1, TestComponent1> arguments) =>
           assertAction.Invoke(arguments);

        private void Components_BlittableManage(Action<ArgumentsDelegate<TestComponent1, TestManageComponent2, TestUniqueComponent1>> assertAction,
            ArgumentsDelegate<TestComponent1, TestManageComponent2, TestUniqueComponent1> arguments) =>
           assertAction.Invoke(arguments);

        private void Components_BlittableManage_Unique(Action<ArgumentsDelegate<TestUniqueComponent1, TestManageComponent1, TestUniqueComponent2>> assertAction,
            ArgumentsDelegate<TestUniqueComponent1, TestManageComponent1, TestUniqueComponent2> arguments) =>
           assertAction.Invoke(arguments);

        private void Components_Manage(Action<ArgumentsDelegate<TestManageComponent1, TestManageComponent2>> assertAction,
            ArgumentsDelegate<TestManageComponent1, TestManageComponent2> arguments) =>
           assertAction.Invoke(arguments);

        private void Components_Manage_Unique(Action<ArgumentsDelegate<TestManageUniqueComponent1, TestManageComponent1>> assertAction,
            ArgumentsDelegate<TestManageUniqueComponent1, TestManageComponent1> arguments) =>
           assertAction.Invoke(arguments);
    }
}