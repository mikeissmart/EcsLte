using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest
{
    public abstract class BasePrePostTest
    {
        public EcsContext Context { get; private set; }

        [TestInitialize]
        public void PreTest() => Context = EcsContexts.CreateContext("UnitTest");

        [TestCleanup]
        public void PostTest()
        {
            foreach (var context in EcsContexts.GetAllContexts())
                EcsContexts.DestroyContext(context);
            Context = null;
        }

        protected Entity[] TestCreateEntities<T1>(EcsContext context, int entityCount,
            T1 component1)
            where T1 : unmanaged, IComponent
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(component1);
            return context.CreateEntities(entityCount, blueprint);
        }

        protected Entity[] TestCreateEntities<T1, T2>(EcsContext context, int entityCount,
            T1 component1,
            T2 component2)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2);
            return context.CreateEntities(entityCount, blueprint);
        }

        protected Entity[] TestCreateEntities<T1, T2, T3>(EcsContext context, int entityCount,
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3);
            return context.CreateEntities(entityCount, blueprint);
        }

        protected void AssertClassEquals<T>(T same1, T same2, T different, T nullable) where T : IEquatable<T>
        {
            Assert.IsTrue(same1.GetHashCode() != 0);
            Assert.IsTrue(same1.GetHashCode() == same2.GetHashCode());
            Assert.IsFalse(same1.GetHashCode() == different.GetHashCode());

            Assert.IsTrue(same1.Equals(same2));
            Assert.IsFalse(different.Equals(same1));
            Assert.IsFalse(same1.Equals(nullable));
        }
    }
}