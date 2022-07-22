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
        public void CreateEntity_ArcheType()
        {
            var uniqueEntity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 })
                    .AddUniqueComponentType<TestUniqueComponent1>(),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.HasUniqueComponent<TestUniqueComponent1>(uniqueEntity));
            Assert.IsTrue(Context.Entities.HasUniqueComponent<TestUniqueComponent1>());

            Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                Context.Entities.CreateEntity(
                    new EntityArcheType()
                        .AddUniqueComponentType<TestUniqueComponent1>(),
                    EntityState.Active));

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => Context.Entities.CreateEntity(x, EntityState.Active)
               });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntity(archeType, EntityState.Active));
        }

        [TestMethod]
        public void CreateEntity_ArcheType_Reuse()
        {
            var destroyEntity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Active);
            Context.Entities.DestroyEntity(destroyEntity);

            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Active);

            Assert.IsTrue(entity.Version == 2);
            Assert.IsTrue(Context.Entities.HasEntity(entity));
        }

        [TestMethod]
        public void CreateEntity_Blueprint()
        {
            var uniqueEntity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 2 })
                    .SetUniqueComponent(new TestUniqueComponent1 { Prop = 3 }),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(uniqueEntity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>().Prop == 3);

            Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                Context.Entities.CreateEntity(
                    new EntityBlueprint()
                        .SetUniqueComponent(new TestUniqueComponent1 { Prop = 3 }),
                    EntityState.Active));

            var valid = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            AssertBlueprint_Valid_Invalid_Null(
                valid,
                x =>
                {
                    var entity = Context.Entities.CreateEntity(x, EntityState.Active);
                    return AssertEntity(entity,
                        Context.Entities.EntityCount(),
                        new TestComponent1 { Prop = 1 },
                        new TestSharedComponent1 { Prop = 2 });
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntity(valid, EntityState.Active));
        }

        [TestMethod]
        public void CreateEntity_Blueprint_Reuse()
        {
            var destroyEntity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Active);
            Context.Entities.DestroyEntity(destroyEntity);

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Active);

            Assert.IsTrue(entity.Version == 2);
            Assert.IsTrue(Context.Entities.HasEntity(entity));
        }

        [TestMethod]
        public void CreateEntities_ArcheType()
        {
            var valid = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                Context.Entities.CreateEntities(
                    new EntityArcheType()
                        .AddUniqueComponentType<TestUniqueComponent1>(),
                    EntityState.Active,
                    2));

            int firstEntityId = 0;
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    return Context.Entities.CreateEntities(
                        valid,
                        EntityState.Active,
                        UnitTestConsts.SmallCount);
                },
                x =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        EntityState.Active,
                        ref x,
                        UnitTestConsts.SmallCount);
                    return (x, UnitTestConsts.SmallCount);
                },
                (x, startingIndex) =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        EntityState.Active,
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
            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.CreateEntities(
                        x,
                        EntityState.Active,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        EntityState.Active,
                        ref entitiesRef,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        EntityState.Active,
                        ref entitiesRef,
                        2,
                        UnitTestConsts.SmallCount),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
                    UnitTestConsts.SmallCount));
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
                    UnitTestConsts.SmallCount),
                x => Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
                    ref x,
                    UnitTestConsts.SmallCount),
                (x, startingIndex) => Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
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

            Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                Context.Entities.CreateEntities(
                    new EntityBlueprint()
                        .SetUniqueComponent(new TestUniqueComponent1 { Prop = 3 }),
                    EntityState.Active,
                    2));

            int firstEntityId = 0;
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return Context.Entities.CreateEntities(
                        valid,
                        EntityState.Active,
                        UnitTestConsts.SmallCount);
                },
                x =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        EntityState.Active,
                        ref x,
                        UnitTestConsts.SmallCount);
                    return (x, UnitTestConsts.SmallCount);
                },
                (x, startingIndex) =>
                {
                    firstEntityId = Context.Entities.EntityCount() + 1;

                    Context.Entities.CreateEntities(
                        valid,
                        EntityState.Active,
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
                        EntityState.Active,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        EntityState.Active,
                        ref entitiesRef,
                        UnitTestConsts.SmallCount),
                    x => Context.Entities.CreateEntities(
                        x,
                        EntityState.Active,
                        ref entitiesRef,
                        2,
                        UnitTestConsts.SmallCount),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
                    UnitTestConsts.SmallCount));
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
                    UnitTestConsts.SmallCount),
                x => Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
                    ref x,
                    UnitTestConsts.SmallCount),
                (x, startingIndex) => Context.Entities.CreateEntities(
                    valid,
                    EntityState.Active,
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

            Assert.ThrowsException<ComponentsNoneException>(() => assertAction(invalid),
                "Invalid");

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
                Assert.ThrowsException<ComponentsNoneException>(() => action(invalid),
                    "Invalid");

                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");
            }
        }
    }
}
