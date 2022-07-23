using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_EntityStateSet : BasePrePostTest
    {
        [TestMethod]
        public void ChangeEntityState()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active);

            commands.ChangeEntityState(entity, EntityState.InActive);

            Assert.IsTrue(Context.Entities.GetEntityState(entity) == EntityState.Active);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetEntityState(entity) == EntityState.InActive);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DestroyEntity(entity));
        }

        [TestMethod]
        public void ChangeEntityStates()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Creating, UnitTestConsts.SmallCount);

            var state = EntityState.Creating;
            AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    Context.Entities.SetEntityStates(entities, EntityState.Creating);
                    return entities;
                },
                null,
                x =>
                {
                    state = EntityState.Active;
                    commands.ChangeEntityStates(x, state);
                    return new int[x.Length];
                },
                (x, startingIndex) =>
                {
                    state = EntityState.InActive;
                    commands.ChangeEntityStates(x, startingIndex, state);
                    return new int[x.Length - startingIndex];
                },
                (x, startingIndex, count) =>
                {
                    state = EntityState.Destroying;
                    commands.ChangeEntityStates(x, startingIndex, count, state);
                    return new int[count];
                },
                (src, startingIndex, count, destroyed) =>
                {
                    var result = AssertStates(src, EntityState.Creating, startingIndex, count);
                    if (!result.Success)
                    {
                        result.Error = $"Before Execute: {result.Error}";
                        return result;
                    }
                    commands.ExecuteCommands();
                    return AssertStates(src, state, startingIndex, count);
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(new Entity[1], state));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(new Entity[1], 0, state));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(new Entity[1], 0, 1, state));
        }

        [TestMethod]
        public void ChangeEntityStates_ArcheType()
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
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ChangeEntityStates(archeType, EntityState.Active);
            var result = AssertStates(Context.Entities.GetEntities(archeType),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Before Execute: {result.Error}");

            archeType.AddComponentType<TestComponent2>();
            Context.Entities.CreateEntities(
                archeType,
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            result = AssertStates(Context.Entities.GetEntities(archeType),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"ArcheType not snapshoted when creating command: {result.Error}");

            result = AssertStates(Context.Entities.GetEntities(orgArcheType),
                EntityState.Active, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"ArcheType not snapshoted when creating command: {result.Error}");

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => commands.ChangeEntityStates(x, EntityState.Active)
               });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(archeType, EntityState.Active));
        }

        [TestMethod]
        public void ChangeEntityStates_Filter()
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
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ChangeEntityStates(filter, EntityState.Active);
            var result = AssertStates(Context.Entities.GetEntities(filter),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Before Execute: {result.Error}");

            filter.FilterBy(new TestSharedComponent1 { Prop = 2 });
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            result = AssertStates(Context.Entities.GetEntities(filter),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Filter not snapshoted when creating command: {result.Error}");

            result = AssertStates(Context.Entities.GetEntities(orgFilter),
                EntityState.Active, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Filter not snapshoted when creating command: {result.Error}");

            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => commands.ChangeEntityStates(x, EntityState.Active)
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(filter, EntityState.Active));
        }

        [TestMethod]
        public void ChangeEntityStates_Tracker()
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
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ChangeEntityStates(tracker, EntityState.Active);
            var result = AssertStates(Context.Entities.GetEntities(tracker),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Before Execute: {result.Error}");

            tracker.ClearComponentState<TestComponent1>()
                .SetComponentState<TestComponent2>(EntityTrackerState.Added);
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent2>(),
                EntityState.Active,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            result = AssertStates(Context.Entities.GetEntities(orgTracker),
                EntityState.Active, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Tracker not snapshoted when creating command: {result.Error}");

            var diffContext = EcsContexts.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => commands.ChangeEntityStates(x, EntityState.Active)
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(tracker, EntityState.Active));
        }

        [TestMethod]
        public void ChangeEntityStates_Query()
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
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ChangeEntityStates(queryFilter, EntityState.Active);
            var result = AssertStates(Context.Entities.GetEntities(queryFilter),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Before Execute: {result.Error}");

            queryFilter.Filter.FilterBy(new TestSharedComponent1 { Prop = 2 });
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 }),
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();
            result = AssertStates(Context.Entities.GetEntities(orgQueryFilter),
                EntityState.Active, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"QueryFilter not snapshoted when creating command: {result.Error}");
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
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ChangeEntityStates(queryFilterTracker, EntityState.Active);
            result = AssertStates(Context.Entities.GetEntities(queryFilterTracker),
                EntityState.Creating, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"Before Execute: {result.Error}");

            commands.ExecuteCommands();
            result = AssertStates(Context.Entities.GetEntities(orgQueryFilterTracker),
                EntityState.Active, 0, UnitTestConsts.SmallCount);
            Assert.IsTrue(result.Success,
                $"QueryTracker not snapshoted when creating command: {result.Error}");
            Context.Entities.DestroyEntities(queryFilter);

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"),
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => commands.ChangeEntityStates(x, EntityState.Active)
                });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.ChangeEntityStates(queryFilter, EntityState.Active));
        }

        private TestResult AssertStates(Entity[] entities, EntityState state,
            int startingIndex, int count)
        {
            var result = new TestResult();
            for (var i = startingIndex; i < count; i++)
            {
                if (Context.Entities.GetEntityState(entities[i]) != state)
                {
                    result.Success = false;
                    result.Error = $"Entity: {entities[i]}, EntityState: {state}, StartingIndex: {startingIndex}";
                    break;
                }
            }
            return result;
        }
    }
}
