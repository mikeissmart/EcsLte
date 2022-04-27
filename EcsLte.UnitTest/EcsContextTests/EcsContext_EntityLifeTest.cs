using EcsLte.Exceptions;
using EcsLte.UnitTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_EntityLifeTest : BasePrePostTest, IEntityLifeTest
    {
        [TestMethod]
        public void CreateEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount == 1);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
        }

        [TestMethod]
        public void CreateEntity_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntity_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityCount == entities.Length);
            for (int i = 0, id = 1; i < entities.Length; i++, id++)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntity_Reuse()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entity = Context.CreateEntity(blueprint);
            Context.DestroyEntity(entity);
            entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount == 1);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
        }

        [TestMethod]
        public void CreateEntity_Reuse_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, blueprint);
            Context.DestroyEntities(entities);
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityCount == entities.Length);
            for (int i = 0, id = entities.Length; i < entities.Length; i++, id--)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount == 1);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[0].Version == 1);
        }

        [TestMethod]
        public void CreateEntities_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntities(1, new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount == entities.Length);
            for (int i = 0, id = 1; i < entities.Length; i++, id++)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities_Reuse()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.CreateEntities(1, blueprint);
            Context.DestroyEntities(entities);
            entities = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount == 1);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[0].Version == 2);
        }

        [TestMethod]
        public void CreateEntities_Reuse_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, blueprint);
            Context.DestroyEntities(entities);
            entities = Context.CreateEntities(UnitTestConsts.LargeCount, blueprint);

            Assert.IsTrue(Context.EntityCount == entities.Length);
            for (int i = 0, id = entities.Length; i < entities.Length; i++, id--)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntity(entity);

            Assert.IsTrue(Context.EntityCount == 0);
            Assert.IsFalse(Context.HasEntity(entity));
        }

        [TestMethod]
        public void DestroyEntity_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntity_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            for (var i = 0; i < entities.Length; i++)
                Context.DestroyEntity(entities[i]);

            Assert.IsTrue(Context.EntityCount == 0);
            for (int i = 0; i < entities.Length; i++)
                Assert.IsFalse(Context.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
        }

        [TestMethod]
        public void DestroyEntity_Never()
        {
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityCount == 0);
            for (int i = 0; i < entities.Length; i++)
                Assert.IsFalse(Context.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
        }

        [TestMethod]
        public void DestroyEntities_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntities(new Entity[0]));
        }

        [TestMethod]
        public void DestroyEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityCount == 0);
            for (int i = 0; i < entities.Length; i++)
                Assert.IsFalse(Context.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
        }

        [TestMethod]
        public void DestroyEntities_Never()
        {
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.DestroyEntities(new[] { Entity.Null }));
        }
    }
}
