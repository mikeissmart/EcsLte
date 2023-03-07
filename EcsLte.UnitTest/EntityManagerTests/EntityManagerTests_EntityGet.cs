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
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
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

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(),
                x => Context.Entities.GetEntities(ref x),
                (x, startingIndex) => Context.Entities.GetEntities(ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_ArcheType()
        {
            var archeType = Context.ArcheTypes
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
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
            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref emptyEntities),
                    x => Context.Entities.GetEntities(x, ref emptyEntities, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(archeType),
                x => Context.Entities.GetEntities(archeType, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(archeType, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
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

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(filter),
                x => Context.Entities.GetEntities(filter, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(filter, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_Tracker()
        {
            var tracker = Context.Tracking
                .SetTrackingComponent<TestComponent1>(true);

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
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

            var diffContext = EcsContexts.Instance.CreateContext("DiffContext")
                .Tracking
                .SetTrackingMode(EntityTrackerMode.AnyChanges);
            var refEntities = new Entity[0];
            AssertTracker_Different_Null(
                diffContext,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref refEntities),
                    x => Context.Entities.GetEntities(x, ref refEntities, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(tracker),
                x => Context.Entities.GetEntities(tracker, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(tracker, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntities_Query()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var query = Context.Queries
                .SetFilter(filter)
                .SetTracker(Context.Tracking
                    .SetTrackingComponent<TestComponent1>(true));

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
                () => Context.Entities.GetEntities(query),
                x =>
                {
                    var count = Context.Entities.GetEntities(query, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntities(query, ref x, startingIndex);
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

            var refEntities = new Entity[0];
            AssertQuery_DiffContext_Null(
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.GetEntities(x),
                    x => Context.Entities.GetEntities(x, ref refEntities),
                    x => Context.Entities.GetEntities(x, ref refEntities, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<Entity>(
                () => Context.Entities.GetEntities(query),
                x => Context.Entities.GetEntities(query, ref x),
                (x, startingIndex) => Context.Entities.GetEntities(query, ref x, startingIndex));
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
