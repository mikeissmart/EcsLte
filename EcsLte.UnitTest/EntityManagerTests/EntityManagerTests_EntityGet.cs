using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityGet : BasePrePostTest
    {
        [TestMethod]
        public void GetEntities()
        {
            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntities(),
                x =>
                {
                    var count = Context.Entities.GetEntities(ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertEntities(entities, x, startingIndex, count);
                    return result;
                });

            EcsContexts.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(),
                x => Context.Entities.GetEntities(ref x),
                (x, startingIndex) => Context.Entities.GetEntities(ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_ArcheType()
        {
            var archeType = new EntityArcheType()
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntities(archeType),
                x =>
                {
                    var count = Context.Entities.GetEntities(archeType, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(archeType, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertEntities(entities, x, startingIndex, count);
                    return result;
                });

            var emptyEntities = new Entity[0];
            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref emptyEntities),
                    x => Context.Entities.GetEntities(x, ref emptyEntities, 0),
                });

            EcsContexts.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(archeType),
                x => Context.Entities.GetEntities(archeType, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(archeType, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_Filter()
        {
            var filter = new EntityFilter()
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntities(filter),
                x =>
                {
                    var count = Context.Entities.GetEntities(filter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(filter, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertEntities(entities, x, startingIndex, count);
                    return result;
                });

            var emptyEntities = new Entity[0];
            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref emptyEntities),
                    x => Context.Entities.GetEntities(x, ref emptyEntities, 0),
                });

            EcsContexts.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(filter),
                x => Context.Entities.GetEntities(filter, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(filter, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntities(tracker),
                x =>
                {
                    var count = Context.Entities.GetEntities(tracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(tracker, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertEntities(entities, x, startingIndex, count);
                    return result;
                });

            var diffContext = EcsContexts.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            var refEntities = new Entity[0];
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref refEntities),
                    x => Context.Entities.GetEntities(x, ref refEntities, 0),
                });

            EcsContexts.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(tracker),
                x => Context.Entities.GetEntities(tracker, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(tracker, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_Query()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(Context.Tracking.CreateTracker("Tracker1"),
                filter);
            queryFilterTracker.Tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            queryFilterTracker.Tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntities(queryFilter),
                x =>
                {
                    var count = Context.Entities.GetEntities(queryFilter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(queryFilter, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertEntities(entities, x, startingIndex, count);
                    return result;
                });
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntities(queryFilterTracker),
                x =>
                {
                    var count = Context.Entities.GetEntities(queryFilterTracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(queryFilterTracker, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertEntities(entities, x, startingIndex, count);
                    return result;
                });

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            var refEntities = new Entity[0];
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"), filter),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref refEntities),
                    x => Context.Entities.GetEntities(x, ref refEntities, 0),
                });

            EcsContexts.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(queryFilter),
                x => Context.Entities.GetEntities(queryFilter, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(queryFilter, ref x, startingIndex));
        }

        private TestResult AssertEntities(Entity[] orginalEntities,
            Entity[] getEntities, int startingIndex, int count)
        {
            var result = new TestResult();
            for (var i = 0; i < count; i++)
            {
                Assert.IsTrue(getEntities[i + startingIndex] == orginalEntities[i],
                    $"Entity: {orginalEntities[i]}, StartingIndex: {startingIndex}");
            }
            return result;
        }
    }
}
