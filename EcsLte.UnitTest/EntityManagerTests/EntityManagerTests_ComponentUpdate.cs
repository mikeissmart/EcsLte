using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentUpdate : BasePrePostTest
    {
        [TestMethod]
        public void UpdateComponent()
        {
            var updateTracker = Context.Tracking.CreateTracker("UpdateTracker");
            updateTracker.SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            updateTracker.StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 }),
                EntityState.Active);

            Context.Entities.UpdateComponent(entity, new TestComponent1 { Prop = 2 });
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 2);

            Assert.IsTrue(Context.Entities.GetEntities(updateTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(updateTracker)[0] == entity);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponent(entity, new TestComponent2()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponent(Entity.Null, new TestComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponents_T12()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
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
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2()));
        }

        [TestMethod]
        public void UpdateComponents_T123()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2"),
                Context.Tracking.CreateTracker("UpdateTracker3")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);
            trackers[2].SetComponentState<TestComponent3>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
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
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3()));
        }

        [TestMethod]
        public void UpdateComponents_T1234()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2"),
                Context.Tracking.CreateTracker("UpdateTracker3"),
                Context.Tracking.CreateTracker("UpdateTracker4")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);
            trackers[2].SetComponentState<TestComponent3>(EntityTrackerState.Updated);
            trackers[3].SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
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
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateComponents_T12345()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2"),
                Context.Tracking.CreateTracker("UpdateTracker3"),
                Context.Tracking.CreateTracker("UpdateTracker4"),
                Context.Tracking.CreateTracker("UpdateTracker5")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);
            trackers[2].SetComponentState<TestComponent3>(EntityTrackerState.Updated);
            trackers[3].SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            trackers[4].SetComponentState<TestSharedComponent2>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
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
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
        }

        [TestMethod]
        public void UpdateComponents_T123456()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2"),
                Context.Tracking.CreateTracker("UpdateTracker3"),
                Context.Tracking.CreateTracker("UpdateTracker4"),
                Context.Tracking.CreateTracker("UpdateTracker5"),
                Context.Tracking.CreateTracker("UpdateTracker6")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);
            trackers[2].SetComponentState<TestComponent3>(EntityTrackerState.Updated);
            trackers[3].SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            trackers[4].SetComponentState<TestSharedComponent2>(EntityTrackerState.Updated);
            trackers[5].SetComponentState<TestSharedComponent3>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
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
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
        }

        [TestMethod]
        public void UpdateComponents_T1234567()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2"),
                Context.Tracking.CreateTracker("UpdateTracker3"),
                Context.Tracking.CreateTracker("UpdateTracker4"),
                Context.Tracking.CreateTracker("UpdateTracker5"),
                Context.Tracking.CreateTracker("UpdateTracker6"),
                Context.Tracking.CreateTracker("UpdateTracker7")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);
            trackers[2].SetComponentState<TestComponent3>(EntityTrackerState.Updated);
            trackers[3].SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            trackers[4].SetComponentState<TestSharedComponent2>(EntityTrackerState.Updated);
            trackers[5].SetComponentState<TestSharedComponent3>(EntityTrackerState.Updated);
            trackers[6].SetComponentState<TestUniqueComponent1>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 },
                new TestUniqueComponent1 { Prop = 7 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 7);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestComponent4(),
                    new TestUniqueComponent1()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));
        }

        [TestMethod]
        public void UpdateComponents_T12345678()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("UpdateTracker1"),
                Context.Tracking.CreateTracker("UpdateTracker2"),
                Context.Tracking.CreateTracker("UpdateTracker3"),
                Context.Tracking.CreateTracker("UpdateTracker4"),
                Context.Tracking.CreateTracker("UpdateTracker5"),
                Context.Tracking.CreateTracker("UpdateTracker6"),
                Context.Tracking.CreateTracker("UpdateTracker7"),
                Context.Tracking.CreateTracker("UpdateTracker8")
            };
            foreach (var tracker in trackers)
                tracker.StartTracking();
            trackers[0].SetComponentState<TestComponent1>(EntityTrackerState.Updated);
            trackers[1].SetComponentState<TestComponent2>(EntityTrackerState.Updated);
            trackers[2].SetComponentState<TestComponent3>(EntityTrackerState.Updated);
            trackers[3].SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            trackers[4].SetComponentState<TestSharedComponent2>(EntityTrackerState.Updated);
            trackers[5].SetComponentState<TestSharedComponent3>(EntityTrackerState.Updated);
            trackers[6].SetComponentState<TestUniqueComponent1>(EntityTrackerState.Updated);
            trackers[7].SetComponentState<TestUniqueComponent2>(EntityTrackerState.Updated);

            var entity = CreateTestEntity();

            Context.Entities.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 },
                new TestUniqueComponent1 { Prop = 7 },
                new TestUniqueComponent2 { Prop = 8 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 7);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent2>(entity).Prop == 8);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 1, i.ToString());
                Assert.IsTrue(trackedEntities[0] == entity, i.ToString());
            }

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent4(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent4(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent4(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestComponent4(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestComponent4(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestComponent4(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestComponent4(),
                    new TestUniqueComponent2()));
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestComponent4()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateComponents(Entity.Null,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
        }

        private Entity CreateTestEntity() => Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 0 })
                    .SetComponent(new TestComponent2 { Prop = 0 })
                    .SetComponent(new TestComponent3 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent2 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent3 { Prop = 0 })
                    .SetUniqueComponent(new TestUniqueComponent1 { Prop = 0 })
                    .SetUniqueComponent(new TestUniqueComponent2 { Prop = 0 }),
                EntityState.Active);
    }
}
