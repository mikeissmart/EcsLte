using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_EntityCopy : BasePrePostTest
    {
        private EntityState _orgState = EntityState.Active;
        private EntityState _nonNullState = EntityState.Destroying;
        private EcsContext _destContext;

        [TestMethod]
        public void CopyEntity()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var commands = _destContext.Commands.CreateCommands("Commands");

            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entity = Context.Entities.CreateEntity(
                blueprint,
                _orgState);

            Assert_CopyEntity(commands, entity, false);
            Assert_CopyEntity(commands, entity, true);

            _destContext.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntity(Context.Entities, entity));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntity(Context.Entities, entity, _nonNullState));
        }

        [TestMethod]
        public void CopyEntities()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var commands = _destContext.Commands.CreateCommands("Commands");

            var entities = CreateTestEntities();

            Assert_CopyEntities(commands, entities, false);
            Assert_CopyEntities(commands, entities, true);

            _destContext.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, entities));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, entities, 0));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, entities, 0, 1));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, entities, _nonNullState));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, entities, 0, _nonNullState));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, entities, 0, 1, _nonNullState));
        }

        [TestMethod]
        public void CopyEntities_ArcheType()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var commands = _destContext.Commands.CreateCommands("Commands");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            Assert_CopyEntities_ArcheType(commands, archeType, entities, false);
            Assert_CopyEntities_ArcheType(commands, archeType, entities, true);

            _destContext.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, archeType));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, archeType, _nonNullState));
        }

        [TestMethod]
        public void CopyEntities_Filter()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var commands = _destContext.Commands.CreateCommands("Commands");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            Assert_CopyEntities_Filter(commands, filter, entities, false);
            Assert_CopyEntities_Filter(commands, filter, entities, true);

            _destContext.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, filter));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(Context.Entities, filter, _nonNullState));
        }

        [TestMethod]
        public void CopyEntities_Tracker()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var commands = _destContext.Commands.CreateCommands("Commands");

            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            Assert_CopyEntities_Tracker(commands, tracker, entities, false);
            Assert_CopyEntities_Tracker(commands, tracker, entities, true);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.CopyEntities(tracker));
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.CopyEntities(tracker, _nonNullState));

            tracker = _destContext.Tracking.CreateTracker("Tracker");
            _destContext.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(tracker));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(tracker, _nonNullState));
        }

        [TestMethod]
        public void CopyEntities_Query()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var commands = _destContext.Commands.CreateCommands("Commands");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(Context.Tracking.CreateTracker("Tracker1"),
                filter);
            queryFilterTracker.Tracker
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            Assert_CopyEntities_Query(commands, queryFilter, queryFilterTracker, entities, false);
            Assert_CopyEntities_Query(commands, queryFilter, queryFilterTracker, entities, true);

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.CopyEntities(queryFilterTracker.Tracker));
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                commands.CopyEntities(queryFilterTracker.Tracker, _nonNullState));

            _destContext.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(queryFilter));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CopyEntities(queryFilter, _nonNullState));
        }

        private void Assert_CopyEntity(EntityCommands commands, Entity orgEntity, bool isNull)
        {
            var prevCount = _destContext.Entities.EntityCount();
            if (isNull)
                commands.CopyEntity(Context.Entities, orgEntity);
            else
                commands.CopyEntity(Context.Entities, orgEntity, _nonNullState);

            Assert.IsTrue(_destContext.Entities.EntityCount() == prevCount,
                $"Before Execute EntityCount: {_destContext.Entities.EntityCount()}");

            commands.ExecuteCommands();
            Assert.IsTrue(_destContext.Entities.EntityCount() == prevCount + 1,
                $"After Execute EntityCount: {_destContext.Entities.EntityCount()}");

            var copyEntity = _destContext.Entities.GetEntities()[prevCount];

            Assert.IsTrue(_destContext.Entities.GetEntityState(copyEntity) == (isNull
                ? _orgState
                : _nonNullState));
        }

        private void Assert_CopyEntities(EntityCommands commands, Entity[] orgEntities, bool isNull)
        {
            AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => orgEntities,
                null,
                x =>
                {
                    if (isNull)
                        commands.CopyEntities(Context.Entities, x);
                    else
                        commands.CopyEntities(Context.Entities, x, _nonNullState);
                    return x;
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        commands.CopyEntities(Context.Entities, x, startingIndex);
                    else
                        commands.CopyEntities(Context.Entities, x, startingIndex, _nonNullState);
                    return x;
                },
                (x, startingIndex, count) =>
                {
                    if (isNull)
                        commands.CopyEntities(Context.Entities, x, startingIndex, count);
                    else
                        commands.CopyEntities(Context.Entities, x, startingIndex, count, _nonNullState);
                    return x;
                },
                (inSrc, startingIndex, count, outRef) =>
                {
                    var result = new TestResult();
                    var prevCount = _destContext.Entities.EntityCount();

                    commands.ExecuteCommands();

                    if (_destContext.Entities.EntityCount() != prevCount + count)
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

        private void Assert_CopyEntities_ArcheType(EntityCommands commands, EntityArcheType archeType, Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = _destContext.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.CopyEntities(Context.Entities, archeType);
            else
                commands.CopyEntities(Context.Entities, archeType, _nonNullState);

            commands.ExecuteCommands();

            if (_destContext.Entities.EntityCount() != prevCount + count)
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

            _destContext.Entities.DestroyEntities(_destContext.Entities.GetEntities());
        }

        private void Assert_CopyEntities_Filter(EntityCommands commands, EntityFilter filter, Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = _destContext.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.CopyEntities(Context.Entities, filter);
            else
                commands.CopyEntities(Context.Entities, filter, _nonNullState);

            commands.ExecuteCommands();

            if (_destContext.Entities.EntityCount() != prevCount + count)
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

            _destContext.Entities.DestroyEntities(_destContext.Entities.GetEntities());
        }

        private void Assert_CopyEntities_Tracker(EntityCommands commands, EntityTracker tracker, Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = _destContext.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.CopyEntities(tracker);
            else
                commands.CopyEntities(tracker, _nonNullState);

            commands.ExecuteCommands();

            if (_destContext.Entities.EntityCount() != prevCount + count)
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

            _destContext.Entities.DestroyEntities(_destContext.Entities.GetEntities());
        }

        private void Assert_CopyEntities_Query(EntityCommands commands, EntityQuery queryFilter, EntityQuery queryFilterTracker,
            Entity[] orgEntities, bool isNull)
        {
            var result = new TestResult();
            var prevCount = _destContext.Entities.EntityCount();
            var count = orgEntities.Length;

            if (isNull)
                commands.CopyEntities(queryFilter);
            else
                commands.CopyEntities(queryFilter, _nonNullState);

            commands.ExecuteCommands();

            if (_destContext.Entities.EntityCount() != prevCount + count)
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

            _destContext.Entities.DestroyEntities(_destContext.Entities.GetEntities());

            result = new TestResult();
            prevCount = _destContext.Entities.EntityCount();
            count = orgEntities.Length;

            if (isNull)
                commands.CopyEntities(queryFilterTracker);
            else
                commands.CopyEntities(queryFilterTracker, _nonNullState);

            commands.ExecuteCommands();

            if (_destContext.Entities.EntityCount() != prevCount + count)
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

            _destContext.Entities.DestroyEntities(_destContext.Entities.GetEntities());
        }

        private TestResult AssertEntities(Entity[] orgEntities, int startingIndex,
            int prevCount, int copyCount, EntityState state)
        {
            var result = new TestResult();
            var copyEntities = _destContext.Entities.GetEntities()
                .Skip(prevCount)
                .ToArray();
            for (var i = startingIndex; i < copyCount; i++)
            {
                var orgEntity = orgEntities[i + startingIndex];
                var copyEntity = copyEntities[i];

                var copyState = _destContext.Entities.GetEntityState(copyEntity);

                if (_destContext.Entities.GetEntityState(copyEntity) != state)
                {
                    result.Success = false;
                    result.Error = $"EntityState OrgEntity: {orgEntity}, CopyEntity: {copyEntity}";
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
