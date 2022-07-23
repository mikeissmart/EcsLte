using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentGetUnique : BasePrePostTest
    {
        [TestMethod]
        public void GetSharedComponent()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetUniqueComponent(new TestUniqueComponent1 { Prop = 1 }),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>().Prop == 1);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.Entities.GetUniqueComponent<TestUniqueComponent2>(entity));
            Assert.ThrowsException<EntityUniqueComponentNotExistsException>(() =>
                Context.Entities.GetUniqueComponent<TestUniqueComponent2>());

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetUniqueComponent<TestUniqueComponent1>(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity));
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetUniqueComponent<TestUniqueComponent1>());
        }
    }
}
