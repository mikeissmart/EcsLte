using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentGet : BasePrePostTest
    {
        [TestMethod]
        public void GetComponent()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 }));

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponent<TestComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponent<TestComponent1>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void GetComponents_T12()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _));
        }

        [TestMethod]
        public void GetComponents_T123()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2,
                out TestComponent3 component3);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);
            Assert.IsTrue(component3.Prop == 3);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent2 _,
                    out TestComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _,
                    out TestComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _));
        }

        [TestMethod]
        public void GetComponents_T1234()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2,
                out TestComponent3 component3,
                out TestSharedComponent1 component4);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);
            Assert.IsTrue(component3.Prop == 3);
            Assert.IsTrue(component4.Prop == 4);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent4 _,
                    out TestSharedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _));
        }

        [TestMethod]
        public void GetComponents_T12345()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2,
                out TestComponent3 component3,
                out TestSharedComponent1 component4,
                out TestSharedComponent2 component5);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);
            Assert.IsTrue(component3.Prop == 3);
            Assert.IsTrue(component4.Prop == 4);
            Assert.IsTrue(component5.Prop == 5);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent4 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestComponent4 _,
                    out TestSharedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _));
        }

        [TestMethod]
        public void GetComponents_T123456()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2,
                out TestComponent3 component3,
                out TestSharedComponent1 component4,
                out TestSharedComponent2 component5,
                out TestSharedComponent3 component6);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);
            Assert.IsTrue(component3.Prop == 3);
            Assert.IsTrue(component4.Prop == 4);
            Assert.IsTrue(component5.Prop == 5);
            Assert.IsTrue(component6.Prop == 6);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent4 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestComponent4 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestComponent4 _,
                    out TestSharedComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));
        }

        [TestMethod]
        public void GetComponents_T1234567()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2,
                out TestComponent3 component3,
                out TestSharedComponent1 component4,
                out TestSharedComponent2 component5,
                out TestSharedComponent3 component6,
                out TestManagedComponent1 component7);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);
            Assert.IsTrue(component3.Prop == 3);
            Assert.IsTrue(component4.Prop == 4);
            Assert.IsTrue(component5.Prop == 5);
            Assert.IsTrue(component6.Prop == 6);
            Assert.IsTrue(component7.Prop == 7);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent4 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestComponent4 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestComponent4 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestComponent4 _,
                    out TestManagedComponent1 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _));
        }

        [TestMethod]
        public void GetComponents_T12345678()
        {
            var entity = CreateTestEntity();

            Context.Entities.GetComponents(entity,
                out TestComponent1 component1,
                out TestComponent2 component2,
                out TestComponent3 component3,
                out TestSharedComponent1 component4,
                out TestSharedComponent2 component5,
                out TestSharedComponent3 component6,
                out TestManagedComponent1 component7,
                out TestManagedComponent2 component8);

            Assert.IsTrue(component1.Prop == 1);
            Assert.IsTrue(component2.Prop == 2);
            Assert.IsTrue(component3.Prop == 3);
            Assert.IsTrue(component4.Prop == 4);
            Assert.IsTrue(component5.Prop == 5);
            Assert.IsTrue(component6.Prop == 6);
            Assert.IsTrue(component7.Prop == 7);
            Assert.IsTrue(component8.Prop == 8);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _,
                    out TestComponent1 _));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent4 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent4 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent4 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestComponent4 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestComponent4 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestComponent4 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestComponent4 _,
                    out TestManagedComponent2 _));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestComponent4 _));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetComponents(Entity.Null,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetComponents(entity,
                    out TestComponent1 _,
                    out TestComponent2 _,
                    out TestComponent3 _,
                    out TestSharedComponent1 _,
                    out TestSharedComponent2 _,
                    out TestSharedComponent3 _,
                    out TestManagedComponent1 _,
                    out TestManagedComponent2 _));
        }

        [TestMethod]
        public void GetComponent_ArcheType()
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetComponents<TestComponent1>(archeType),
                x =>
                {
                    var count = Context.Entities.GetComponents(archeType, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetComponents(archeType, ref x, startingIndex);
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
                        result = AssertComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestComponent1[0];
            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.GetComponents<TestComponent1>(x),
                    x => Context.Entities.GetComponents(x, ref emptyComponents),
                    x => Context.Entities.GetComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestComponent1>(
                () => Context.Entities.GetComponents<TestComponent1>(archeType),
                x => Context.Entities.GetComponents(archeType, ref x),
                (x, startingIndex) => Context.Entities.GetComponents(archeType, ref x, startingIndex));
        }

        [TestMethod]
        public void GetComponent_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetComponents<TestComponent1>(filter),
                x =>
                {
                    var count = Context.Entities.GetComponents(filter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetComponents(filter, ref x, startingIndex);
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
                        result = AssertComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestComponent1[0];
            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.GetComponents<TestComponent1>(x),
                    x => Context.Entities.GetComponents(x, ref emptyComponents),
                    x => Context.Entities.GetComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestComponent1>(
                () => Context.Entities.GetComponents<TestComponent1>(filter),
                x => Context.Entities.GetComponents(filter, ref x),
                (x, startingIndex) => Context.Entities.GetComponents(filter, ref x, startingIndex));
        }

        [TestMethod]
        public void GetComponent_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetComponents<TestComponent1>(tracker),
                x =>
                {
                    var count = Context.Entities.GetComponents(tracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetComponents(tracker, ref x, startingIndex);
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
                        result = AssertComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestComponent1[0];
            var diffContext = EcsContexts.Instance.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.GetComponents<TestComponent1>(x),
                    x => Context.Entities.GetComponents(x, ref emptyComponents),
                    x => Context.Entities.GetComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestComponent1>(
                () => Context.Entities.GetComponents<TestComponent1>(tracker),
                x => Context.Entities.GetComponents(tracker, ref x),
                (x, startingIndex) => Context.Entities.GetComponents(tracker, ref x, startingIndex));
        }

        [TestMethod]
        public void GetComponent_Query()
        {
            var query = Context.Queries
                .SetFilter(Context.Filters
                    .WhereAllOf<TestComponent1>())
                .SetTracker(Context.Tracking.CreateTracker("Tracker1")
                    .SetTrackingState<TestComponent1>(TrackingState.Added)
                    .StartTracking());

            var entities = CreateTestEntities();

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetComponents<TestComponent1>(query),
                x =>
                {
                    var count = Context.Entities.GetComponents(query, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetComponents(query, ref x, startingIndex);
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
                        result = AssertComponents(x, startingIndex, count);
                    }
                    return result;
                });

            var emptyComponents = new TestComponent1[0];
            Context.Tracking.RemoveTracker(query.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                query,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.GetComponents<TestComponent1>(x),
                    x => Context.Entities.GetComponents(x, ref emptyComponents),
                    x => Context.Entities.GetComponents(x, ref emptyComponents, 0),
                });

            EcsContexts.Instance.DestroyContext(Context);
            AssertGetRef_ContextDestroyed<TestComponent1>(
                () => Context.Entities.GetComponents<TestComponent1>(query),
                x => Context.Entities.GetComponents(query, ref x),
                (x, startingIndex) => Context.Entities.GetComponents(query, ref x, startingIndex));
        }

        private TestResult AssertComponents(TestComponent1[] components, int startingIndex, int count)
        {
            var result = new TestResult();
            var propNum = startingIndex;
            for (var i = 0; i < count; i++, propNum++)
            {
                if (components[i + startingIndex].Prop != i + 1)
                {
                    result.Success = false;
                    result.Error = $"TestComponent1.Prop: {components[i + startingIndex].Prop}, PropNum: {propNum}";
                    break;
                }
            }
            return result;
        }

        private Entity CreateTestEntity() => Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetComponent(new TestComponent2 { Prop = 2 })
                    .SetComponent(new TestComponent3 { Prop = 3 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 4 })
                    .SetSharedComponent(new TestSharedComponent2 { Prop = 5 })
                    .SetSharedComponent(new TestSharedComponent3 { Prop = 6 })
                    .SetManagedComponent(new TestManagedComponent1 { Prop = 7 })
                    .SetManagedComponent(new TestManagedComponent2 { Prop = 8 }));

        private Entity[] CreateTestEntities()
        {
            var entities = new Entity[UnitTestConsts.SmallCount];
            var blueprint = new EntityBlueprint()
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            for (var i = 0; i < UnitTestConsts.SmallCount; i++)
            {
                blueprint.SetComponent(
                    new TestComponent1 { Prop = Context.Entities.EntityCount() + 1 });
                entities[i] = Context.Entities.CreateEntity(blueprint);
            }

            return entities;
        }
    }
}
