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
                .SetTrackingComponent<TestComponent1>(true)
                .SetTrackingEntities(true);

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 }));

            Context.Entities.RemoveComponent<TestComponent1>(entity);
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            // Remove and destroy not trackable
            Assert.IsTrue(Context.Entities.GetEntities(removeTracker).Length == 0);

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
                .SetTrackingComponent<TestComponent1>(true);

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 1 }));

            Context.Entities.RemoveComponent(entity, ComponentConfig<TestComponent1>.Config);
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);
        }

        [TestMethod]
        public void RemoveComponents_T12()
        {
            var trackers = new[]
            {
                Context.Tracking.CreateTracker("RemoveTracker1"),
                Context.Tracking.CreateTracker("RemoveTracker2")
            };
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);

            var entity = CreateTestEntity();

            Context.Entities.RemoveComponents<
                TestComponent1,
                TestComponent2,
                TestComponent3>(entity);

            Assert.IsFalse(Context.Entities.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent2>(entity));
            Assert.IsFalse(Context.Entities.HasComponent<TestComponent3>(entity));
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);
            trackers[5].SetTrackingComponent<TestSharedComponent3>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);
            trackers[5].SetTrackingComponent<TestSharedComponent3>(true);
            trackers[6].SetTrackingComponent<TestManagedComponent1>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
            trackers[0].SetTrackingComponent<TestComponent1>(true);
            trackers[1].SetTrackingComponent<TestComponent2>(true);
            trackers[2].SetTrackingComponent<TestComponent3>(true);
            trackers[3].SetTrackingComponent<TestSharedComponent1>(true);
            trackers[4].SetTrackingComponent<TestSharedComponent2>(true);
            trackers[5].SetTrackingComponent<TestSharedComponent3>(true);
            trackers[6].SetTrackingComponent<TestManagedComponent1>(true);
            trackers[7].SetTrackingComponent<TestManagedComponent2>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            for (var i = 0; i < trackers.Length; i++)
            {
                var tracker = trackers[i];
                // Remove and destroy not trackable
                var trackedEntities = Context.Entities.GetEntities(tracker);
                Assert.IsTrue(trackedEntities.Length == 0, i.ToString());
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
