using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityCreate : BasePrePostTest
    {
        [TestMethod]
        public void CreateEntity()
        {
            var entity = Context.Entities.CreateEntity();
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
            Assert.IsTrue(Context.Entities.HasEntity(entity));
            Assert.IsTrue(Context.Entities.GetAllComponents(entity).Length == 0);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntity());
        }

        [TestMethod]
        public void CreateEntity_ArcheType()
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entity = Context.Entities.CreateEntity(archeType);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
            Assert.IsTrue(Context.Entities.HasEntity(entity));
            Assert.IsTrue(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsTrue(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            AssertArcheType_DiffContext_Null(
               new Action<EntityArcheType>[]
               {
                   x => Context.Entities.CreateEntity(x)
               });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntity(archeType));
        }

        [TestMethod]
        public void CreateEntity_ArcheType_Reuse()
        {
            var destroyEntity = Context.Entities.CreateEntity(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }));
            Context.Entities.DestroyEntity(destroyEntity);

            var entity = Context.Entities.CreateEntity(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }));

            Assert.IsTrue(entity.Version == 2);
            Assert.IsTrue(Context.Entities.HasEntity(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 4);
        }

        [TestMethod]
        public void CreateEntity_Blueprint()
        {
            var valid = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            AssertBlueprint_Valid_Invalid_Null(
                valid,
                x =>
                {
                    var entity = Context.Entities.CreateEntity(x);
                    return AssertEntity(entity,
                        Context.Entities.EntityCount(),
                        new TestComponent1 { Prop = 1 },
                        new TestSharedComponent1 { Prop = 2 });
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntity(valid));
        }

        [TestMethod]
        public void CreateEntity_Blueprint_Reuse()
        {
            var destroyEntity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 2 }));
            Context.Entities.DestroyEntity(destroyEntity);

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 2 }));

            Assert.IsTrue(entity.Version == 2);
            Assert.IsTrue(Context.Entities.HasEntity(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 4);
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = Context.Entities.CreateEntities(UnitTestConsts.SmallCount);
            Assert.IsTrue(entities.Length == UnitTestConsts.SmallCount);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];

                Assert.IsTrue(entity.Id == i + 1);
                Assert.IsTrue(entity.Version == 1);
                Assert.IsTrue(Context.Entities.HasEntity(entity));
                Assert.IsTrue(Context.Entities.GetAllComponents(entity).Length == 0);
            }

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntities(UnitTestConsts.SmallCount));
        }

        [TestMethod]
        public void CreateEntities_ArcheType()
        {
            var valid = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var firstEntityId = 0;
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                true,
                () => Context.Entities.GlobalVersion,
                () =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    return Context.Entities.CreateEntities(
                        valid,
                        UnitTestConsts.SmallCount);
                },
                x =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        ref x,
                        UnitTestConsts.SmallCount);
                    return (x, UnitTestConsts.SmallCount);
                },
                (x, startingIndex) =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        ref x,
                        startingIndex,
                        UnitTestConsts.SmallCount);
                    return (x, UnitTestConsts.SmallCount);
                },
                (xRef, startingIndex, count) =>
                {
                    return AssertEntities(xRef,
                        startingIndex,
                        firstEntityId,
                        new TestComponent1(),
                        new TestSharedComponent1 { Prop = 2 });
                });

            var entitiesRef = new Entity[0];
            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.CreateEntities(
                        x,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        ref entitiesRef,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        ref entitiesRef,
                        2,
                        UnitTestConsts.SmallCount),
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntities(
                    valid,
                    UnitTestConsts.SmallCount));
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.CreateEntities(
                    valid,
                    UnitTestConsts.SmallCount),
                x => Context.Entities.CreateEntities(
                    valid,
                    ref x,
                    UnitTestConsts.SmallCount),
                (x, startingIndex) => Context.Entities.CreateEntities(
                    valid,
                    ref x,
                    startingIndex,
                    UnitTestConsts.SmallCount));
        }

        [TestMethod]
        public void CreateEntities_Blueprint()
        {
            var valid = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var firstEntityId = 0;
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                true,
                () => Context.Entities.GlobalVersion,
                () =>
                {
                    return Context.Entities.CreateEntities(
                        valid,
                        UnitTestConsts.SmallCount);
                },
                x =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        ref x,
                        UnitTestConsts.SmallCount);
                    return (x, UnitTestConsts.SmallCount);
                },
                (x, startingIndex) =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        ref x,
                        startingIndex,
                        UnitTestConsts.SmallCount);
                    return (x, UnitTestConsts.SmallCount);
                },
                (xRef, startingIndex, count) =>
                {
                    return AssertEntities(xRef,
                        startingIndex,
                        firstEntityId,
                        new TestComponent1 { Prop = 1 },
                        new TestSharedComponent1 { Prop = 2 });
                });

            var entitiesRef = new Entity[0];
            AssertBlueprint_Invalid_Null(
                new Action<EntityBlueprint>[]
                {
                    x => Context.Entities.CreateEntities(
                        x,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        ref entitiesRef,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        ref entitiesRef,
                        2,
                        UnitTestConsts.SmallCount),
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntities(
                    valid,
                    UnitTestConsts.SmallCount));
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.CreateEntities(
                    valid,
                    UnitTestConsts.SmallCount),
                x => Context.Entities.CreateEntities(
                    valid,
                    ref x,
                    UnitTestConsts.SmallCount),
                (x, startingIndex) => Context.Entities.CreateEntities(
                    valid,
                    ref x,
                    startingIndex,
                    UnitTestConsts.SmallCount));
        }

        private TestResult AssertEntity(Entity entity,
            int entityId,
            TestComponent1 component,
            TestSharedComponent1 sharedComponent)
        {
            var result = new TestResult();

            if (entity.Id != entityId)
            {
                result.Success = false;
                result.Error = $"Entity.Id: {entity}";
            }
            else if (entity.Version != 1)
            {
                result.Success = false;
                result.Error = $"Entity.Version: {entity}";
            }
            else if (!Context.Entities.HasEntity(entity))
            {
                result.Success = false;
                result.Error = $"Entity Has: {entity}";
            }
            else if (Context.Entities.GetComponent<TestComponent1>(entity).Prop != component.Prop)
            {
                result.Success = false;
                result.Error = $"Entity Component: {entity}";
            }
            else if (Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop != sharedComponent.Prop)
            {
                result.Success = false;
                result.Error = $"Entity SharedComponent: {entity}";
            }
            return result;
        }

        private TestResult AssertEntities(Entity[] checkEntities, int checkStartingIndex,
            int firstEntityId,
            TestComponent1 component,
            TestSharedComponent1 sharedComponent)
        {
            var result = new TestResult();

            for (var i = checkStartingIndex; i < checkEntities.Length; i++)
            {
                var entity = checkEntities[i];
                result = AssertEntity(entity,
                    firstEntityId++,
                    component,
                    sharedComponent);

                if (!result.Success)
                    break;
                if (checkEntities.Where(x => x == entity).Count() > 1)
                {
                    result.Success = false;
                    result.Error = $"Duplicate: {entity}";
                }
            }

            return result;
        }

        private void AssertBlueprint_Valid_Invalid_Null(
            EntityBlueprint valid,
            Func<EntityBlueprint, TestResult> assertAction)
        {
            TestResult result;

            var invalid = new EntityBlueprint();
            EntityBlueprint nullable = null;

            result = assertAction(valid);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            Assert.ThrowsException<ArgumentNullException>(() => assertAction(nullable),
                "Null");
        }

        private void AssertBlueprint_Invalid_Null(
            params Action<EntityBlueprint>[] assertActions)
        {
            var invalid = new EntityBlueprint();
            EntityBlueprint nullable = null;

            foreach (var action in assertActions)
            {
                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");
            }
        }
    }
}
