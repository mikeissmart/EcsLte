using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_Entity : BasePrePostTest
    {
        [TestMethod]
        public void CreateEntity_Null() => Assert.ThrowsException<ArgumentNullException>(() =>
                                                      Context.CreateEntity(null));

        [TestMethod]
        public void CreateEntity_NoComponents() => Assert.ThrowsException<EntityBlueprintNoComponentsException>(() =>
                                                              Context.CreateEntity(new EntityBlueprint()));

        [TestMethod]
        public void CreateEntity_DuplicateUniqueComponent()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestUniqueComponent1())));
        }

        [TestMethod]
        public void CreateEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntity()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityCount() == entities.Length);
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
            var entities = Context.CreateEntities(UnitTestConsts.SmallCount, blueprint);
            Context.DestroyEntities(entities);
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityCount() == entities.Length);
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
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));

            Assert.IsTrue(Context.EntityCount() == 2);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[0].Version == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[1].Version == 1);
            Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[0]).Prop == 1);
            Assert.IsTrue(Context.GetAllComponents(entities[0]).Length == 1);
            Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[1]).Prop == 1);
            Assert.IsTrue(Context.GetAllComponents(entities[1]).Length == 1);
        }

        [TestMethod]
        public void CreateEntities_DuplicateUniqueComponent() => Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                                                                            Context.CreateEntities(2, new EntityBlueprint()
                                                                                .AddComponent(new TestUniqueComponent1())));

        [TestMethod]
        public void CreateEntities_NegativeCount() => Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                                                                 Context.CreateEntities(-1, new EntityBlueprint()
                                                                     .AddComponent(new TestComponent1())));

        [TestMethod]
        public void CreateEntities_NoComponents() => Assert.ThrowsException<EntityBlueprintNoComponentsException>(() =>
                                                                Context.CreateEntities(1, new EntityBlueprint()));

        [TestMethod]
        public void CreateEntities_Null() => Assert.ThrowsException<ArgumentNullException>(() =>
                                                        Context.CreateEntities(1, null));

        [TestMethod]
        public void CreateEntities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntities(1, new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntities_Reuse()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.CreateEntities(2, blueprint);
            Context.DestroyEntities(entities);
            entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount() == 2);
            Assert.IsTrue(entities[0].Version == 2);
            Assert.IsTrue(entities[1].Version == 2);
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.DestroyEntity(entity);

            Assert.IsTrue(Context.EntityCount() == 1);
            Assert.IsFalse(Context.HasEntity(entity));
            Assert.IsTrue(Context.HasEntity(keepEntity));
        }

        [TestMethod]
        public void DestroyEntity_UniqueComponent()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            Context.DestroyEntity(entity);

            Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void DestroyEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntity_Never() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                        Context.DestroyEntity(Entity.Null));

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent2()))[0];
            Context.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityCount() == 1);
            Assert.IsTrue(Context.HasEntity(keepEntity));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_UniqueComponent()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            Context.DestroyEntities(new Entity[] { entity });

            Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void DestroyEntities_Null()
        {
            Entity[] entities = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.DestroyEntities(entities));
        }

        [TestMethod]
        public void DestroyEntities_Never_All()
        {
            var entities = new Entity[] { Entity.Null, Entity.Null };
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.DestroyEntities(entities));
        }

        [TestMethod]
        public void DestroyEntities_Never_Some()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())),
                Entity.Null
            };
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.DestroyEntities(entities));
        }

        [TestMethod]
        public void DestroyEntities_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntities(new Entity[0]));
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            var entities = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent2()))[0];
            Context.DestroyEntities(archeType);

            Assert.IsTrue(Context.EntityCount() == 1);
            Assert.IsTrue(Context.HasEntity(keepEntity));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntities(archeType));
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.DestroyEntities(archeType));
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType_UniqueComponent()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>();
            Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            Context.DestroyEntities(archeType);

            Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery()
        {
            var entityQuery = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            var entities = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent2()))[0];
            Context.DestroyEntities(entityQuery);

            Assert.IsTrue(Context.EntityCount() == 1);
            Assert.IsTrue(Context.HasEntity(keepEntity));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery_Null()
        {
            EntityQuery entityQuery = null;
            ;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.DestroyEntities(entityQuery));
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery_Destroyed()
        {
            var entityQuery = new EntityQuery();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntities(entityQuery));
        }

        [TestMethod]
        public void HasEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(Entity.Null));
        }

        [TestMethod]
        public void HasEntity()
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
        public void HasEntity_EntityArcheType()
        {
            var entity1 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var entity2 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(Context.HasEntity(entity1, archeType));
            Assert.IsFalse(Context.HasEntity(entity2, archeType));
            Assert.IsFalse(Context.HasEntity(Entity.Null, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityArcheType_Manage()
        {
            var entity1 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestManageComponent1()));
            var entity2 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestManageComponent1>();

            Assert.IsTrue(Context.HasEntity(entity1, archeType));
            Assert.IsFalse(Context.HasEntity(entity2, archeType));
            Assert.IsFalse(Context.HasEntity(Entity.Null, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityArcheType_Null()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.HasEntity(entity, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityArcheType_Destroyed()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(entity, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityQuery()
        {
            var entity1 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var entity2 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(Context.HasEntity(entity1, query));
            Assert.IsFalse(Context.HasEntity(entity2, query));
            Assert.IsFalse(Context.HasEntity(Entity.Null, query));
        }

        [TestMethod]
        public void HasEntity_EntityQuery_Manage()
        {
            var entity1 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestManageComponent1()));
            var entity2 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestManageComponent2()));
            var query = new EntityQuery()
                .WhereAllOf<TestManageComponent1>();

            Assert.IsTrue(Context.HasEntity(entity1, query));
            Assert.IsFalse(Context.HasEntity(entity2, query));
            Assert.IsFalse(Context.HasEntity(Entity.Null, query));
        }

        [TestMethod]
        public void HasEntity_EntityQuery_Null()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            EntityQuery archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.HasEntity(entity, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityQuery_Destroyed()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(entity, query));
        }

        [TestMethod]
        public void GetEntities()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var getEntities = Context.GetEntities();

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_Destroyed()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetEntities());
        }

        [TestMethod]
        public void GetEntities_EntityArcheType()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            var getEntities = Context.GetEntities(archeType);

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_EntityArcheType_Manage()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestManageComponent1()));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestManageComponent2()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestManageComponent1>();

            var getEntities = Context.GetEntities(archeType);

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_EntityArcheType_Null()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetEntities(archeType));
        }

        [TestMethod]
        public void GetEntities_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetEntities(archeType));
        }

        [TestMethod]
        public void GetEntities_EntityQuery()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            var getEntities = Context.GetEntities(query);

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_EntityQuery_Manage()
        {
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestManageComponent1()));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestManageComponent2()));
            var query = new EntityQuery()
                .WhereAllOf<TestManageComponent1>();

            var getEntities = Context.GetEntities(query);

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_EntityQuery_Null()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            EntityQuery query = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.GetEntities(query));
        }

        [TestMethod]
        public void GetEntities_EntityQuery_Destroyed()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetEntities(query));
        }
    }
}