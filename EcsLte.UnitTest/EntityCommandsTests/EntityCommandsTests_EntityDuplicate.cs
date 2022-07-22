using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_EntityDuplicate : BasePrePostTest
    {
        private EntityState _orgState = EntityState.Active;
        private EntityState _nonNullState = EntityState.Destroying;

        [TestMethod]
        public void DuplicateEntity()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entity = Context.Entities.CreateEntity(
                blueprint,
                _orgState);

            Assert_DuplicateEntity(commands, entity, false);
            Assert_DuplicateEntity(commands, entity, true);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntity(entity));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntity(entity, _nonNullState));
        }

        [TestMethod]
        public void DuplicateEntities()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entities = CreateTestEntities();

            Assert_DuplicateEntities(commands, entities, false);
            Assert_DuplicateEntities(commands, entities, true);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(entities));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(entities, 0));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(entities, 0, 1));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(entities, _nonNullState));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(entities, 0, _nonNullState));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(entities, 0, 1, _nonNullState));
        }

        [TestMethod]
        public void DuplicateEntities_ArcheType()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_ArcheType(commands, archeType, entities, false);
            Assert_DuplicateEntities_ArcheType(commands, archeType, entities, true);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(archeType));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(archeType, _nonNullState));
        }

        [TestMethod]
        public void DuplicateEntities_Filter()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Filter(commands, filter, entities, false);
            Assert_DuplicateEntities_Filter(commands, filter, entities, true);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(filter));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(filter, _nonNullState));
        }

        [TestMethod]
        public void DuplicateEntities_Tracker()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Tracker(commands, tracker, entities, false);
            Assert_DuplicateEntities_Tracker(commands, tracker, entities, true);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.DuplicateEntities(tracker));
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.DuplicateEntities(tracker, _nonNullState));

            tracker = Context.Tracking.CreateTracker("Tracker");
            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(tracker));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(tracker, _nonNullState));
        }

        [TestMethod]
        public void DuplicateEntities_Query()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(Context.Tracking.CreateTracker("Tracker1"),
                filter);
            queryFilterTracker.Tracker
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Query(commands, queryFilter, queryFilterTracker, entities, false);
            Assert_DuplicateEntities_Query(commands, queryFilter, queryFilterTracker, entities, true);

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.DuplicateEntities(queryFilterTracker.Tracker));
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.DuplicateEntities(queryFilterTracker.Tracker, _nonNullState));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(queryFilter));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.DuplicateEntities(queryFilter, _nonNullState));
        }

        private void Assert_DuplicateEntity(EntityCommands commands, Entity orgEntity, bool isNull)
        {
            var prevCount = Context.Entities.EntityCount();
            if (isNull)
                commands.DuplicateEntity(orgEntity);
            else
                commands.DuplicateEntity(orgEntity, _nonNullState);

            Assert.IsTrue(Context.Entities.EntityCount() == prevCount,
                $"Before Execute EntityCount: {Context.Entities.EntityCount()}");

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount() == prevCount + 1,
                $"After Execute EntityCount: {Context.Entities.EntityCount()}");

            var dupEntity = Context.Entities.GetEntities()[prevCount];

            Assert.IsTrue(Context.Entities.GetEntityState(dupEntity) == (isNull
                ? _orgState
                : _nonNullState));
        }

        private void Assert_DuplicateEntities(EntityCommands commands, Entity[] orgEntities, bool isNull)
        {
            AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => orgEntities,
                null,
                x =>
                {
                    if (isNull)
                        commands.DuplicateEntities(x);
                    else
                        commands.DuplicateEntities(x, _nonNullState);
                    return x;
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        commands.DuplicateEntities(x, startingIndex);
                    else
                        commands.DuplicateEntities(x, startingIndex, _nonNullState);
                    return x;
                },
                (x, startingIndex, count) =>
                {
                    if (isNull)
                        commands.DuplicateEntities(x, startingIndex, count);
                    else
                        commands.DuplicateEntities(x, startingIndex, count, _nonNullState);
                    return x;
                },
                (inSrc, startingIndex, count, outRef) =>
                {
                    var result = new TestResult();
                    var prevCount = Context.Entities.EntityCount();

                    commands.ExecuteCommands();

                    if (Context.Entities.EntityCount() != prevCount + count)
                    {
                        result.Success = false;
                        result.Error = $"Prev Count: {prevCount}, Count: {count}";
                    }
                    else
                    {
                        result = AssertEntities(inSrc, startingIndex, prevCount, Context.Entities.EntityCount() - prevCount,
                            isNull
                                ? _orgState
                                : _nonNullState);
                    }
                    return result;
                });
        }

        private void Assert_DuplicateEntities_ArcheType(EntityCommands commands, EntityArcheType archeType, Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = Context.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.DuplicateEntities(archeType);
            else
                commands.DuplicateEntities(archeType, _nonNullState);

            commands.ExecuteCommands();

            if (Context.Entities.EntityCount() != prevCount + count)
            {
                result.Success = false;
                result.Error = $"Prev Count: {prevCount}, Count: {count}";
            }
            else
            {
                result = AssertEntities(orgEntities, 0, prevCount, Context.Entities.EntityCount() - prevCount,
                    isNull
                        ? _orgState
                        : _nonNullState);
            }
            Assert.IsTrue(result.Success, result.Error);

            Context.Entities.DestroyEntities(Context.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray());
        }

        private void Assert_DuplicateEntities_Filter(EntityCommands commands, EntityFilter filter, Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = Context.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.DuplicateEntities(filter);
            else
                commands.DuplicateEntities(filter, _nonNullState);

            commands.ExecuteCommands();

            if (Context.Entities.EntityCount() != prevCount + count)
            {
                result.Success = false;
                result.Error = $"Prev Count: {prevCount}, Count: {count}";
            }
            else
            {
                result = AssertEntities(orgEntities, 0, prevCount, Context.Entities.EntityCount() - prevCount,
                    isNull
                        ? _orgState
                        : _nonNullState);
            }
            Assert.IsTrue(result.Success, result.Error);

            Context.Entities.DestroyEntities(Context.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray());
        }

        private void Assert_DuplicateEntities_Tracker(EntityCommands commands, EntityTracker tracker, Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = Context.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.DuplicateEntities(tracker);
            else
                commands.DuplicateEntities(tracker, _nonNullState);

            commands.ExecuteCommands();

            if (Context.Entities.EntityCount() != prevCount + count)
            {
                result.Success = false;
                result.Error = $"Prev Count: {prevCount}, Count: {count}";
            }
            else
            {
                result = AssertEntities(orgEntities, 0, prevCount, Context.Entities.EntityCount() - prevCount,
                    isNull
                        ? _orgState
                        : _nonNullState);
            }
            Assert.IsTrue(result.Success, result.Error);

            Context.Entities.DestroyEntities(Context.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray());
        }

        private void Assert_DuplicateEntities_Query(EntityCommands commands, EntityQuery queryFilter, EntityQuery queryFilterTracker,
            Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = Context.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.DuplicateEntities(queryFilter);
            else
                commands.DuplicateEntities(queryFilter, _nonNullState);

            commands.ExecuteCommands();

            if (Context.Entities.EntityCount() != prevCount + count)
            {
                result.Success = false;
                result.Error = $"Prev Count: {prevCount}, Count: {count}";
            }
            else
            {
                result = AssertEntities(orgEntities, 0, prevCount, Context.Entities.EntityCount() - prevCount,
                    isNull
                        ? _orgState
                        : _nonNullState);
            }
            Assert.IsTrue(result.Success, result.Error);

            Context.Entities.DestroyEntities(Context.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray());

            if (isNull)
                commands.DuplicateEntities(queryFilterTracker);
            else
                commands.DuplicateEntities(queryFilterTracker, _nonNullState);

            result = new TestResult();
            prevCount = Context.Entities.EntityCount();
            count = orgEntities.Length;

            commands.ExecuteCommands();

            if (Context.Entities.EntityCount() != prevCount + count)
            {
                result.Success = false;
                result.Error = $"Prev Count: {prevCount}, Count: {count}";
            }
            else
            {
                result = AssertEntities(orgEntities, 0, prevCount, Context.Entities.EntityCount() - prevCount,
                    isNull
                        ? _orgState
                        : _nonNullState);
            }
            Assert.IsTrue(result.Success, result.Error);

            Context.Entities.DestroyEntities(Context.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray());
        }

        private TestResult AssertEntities(Entity[] orgEntities, int startingIndex,
            int prevCount, int dupCount, EntityState state)
        {
            var result = new TestResult();
            var dupEntities = Context.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray();
            for (var i = startingIndex; i < dupCount; i++)
            {
                var orgEntity = orgEntities[i + startingIndex];
                var dupEntity = dupEntities[i];

                var dupState = Context.Entities.GetEntityState(dupEntity);

                if (Context.Entities.GetEntityState(dupEntity) != state)
                {
                    result.Success = false;
                    result.Error = $"EntityState OrgEntity: {orgEntity}, DupEntity: {dupEntity}";
                    break;
                }
            }
            return result;
        }

        private Entity[] CreateTestEntities()
        {
            var entities = new Entity[UnitTestConsts.SmallCount];
            var blueprint = new EntityBlueprint()
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            for (var i = 0; i < UnitTestConsts.SmallCount; i++)
            {
                blueprint.SetComponent(
                    new TestComponent1 { Prop = Context.Entities.EntityCount() + 1 });
                entities[i] = Context.Entities.CreateEntity(blueprint, EntityState.Active);
            }

            return entities;
        }
    }
}
