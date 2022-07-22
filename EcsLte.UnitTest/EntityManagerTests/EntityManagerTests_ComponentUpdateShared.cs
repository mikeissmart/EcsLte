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
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker");
            updateSharedTracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            updateSharedTracker.StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 }),
                EntityState.Active);

            Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent1 { Prop = 1 });
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 1);

            Assert.IsTrue(Context.Entities.GetEntities(updateSharedTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(updateSharedTracker)[0] == entity);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
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
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker");
            updateSharedTracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            updateSharedTracker.StartTracking();

            var archeType1 = new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 0 });
            var archeType2 = new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            var entities = Context.Entities.CreateEntities(
                archeType1,
                EntityState.Active,
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

            Assert.ThrowsException<EntityArcheTypeNotHaveComponentException>(() =>
                Context.Entities.UpdateSharedComponent(archeType1, new TestSharedComponent2()));

            AssertArcheType_Invalid_Null(
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
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker");
            updateSharedTracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            updateSharedTracker.StartTracking();

            var anyFilter = new EntityFilter()
                    .WhereAllOf<TestSharedComponent1>();
            var updatedFilter = new EntityFilter()
                    .FilterBy(new TestSharedComponent1 { Prop = 10 });
            var filters = new EntityFilter[5];

            var blueprint = new EntityBlueprint();
            var createdEntities = new Entity[0];
            for (var i = 0; i < 5; i++)
            {
                filters[i] = new EntityFilter()
                    .FilterBy(new TestSharedComponent1 { Prop = i + 1 });
                blueprint.SetSharedComponent(new TestSharedComponent1 { Prop = i + 1 });
                Context.Entities.CreateEntities(
                    blueprint,
                    EntityState.Active,
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
            var addedTracker = Context.Tracking.CreateTracker("AddedTracker");
            addedTracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Added);
            addedTracker.StartTracking();
            var updatedTracker = Context.Tracking.CreateTracker("UpdatedTracker");
            updatedTracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            updatedTracker.StartTracking();

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 0 }),
                EntityState.Active,
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
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker");
            updateSharedTracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Updated);
            updateSharedTracker.StartTracking();

            var query0FilterAddedTracker = new EntityQuery(
                Context.Tracking.CreateTracker("AddedTracker"),
                new EntityFilter()
                    .FilterBy(new TestSharedComponent1 { Prop = 0 }));
            query0FilterAddedTracker.Tracker.SetComponentState<TestSharedComponent1>(EntityTrackerState.Added);
            query0FilterAddedTracker.Tracker.StartTracking();

            var query1FilterTracker = new EntityQuery(
                Context,
                new EntityFilter()
                    .FilterBy(new TestSharedComponent1 { Prop = 1 }));

            var filter10 = new EntityFilter()
                .FilterBy(new TestSharedComponent1 { Prop = 10 });

            var createdEntities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 0 }),
                EntityState.Active,
                UnitTestConsts.SmallCount);
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                EntityState.Active,
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
                new EntityQuery(EcsContexts.CreateContext("DiffContext"),
                    new EntityFilter()
                        .WhereAllOf<TestSharedComponent1>()),
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
