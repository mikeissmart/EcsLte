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
            var addTracker = Context.Tracking
                .SetTrackingComponent<TestComponent1>(true);

            var entity = Context.Entities.CreateEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            Context.Entities.AddComponent(entity, new TestComponent1 { Prop = 2 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            Assert.IsTrue(Context.Entities.GetEntities(addTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(addTracker)[0] == entity);

            Assert.ThrowsException<ComponentAlreadyHaveException>(() =>
                Context.Entities.AddComponent(entity, new TestComponent1()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.AddComponent(Entity.Null, new TestComponent1()));

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.AddComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void AddComponents_T12()
        {
            var trackers = new[]
            {
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

            Context.Entities.AddComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);
            trackers[5].SetTrackingComponent<TestSharedComponent3>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);
            trackers[5].SetTrackingComponent<TestSharedComponent3>(true);
            trackers[6].SetTrackingComponent<TestManagedComponent1>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges),
                Context.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges)
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);
            trackers[5].SetTrackingComponent<TestSharedComponent3>(true);
            trackers[6].SetTrackingComponent<TestManagedComponent1>(true);
            trackers[7].SetTrackingComponent<TestManagedComponent2>(true);

            var entity = CreateTestEntity();
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 2);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            // No change after error
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            EcsContexts.Instance.DestroyContext(Context);
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
