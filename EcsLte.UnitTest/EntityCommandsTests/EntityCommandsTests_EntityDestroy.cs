using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_EntityDestroy : BasePrePostTest
    {
        [TestMethod]
        public void DestroyEntity()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active);

            commands.DestroyEntity(entity);

            Assert.IsTrue(Context.Entities.HasEntity(entity));

            commands.ExecuteCommands();

            Assert.IsFalse(Context.Entities.HasEntity(entity));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntity(entity));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.CreateEntities(
                    archeType,
                    EntityState.Active,
                    UnitTestConsts.SmallCount),
                null,
                x =>
                {
                    commands.DestroyEntities(x);
                    return new int[x.Length];
                },
                (x, startingIndex) =>
                {
                    commands.DestroyEntities(x, startingIndex);
                    return new int[x.Length - startingIndex];
                },
                (x, startingIndex, count) =>
                {
                    commands.DestroyEntities(x, startingIndex, count);
                    return new int[count];
                },
                (src, startingIndex, count, destroyed) =>
                {
                    var result = new TestResult();

                    if (Context.Entities.EntityCount() != UnitTestConsts.SmallCount)
                    {
                        result.Success = false;
                        result.Error = $"Destroyed Before Execute: {Context.Entities.EntityCount()}";
                    }

                    commands.ExecuteCommands();

                    if (Context.Entities.EntityCount() != UnitTestConsts.SmallCount - count)
                    {
                        result.Success = false;
                        result.Error = $"Destroyed After Execute: {Context.Entities.EntityCount()}";
                    }
                    else
                    {
                        Context.Entities.DestroyEntities(Context.Entities.GetEntities());
                    }

                    return result;
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntities(new Entity[1]));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntities(new Entity[1], 0));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntities(new Entity[1], 0, 1));
        }

        [TestMethod]
        public void DestroyEntities_ArcheType()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });
            var orgArcheType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Context.Entities.CreateEntities(
                archeType,
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.DestroyEntities(archeType);
            Assert.IsTrue(Context.Entities.EntityCount(archeType) == UnitTestConsts.SmallCount);

            archeType.AddComponentType<TestComponent2>();
            Context.Entities.CreateEntities(
                archeType,
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(orgArcheType) == 0,
                "ArcheType not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(archeType) == UnitTestConsts.SmallCount);

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => commands.CreateEntity(x, EntityState.Active)
               });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntities(archeType));
        }

        [TestMethod]
        public void DestroyEntities_Filter()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });
            var orgFilter = new EntityFilter()
                .WhereAllOf<TestComponent1>()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.DestroyEntities(filter);
            Assert.IsTrue(Context.Entities.EntityCount(filter) == UnitTestConsts.SmallCount);

            filter.FilterBy(new TestSharedComponent1 { Prop = 2 });
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(orgFilter) == 0,
                "ArcheType not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(filter) == UnitTestConsts.SmallCount);

            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => commands.DestroyEntities(x)
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntities(filter));
        }

        [TestMethod]
        public void DestroyEntities_Tracker()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();
            var orgTracker = Context.Tracking.CreateTracker("OrgTracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.DestroyEntities(tracker);
            Assert.IsTrue(Context.Entities.EntityCount(tracker) == UnitTestConsts.SmallCount);

            tracker.ClearComponentState<TestComponent1>()
                .SetComponentState<TestComponent2>(EntityTrackerState.Added);
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent2>(),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(orgTracker) == 0,
                "Tracker not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(tracker) == UnitTestConsts.SmallCount);

            var diffContext = EcsContexts.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => commands.DestroyEntities(x)
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntities(tracker));
        }

        [TestMethod]
        public void DestroyEntities_Query()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var queryFilter = new EntityQuery(Context,
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()
                    .FilterBy(new TestSharedComponent1 { Prop = 1 }));
            var orgQueryFilter = new EntityQuery(Context,
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()
                    .FilterBy(new TestSharedComponent1 { Prop = 1 }));

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.DestroyEntities(queryFilter);
            Assert.IsTrue(Context.Entities.EntityCount(queryFilter) == UnitTestConsts.SmallCount);

            queryFilter.Filter.FilterBy(new TestSharedComponent1 { Prop = 2 });
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(orgQueryFilter) == 0,
                "QueryFilter not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(queryFilter) == UnitTestConsts.SmallCount);
            Context.Entities.DestroyEntities(queryFilter);

            var queryFilterTracker = new EntityQuery(
                Context.Tracking.CreateTracker("FilterTracker")
                    .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                    .StartTracking(),
                new EntityFilter()
                    .WhereAllOf<TestComponent1>());
            var orgQueryFilterTracker = new EntityQuery(
                Context.Tracking.CreateTracker("OrgFilterTracker")
                    .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                    .StartTracking(),
                new EntityFilter()
                    .WhereAllOf<TestComponent1>());

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.DestroyEntities(queryFilterTracker);
            Assert.IsTrue(Context.Entities.EntityCount(queryFilterTracker) == UnitTestConsts.SmallCount);

            queryFilterTracker.Tracker.ClearComponentState<TestComponent1>()
                .SetComponentState<TestComponent2>(EntityTrackerState.Added);
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent2>(),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(orgQueryFilterTracker) == 0,
                "QueryTracker not snapshoted when creating command");

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"),
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => commands.DestroyEntities(x)
                });
        }
    }
}
