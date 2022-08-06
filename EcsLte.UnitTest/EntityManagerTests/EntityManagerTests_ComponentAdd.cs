using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentAdd : BasePrePostTest
    {
        [TestMethod]
        public void AddComponent()
        {
            var addTracker = Context.Tracking.CreateTracker("AddTracker")
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .StartTracking();

            var entity = Context.Entities.CreateEntity();

            Context.Entities.AddComponent(entity, new TestComponent1 { Prop = 2 });
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 2);

            Assert.IsTrue(Context.Entities.GetEntities(addTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(addTracker)[0] == entity);

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponent(entity, new TestComponent1()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponent(Entity.Null, new TestComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void AddComponents_T12()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2()));
        }

        [TestMethod]
        public void AddComponents_T123()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2"),
                Context.Tracking.CreateTracker("AddTracker3")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3()));
        }

        [TestMethod]
        public void AddComponents_T1234()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2"),
                Context.Tracking.CreateTracker("AddTracker3"),
                Context.Tracking.CreateTracker("AddTracker4")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Added);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
        }

        [TestMethod]
        public void AddComponents_T12345()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2"),
                Context.Tracking.CreateTracker("AddTracker3"),
                Context.Tracking.CreateTracker("AddTracker4"),
                Context.Tracking.CreateTracker("AddTracker5")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Added);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Added);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
        }

        [TestMethod]
        public void AddComponents_T123456()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2"),
                Context.Tracking.CreateTracker("AddTracker3"),
                Context.Tracking.CreateTracker("AddTracker4"),
                Context.Tracking.CreateTracker("AddTracker5"),
                Context.Tracking.CreateTracker("AddTracker6")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Added);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Added);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Added);
            trackers[5].SetTrackingState<TestSharedComponent3>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
        }

        [TestMethod]
        public void AddComponents_T1234567()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2"),
                Context.Tracking.CreateTracker("AddTracker3"),
                Context.Tracking.CreateTracker("AddTracker4"),
                Context.Tracking.CreateTracker("AddTracker5"),
                Context.Tracking.CreateTracker("AddTracker6"),
                Context.Tracking.CreateTracker("AddTracker7")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Added);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Added);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Added);
            trackers[5].SetTrackingState<TestSharedComponent3>(TrackingState.Added);
            trackers[6].SetTrackingState<TestManagedComponent1>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 },
                new TestManagedComponent1 { Prop = 7 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);
            Assert.IsTrue(Context.Entities.GetManagedComponent<TestManagedComponent1>(entity).Prop == 7);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestComponent4(),
                    new TestManagedComponent1()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1()));
        }

        [TestMethod]
        public void AddComponents_T12345678()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("AddTracker1"),
                Context.Tracking.CreateTracker("AddTracker2"),
                Context.Tracking.CreateTracker("AddTracker3"),
                Context.Tracking.CreateTracker("AddTracker4"),
                Context.Tracking.CreateTracker("AddTracker5"),
                Context.Tracking.CreateTracker("AddTracker6"),
                Context.Tracking.CreateTracker("AddTracker7"),
                Context.Tracking.CreateTracker("AddTracker8")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetTrackingState<TestComponent1>(TrackingState.Added);
            trackers[1].SetTrackingState<TestComponent2>(TrackingState.Added);
            trackers[2].SetTrackingState<TestComponent3>(TrackingState.Added);
            trackers[3].SetTrackingState<TestSharedComponent1>(TrackingState.Added);
            trackers[4].SetTrackingState<TestSharedComponent2>(TrackingState.Added);
            trackers[5].SetTrackingState<TestSharedComponent3>(TrackingState.Added);
            trackers[6].SetTrackingState<TestManagedComponent1>(TrackingState.Added);
            trackers[7].SetTrackingState<TestManagedComponent2>(TrackingState.Added);

            var entity = CreateTestEntity();

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 },
                new TestManagedComponent1 { Prop = 7 },
                new TestManagedComponent2 { Prop = 8 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);
            Assert.IsTrue(Context.Entities.GetManagedComponent<TestManagedComponent1>(entity).Prop == 7);
            Assert.IsTrue(Context.Entities.GetManagedComponent<TestManagedComponent2>(entity).Prop == 8);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestComponent4(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestComponent4(),
                    new TestManagedComponent2()));
            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestManagedComponent1(),
                    new TestManagedComponent2()));
        }

        private Entity CreateTestEntity()
            => Context.Entities.CreateEntity();
    }
}
