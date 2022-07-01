using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityTrackerTests
{
    [TestClass]
    public class EntityTrackerTests : BasePrePostTest
    {
        [TestMethod]
        public void Create_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() => new EntityTracker(Context));
        }

        [TestMethod]
        public void Create_Null() => Assert.ThrowsException<ArgumentNullException>(() => new EntityTracker(null));

        [TestMethod]
        public void StartComponentTrack_Added()
        {
            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added);

            Assert.IsTrue(tracker.AddedTrackComponentTypes.Length == 1);
            Assert.IsTrue(tracker.AddedTrackComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsTrue(tracker.UpdatedTrackComponentTypes.Length == 0);
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));
        }

        [TestMethod]
        public void StartComponentTrack_Updated()
        {
            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated);

            Assert.IsTrue(tracker.AddedTrackComponentTypes.Length == 0);
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsTrue(tracker.UpdatedTrackComponentTypes.Length == 1);
            Assert.IsTrue(tracker.UpdatedTrackComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));
        }

        [TestMethod]
        public void StartComponentTrack_AddedOrUpdated()
        {
            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated);

            Assert.IsTrue(tracker.AddedTrackComponentTypes.Length == 1);
            Assert.IsTrue(tracker.AddedTrackComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsTrue(tracker.UpdatedTrackComponentTypes.Length == 1);
            Assert.IsTrue(tracker.UpdatedTrackComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));
        }

        [TestMethod]
        public void StartComponentTrack_Change()
        {
            var tracker = new EntityTracker(Context);

            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));

            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added);
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));

            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated);
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsFalse(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));

            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated);
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated));
            Assert.IsTrue(tracker.GetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated));
        }

        [TestMethod]
        public void ChangeComponentTrackingState_Added()
        {
            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added);
            tracker.StartTracking();

            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount(tracker) == 1);
            Assert.IsTrue(Context.GetEntities(tracker)[0] == entity);
        }

        [TestMethod]
        public void ChangeComponentTrackingState_Updated()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Updated);
            tracker.StartTracking();

            Context.UpdateComponent(entity, new TestComponent1 { Prop = 1 });

            Assert.IsTrue(Context.EntityCount(tracker) == 1);
            Assert.IsTrue(Context.GetEntities(tracker)[0] == entity);
        }

        [TestMethod]
        public void ChangeComponentTrackingState_AddedOrUpdated()
        {
            var entity1 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.AddedOrUpdated);
            tracker.StartTracking();

            Context.UpdateComponent(entity1, new TestComponent1 { Prop = 1 });
            var entity2 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount(tracker) == 2);
            Assert.IsTrue(Context.GetEntities(tracker).Contains(entity1));
            Assert.IsTrue(Context.GetEntities(tracker).Contains(entity2));
        }

        [TestMethod]
        public void TrackingEntityDestroyed()
        {
            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added);
            tracker.StartTracking();

            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount(tracker) == 1);

            Context.DestroyEntity(entity);
            Assert.IsTrue(Context.EntityCount(tracker) == 0);
        }

        [TestMethod]
        public void ResetTracking()
        {
            var tracker = new EntityTracker(Context);
            tracker.SetComponentTrackingState<TestComponent1>(EntityTracker.TrackingState.Added);
            tracker.StartTracking();

            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            tracker.ResetTracking();

            Assert.IsTrue(Context.EntityCount(tracker) == 0);
        }
    }
}
