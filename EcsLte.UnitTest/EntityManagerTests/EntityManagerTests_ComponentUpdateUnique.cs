using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentUpdateUnique : BasePrePostTest
    {
        [TestMethod]
        public void UpdateUniqueComponent()
        {
            var updateUniqueTracker = Context.Tracking.CreateTracker("UpdateUniqueTracker");
            updateUniqueTracker.SetComponentState<TestUniqueComponent1>(EntityTrackerState.Updated);
            updateUniqueTracker.StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetUniqueComponent(new TestUniqueComponent1 { Prop = 0 }),
                EntityState.Active);

            Context.Entities.UpdateUniqueComponent(entity, new TestUniqueComponent1 { Prop = 1 });
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>().Prop == 1);

            Assert.IsTrue(Context.Entities.GetEntities(updateUniqueTracker).Length == 1);
            Assert.IsTrue(Context.Entities.GetEntities(updateUniqueTracker)[0] == entity);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.UpdateUniqueComponent(entity, new TestUniqueComponent2()));
            Assert.ThrowsException<EntityUniqueComponentNotExistsException>(() =>
                Context.Entities.UpdateUniqueComponent(new TestUniqueComponent2()));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.UpdateUniqueComponent(Entity.Null, new TestUniqueComponent1()));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateUniqueComponent(entity, new TestUniqueComponent1()));
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.UpdateUniqueComponent(new TestUniqueComponent1()));
        }
    }
}
