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
    public class EcsContext_EntityGetTest : BasePrePostTest, IEntityGetTest
    {
        [TestMethod]
        public void HasEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(Entity.Null));
        }

        [TestMethod]
        public void HasEntity_Has()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.HasEntity(entity));
        }

        [TestMethod]
        public void GetEntities()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var getEntities = Context.GetEntities();

            Assert.IsTrue(getEntities.Length == 1);
            Assert.IsTrue(getEntities[0] == entity);
        }

        [TestMethod]
        public void GetEntities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetEntities());
        }

        [TestMethod]
        public void GetEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var getEntities = Context.GetEntities();

            Assert.IsTrue(getEntities.Length == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(getEntities[i] == entities[i],
                    $"Enity.Id {entities[i].Id}");
            }
        }
    }
}
