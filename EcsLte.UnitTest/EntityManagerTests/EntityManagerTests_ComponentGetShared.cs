using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentGetShared : BasePrePostTest
    {
        [TestMethod]
        public void GetSharedComponent()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 1 }));

            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetSharedComponent<TestSharedComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetSharedComponent<TestSharedComponent1>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetSharedComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void GetSharedComponent_ArcheType()
        {
            var archeType = Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Context.Entities.CreateEntity(
                archeType);

            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(archeType).Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetSharedComponent<TestSharedComponent2>(archeType));

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.GetSharedComponent<TestSharedComponent1>(x),
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetSharedComponent<TestSharedComponent1>(archeType));
        }

        [TestMethod]
        public void GetSharedComponents_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestSharedComponent1>();

            var blueprint = new EntityBlueprint();
            for (var i = 0; i < 5; i++)
            {
                blueprint.SetSharedComponent(new TestSharedComponent1 { Prop = i + 1 });
                Context.Entities.CreateEntity(blueprint);
            }

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
                () => Context.Entities.GetSharedComponents<TestSharedComponent1>(filter),
                x =>
                {
                    var count = Context.Entities.GetSharedComponents(filter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetSharedComponents(filter, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertSharedComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestSharedComponent1[0];
            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.GetSharedComponents<TestSharedComponent1>(x),
                    x => Context.Entities.GetSharedComponents(x, ref emptyComponents),
                    x => Context.Entities.GetSharedComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestSharedComponent1>(
                () => Context.Entities.GetSharedComponents<TestSharedComponent1>(filter),
                x => Context.Entities.GetSharedComponents(filter, ref x),
                (x, startingIndex) => Context.Entities.GetSharedComponents(filter, ref x, startingIndex));
        }

        [TestMethod]
        public void GetComponent_Tracker()
        {
            var tracker = Context.Tracking
                .SetTrackingComponent<TestSharedComponent1>(true);

            var blueprint = new EntityBlueprint();
            for (var i = 0; i < 5; i++)
            {
                blueprint.SetSharedComponent(new TestSharedComponent1 { Prop = i + 1 });
                Context.Entities.CreateEntity(blueprint);
            }

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
                () => Context.Entities.GetSharedComponents<TestSharedComponent1>(tracker),
                x =>
                {
                    var count = Context.Entities.GetSharedComponents(tracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetSharedComponents(tracker, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertSharedComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestSharedComponent1[0];
            var diffContext = EcsContexts.Instance.CreateContext("DiffContext")
                .Tracking
                .SetTrackingMode(EntityTrackerMode.AnyChanges);
            AssertTracker_Different_Null(
                diffContext,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.GetSharedComponents<TestSharedComponent1>(x),
                    x => Context.Entities.GetSharedComponents(x, ref emptyComponents),
                    x => Context.Entities.GetSharedComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestSharedComponent1>(
                () => Context.Entities.GetSharedComponents<TestSharedComponent1>(tracker),
                x => Context.Entities.GetSharedComponents(tracker, ref x),
                (x, startingIndex) => Context.Entities.GetSharedComponents(tracker, ref x, startingIndex));
        }

        [TestMethod]
        public void GetComponent_Query()
        {
            var query = Context.Queries
                .SetFilter(Context.Filters
                    .WhereAllOf<TestComponent1>())
                .SetTracker(Context.Tracking
                    .SetTrackingComponent<TestComponent1>(true));

            var blueprint = new EntityBlueprint();
            for (var i = 0; i < 5; i++)
            {
                blueprint.SetSharedComponent(new TestSharedComponent1 { Prop = i + 1 });
                Context.Entities.CreateEntity(blueprint);
            }

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                false,
                () => Context.Entities.GlobalVersion,
                () => Context.Entities.GetSharedComponents<TestSharedComponent1>(query),
                x =>
                {
                    var count = Context.Entities.GetSharedComponents(query, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetSharedComponents(query, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertSharedComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestSharedComponent1[0];
            AssertQuery_DiffContext_Null(
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.GetSharedComponents<TestSharedComponent1>(x),
                    x => Context.Entities.GetSharedComponents(x, ref emptyComponents),
                    x => Context.Entities.GetSharedComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestSharedComponent1>(
                () => Context.Entities.GetSharedComponents<TestSharedComponent1>(query),
                x => Context.Entities.GetSharedComponents(query, ref x),
                (x, startingIndex) => Context.Entities.GetSharedComponents(query, ref x, startingIndex));
        }

        private TestResult AssertSharedComponents(TestSharedComponent1[] components, int startingIndex, int count)
        {
            var result = new TestResult();
            var propNum = startingIndex;
            for (var i = 0; i < count; i++, propNum++)
            {
                if (components[i + startingIndex].Prop != i + 1)
                {
                    result.Success = false;
                    result.Error = $"TestSharedComponent1.Prop: {components[i + startingIndex].Prop}, PropNum: {propNum}";
                    break;
                }
            }
            return result;
        }
    }
}
