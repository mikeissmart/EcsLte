using EcsLte.Exceptions;
using EcsLte.HybridArcheType;
using EcsLte.UnitTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EcsContextHybridTests
{
    [TestClass]
    public class EscContext_Hybrid_EntityLifeTest : IEntityLifeTest
    {
        public EcsContext_Hybrid Context { get; private set; }

        [TestInitialize]
        public void PreTest() => Context = EcsContexts.CreateEcsContext_Hybrid("Test_Hybrid");

        [TestCleanup]
        public void PostTest()
        {
            if (!Context.IsDestroyed)
                EcsContexts.DestroyContext_Hybrid(Context);
            Context = null;
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.HasEntity(entities[0]));
            Assert.IsTrue(Context.HasEntity(entities[1]));
            Assert.IsTrue(Context.GetEntities()[0] == entities[0]);
            Assert.IsTrue(Context.GetEntities()[1] == entities[1]);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0].Version == 1);
            Assert.IsTrue(entities[1].Version == 1);
        }

        [TestMethod]
        public void CreateEntities_Destroy()
        {
            EcsContexts.DestroyContext_Hybrid(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntities(2, new EntityBlueprint_Hybrid()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(entities[i].Id == i + 1, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities_Reuse()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntities(entities);

            entities = Context.CreateEntities(2, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            for (int i = 0, lifoId = 2; i < entities.Length; i++, lifoId--)
            {
                Assert.IsTrue(entities[i].Id == lifoId, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities_Reuse_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntities(entities);

            entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            for (int i = 0, lifoId = UnitTestConsts.LargeCount; i < entities.Length; i++, lifoId--)
            {
                Assert.IsTrue(entities[i].Id == lifoId, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.HasEntity(entity));
            Assert.IsTrue(Context.GetEntities()[0] == entity);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
        }

        [TestMethod]
        public void CreateEntity_Destroy()
        {
            EcsContexts.DestroyContext_Hybrid(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntity_Large()
        {
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(entities[i].Id == i + 1, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntity_Reuse()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntity(entity);

            entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
            Assert.IsTrue(Context.HasEntity(entity));
            Assert.IsTrue(Context.GetEntities()[0] == entity);
        }

        [TestMethod]
        public void CreateEntity_Reuse_Large()
        {
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            }

            Context.DestroyEntities(entities);

            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));
            }

            for (int i = 0, lifoId = UnitTestConsts.LargeCount; i < entities.Length; i++, lifoId--)
            {
                Assert.IsTrue(entities[i].Id == lifoId, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Context.DestroyEntities(entities);

            Assert.IsFalse(Context.HasEntity(entities[0]));
            Assert.IsFalse(Context.HasEntity(entities[1]));
            Assert.IsTrue(Context.GetEntities().Length == 0);
        }

        [TestMethod]
        public void DestroyEntities_Destroy()
        {
            EcsContexts.DestroyContext_Hybrid(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntities(new Entity[0]));
        }

        [TestMethod]
        public void DestroyEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Context.DestroyEntities(entities);

            Assert.IsTrue(Context.GetEntities().Length == 0);
            for (var i = 0; i < entities.Length; i++)
                Assert.IsFalse(Context.HasEntity(entities[i]), $"Entity.Id {entities[i].Id}");
        }

        [TestMethod]
        public void DestroyEntities_EntityNull() => Assert.ThrowsException<ArgumentNullException>(() =>
                                                      Context.DestroyEntities(null));

        [TestMethod]
        public void DestroyEntities_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                Context.DestroyEntities(new Entity[] { Entity.Null }));

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Context.DestroyEntity(entity);

            Assert.IsFalse(Context.HasEntity(entity));
            Assert.IsTrue(Context.GetEntities().Length == 0);
        }

        [TestMethod]
        public void DestroyEntity_Destroy()
        {
            EcsContexts.DestroyContext_Hybrid(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntity_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                              Context.DestroyEntity(Entity.Null));
    }
}
