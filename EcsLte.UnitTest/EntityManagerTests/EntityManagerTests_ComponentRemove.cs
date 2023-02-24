using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentRemove : BasePrePostTest
    {
        [TestMethod]
        public void RemoveComponent()
        {
            var removeTracker = Context.Tracking.CreateTracker("RemoveTracker")
                .SetTrackingState<TestComponent1>(TrackingState.Removed)
                .StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 }));

            Context.Entities.RemoveComponent<TestComponent1>(entity);
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));

            Assert.IsTrue(Context.Entities.GetEntities(removeTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(removeTracker)[0] == entity);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponent<TestComponent2>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponent<TestComponent1>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponent<TestComponent1>(entity));
        }
        [TestMethod]
        public void RemoveComponent_Config()
        {
            var removeTracker = Context.Tracking.CreateTracker("RemoveTracker")
                .SetTrackingState<TestComponent1>(TrackingState.Removed)
                .StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 1 }));

            Context.Entities.RemoveComponent(entity, ComponentConfig<TestComponent1>.Config);
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T12()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T123()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2"),
                Context.Tracking.CreateTracker("RemoveTracker3")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2,
                    TestComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4,
                    TestComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T1234()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2"),
                Context.Tracking.CreateTracker("RemoveTracker3"),
                Context.Tracking.CreateTracker("RemoveTracker4")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Removed);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3,
                TestSharedComponent1>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4,
                    TestComponent3,
                    TestSharedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent4,
                    TestSharedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T12345()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2"),
                Context.Tracking.CreateTracker("RemoveTracker3"),
                Context.Tracking.CreateTracker("RemoveTracker4"),
                Context.Tracking.CreateTracker("RemoveTracker5")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Removed);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Removed);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3,
                TestSharedComponent1,
                TestSharedComponent2>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent2>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent4,
                    TestSharedComponent1,
                    TestSharedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestComponent4,
                    TestSharedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T123456()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2"),
                Context.Tracking.CreateTracker("RemoveTracker3"),
                Context.Tracking.CreateTracker("RemoveTracker4"),
                Context.Tracking.CreateTracker("RemoveTracker5"),
                Context.Tracking.CreateTracker("RemoveTracker6")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Removed);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Removed);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Removed);
            trackers[5].SetTrackingState<TestSharedComponent3>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3,
                TestSharedComponent1,
                TestSharedComponent2,
                TestSharedComponent3>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent3>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent4,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestComponent4,
                    TestSharedComponent2,
                    TestSharedComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestComponent4,
                    TestSharedComponent3>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T1234567()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2"),
                Context.Tracking.CreateTracker("RemoveTracker3"),
                Context.Tracking.CreateTracker("RemoveTracker4"),
                Context.Tracking.CreateTracker("RemoveTracker5"),
                Context.Tracking.CreateTracker("RemoveTracker6"),
                Context.Tracking.CreateTracker("RemoveTracker7")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Removed);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Removed);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Removed);
            trackers[5].SetTrackingState<TestSharedComponent3>(TrackingState.Removed);
            trackers[6].SetTrackingState<TestManagedComponent1>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3,
                TestSharedComponent1,
                TestSharedComponent2,
                TestSharedComponent3,
                TestManagedComponent1>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasManagedComponent<TestManagedComponent1>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent4,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestComponent4,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestComponent4,
                    TestSharedComponent3,
                    TestManagedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestComponent4,
                    TestManagedComponent1>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponents_T12345678()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2"),
                Context.Tracking.CreateTracker("RemoveTracker3"),
                Context.Tracking.CreateTracker("RemoveTracker4"),
                Context.Tracking.CreateTracker("RemoveTracker5"),
                Context.Tracking.CreateTracker("RemoveTracker6"),
                Context.Tracking.CreateTracker("RemoveTracker7"),
                Context.Tracking.CreateTracker("RemoveTracker8")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Removed);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Removed);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Removed);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Removed);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Removed);
            trackers[5].SetTrackingState<TestSharedComponent3>(TrackingState.Removed);
            trackers[6].SetTrackingState<TestManagedComponent1>(TrackingState.Removed);
            trackers[7].SetTrackingState<TestManagedComponent2>(TrackingState.Removed);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3,
                TestSharedComponent1,
                TestSharedComponent2,
                TestSharedComponent3,
                TestManagedComponent1,
                TestManagedComponent2>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasSharedComponent<TestSharedComponent3>(entity));
            Assert.IsFalse(Context.Entities.HasManagedComponent<TestManagedComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasManagedComponent<TestManagedComponent2>(entity));

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1,
                    TestComponent1>(entity));

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent4,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent4,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent4,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestComponent4,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestComponent4,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestComponent4,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestComponent4,
                    TestManagedComponent2>(entity));
            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestComponent4>(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.RemoveComponents<
                    TestComponent1,
                    TestComponent2,
                    TestComponent3,
                    TestSharedComponent1,
                    TestSharedComponent2,
                    TestSharedComponent3,
                    TestManagedComponent1,
                    TestManagedComponent2>(entity));
        }

        private Entity CreateTestEntity()
            => Context.Entities.CreateEntity(
            new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 0 })
                .SetComponent(new TestComponent2 { Prop = 0 })
                .SetComponent(new TestComponent3 { Prop = 0 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 0 })
                .SetSharedComponent(new TestSharedComponent2 { Prop = 0 })
                .SetSharedComponent(new TestSharedComponent3 { Prop = 0 })
                .SetManagedComponent(new TestManagedComponent1 { Prop = 0 })
                .SetManagedComponent(new TestManagedComponent2 { Prop = 0 }));
    }
}
