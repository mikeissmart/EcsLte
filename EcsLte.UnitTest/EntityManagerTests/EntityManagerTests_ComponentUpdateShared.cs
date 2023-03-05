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
                .SetTrackingComponent<TestSharedComponent1>(true);

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 }));

            Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent1 { Prop = 1 });
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

            Assert.IsTrue(Context.Entities.GetEntities(updateSharedTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(updateSharedTracker)[0] == entity);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent2()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateSharedComponent(Entity.Null, new TestSharedComponent1()));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponent(entity, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_ArcheType()
        {
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker")
                .SetTrackingComponent<TestSharedComponent1>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 3);

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

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponent(archeType1, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponents_Filter()
        {
            var updateSharedTracker = Context.Tracking.CreateTracker("UpdateSharedTracker")
                .SetTrackingComponent<TestSharedComponent1>(true);

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
            Assert.IsTrue(Context.Entities.GlobalVersion.Version == 8);

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

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateSharedComponents(anyFilter, new TestSharedComponent1()));
        }
    }
}
