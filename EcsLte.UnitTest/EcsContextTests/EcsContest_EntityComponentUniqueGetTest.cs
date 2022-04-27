using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContest_EntityComponentUniqueGetTest : BasePrePostTest, IEntityComponentUniqueGetTest
    {
        [TestMethod]
        public void Unique_GetUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetUniqueComponent<TestUniqueComponent1>().Prop == component.Prop);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueEntity()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.GetUniqueEntity<TestUniqueComponent1>() == entity);
        }

        [TestMethod]
        public void Unique_HasUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_HasUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.HasUniqueComponent<TestUniqueComponent1>());
        }
    }
}
