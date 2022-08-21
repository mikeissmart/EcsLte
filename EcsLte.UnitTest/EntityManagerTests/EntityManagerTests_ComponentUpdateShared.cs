using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentUpdateShared : BasePrePostTest
    {
        [TestMethod]
        public void UpdateSharedComponent()
        {
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker")
                .SetTrackingState<TestSharedComponent1>(TrackingState.Updated)
                .StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 }));

            Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent1 { Prop = 1 });
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 1);

            Assert.IsTrue(Context.Entities.GetEntities(updateSharedTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(updateSharedTracker)[0] == entity);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent2()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateSharedComponent(Entity.Null, new TestSharedComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_ArcheType()
        {
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker")
                .SetTrackingState<TestSharedComponent1>(TrackingState.Updated)
                .StartTracking();

            var archeType1 = Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 0 });
            var archeType2 = Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            var entities = Context.Entities.CreateEntities(
                archeType1,
                UnitTestConsts.SmallCount);

            Context.Entities.UpdateSharedComponent(archeType1, new TestSharedComponent1 { Prop = 1 });
            Assert.IsTrue(Context.Entities.EntityCount(archeType1) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(archeType2) == UnitTestConsts.SmallCount);

            var trackedEntities = Context.Entities.GetEntities(updateSharedTracker);
            Assert.IsTrue(trackedEntities.Length == UnitTestConsts.SmallCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(trackedEntities.Any(x => x == entities[i]),
                    $"Tracked Entity: {entities[i]}");
            }

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.UpdateSharedComponent(archeType1, new TestSharedComponent2()));

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.UpdateSharedComponent(x, new TestSharedComponent1()),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponent(archeType1, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponents_Filter()
        {
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker")
                .SetTrackingState<TestSharedComponent1>(TrackingState.Updated)
                .StartTracking();

            var anyFilter = Context.Filters
                    .WhereAllOf<TestSharedComponent1>();
            var updatedFilter = Context.Filters
                    .FilterBy(new TestSharedComponent1 { Prop = 10 });
            var filters = new EntityFilter[5];

            var blueprint = new EntityBlueprint();
            var createdEntities = new Entity[0];
            for (var i = 0; i < 5; i++)
            {
                filters[i] = Context.Filters
                    .FilterBy(new TestSharedComponent1 { Prop = i + 1 });
                blueprint.SetSharedComponent(new TestSharedComponent1 { Prop = i + 1 });
                Context.Entities.CreateEntities(
                    blueprint,
                    ref createdEntities,
                    createdEntities.Length,
                    UnitTestConsts.SmallCount);
            }

            Context.Entities.UpdateSharedComponents(anyFilter, new TestSharedComponent1 { Prop = 10 });
            for (var i = 0; i < 5; i++)
            {
                Assert.IsTrue(Context.Entities.EntityCount(filters[i]) == 0);
            }
            Assert.IsTrue(Context.Entities.EntityCount(updatedFilter) == UnitTestConsts.SmallCount * 5);

            var trackedEntities = Context.Entities.GetEntities(updateSharedTracker);
            Assert.IsTrue(trackedEntities.Length == UnitTestConsts.SmallCount * 5);
            for (var i = 0; i < createdEntities.Length; i++)
            {
                Assert.IsTrue(trackedEntities.Any(x => x == createdEntities[i]),
                    $"Tracked Entity: {createdEntities[i]}");
            }

            var emptyComponents = new TestSharedComponent1[0];
            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.UpdateSharedComponents(x, new TestSharedComponent1()),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponents(anyFilter, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponents_Tracker()
        {
            var addedTracker = Context.Tracking.CreateTracker("AddedTracker")
                .SetTrackingState<TestSharedComponent1>(TrackingState.Added)
                .StartTracking();
            var updatedTracker = Context.Tracking.CreateTracker("UpdatedTracker")
                .SetTrackingState<TestSharedComponent1>(TrackingState.Updated)
                .StartTracking();

            Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 0 }),
                UnitTestConsts.SmallCount);

            Context.Entities.UpdateSharedComponents(addedTracker, new TestSharedComponent1 { Prop = 10 });
            var entities = Context.Entities.GetEntities(updatedTracker);
            Assert.IsTrue(entities.Length == UnitTestConsts.SmallCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entities[i]).Prop == 10,
                    $"Entity: {entities[i]}");
            }

            var emptyComponents = new TestSharedComponent1[0];
            var diffContext = EcsContexts.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.UpdateSharedComponents(x, new TestSharedComponent1()),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponents(updatedTracker, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponents_Query()
        {
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker")
                .SetTrackingState<TestSharedComponent1>(TrackingState.Updated)
                .StartTracking();

            var query0FilterAddedTracker = Context.Queries
                .SetTracker(Context.Tracking.CreateTracker("AddedTracker"))
                .SetFilter(Context.Filters
                    .FilterBy(new TestSharedComponent1 { Prop = 0 }));
            query0FilterAddedTracker.Tracker.SetTrackingState<TestSharedComponent1>(TrackingState.Added);
            query0FilterAddedTracker.Tracker.StartTracking();

            var query1FilterTracker = Context.Queries
                .SetFilter(Context.Filters
                    .FilterBy(new TestSharedComponent1 { Prop = 1 }));

            var filter10 = Context.Filters
                .FilterBy(new TestSharedComponent1 { Prop = 10 });

            var createdEntities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 0 }),
                UnitTestConsts.SmallCount);
            Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                ref createdEntities,
                createdEntities.Length,
                UnitTestConsts.SmallCount);

            Context.Entities.UpdateSharedComponents(query0FilterAddedTracker, new TestSharedComponent1 { Prop = 10 });
            Context.Entities.UpdateSharedComponents(query1FilterTracker, new TestSharedComponent1 { Prop = 10 });
            var filteredEntities = Context.Entities.GetEntities(filter10);
            Assert.IsTrue(filteredEntities.Length == UnitTestConsts.SmallCount * 2);
            for (var i = 0; i < filteredEntities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(filteredEntities[i]).Prop == 10,
                    $"Entity: {filteredEntities[i]}");
            }

            var trackedEntities = Context.Entities.GetEntities(updateSharedTracker);
            Assert.IsTrue(trackedEntities.Length == UnitTestConsts.SmallCount * 2);
            for (var i = 0; i < createdEntities.Length; i++)
            {
                Assert.IsTrue(trackedEntities.Any(x => x == createdEntities[i]),
                    $"Tracked Entity: {createdEntities[i]}");
            }

            var emptyComponents = new TestSharedComponent1[0];
            Context.Tracking.RemoveTracker(query0FilterAddedTracker.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                query0FilterAddedTracker,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.UpdateSharedComponents(x, new TestSharedComponent1()),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponents(query1FilterTracker, new TestSharedComponent1()));
        }
    }
}
