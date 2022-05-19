using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest : BasePrePostTest
    {
        private TestComponent1 _testComponent1_1 = new TestComponent1 { Prop = 1 };
        private TestSharedComponent1 _testSharedComponent1_1 = new TestSharedComponent1 { Prop = 2 };
        private TestUniqueComponent1 _testUniqueComponent1_1 = new TestUniqueComponent1 { Prop = 3 };
        private TestComponent1 _testComponent1_2 = new TestComponent1 { Prop = 4 };
        private TestSharedComponent1 _testSharedComponent1_2 = new TestSharedComponent1 { Prop = 5 };
        private TestUniqueComponent1 _testUniqueComponent1_2 = new TestUniqueComponent1 { Prop = 6 };

        [TestMethod]
        public void CreateEntity()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
            Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.EntityManager.GetAllComponents(entity).Length == 1);
        }

        [TestMethod]
        public void CreateEntity_Null() => Assert.ThrowsException<ArgumentNullException>(() =>
                                             Context.EntityManager.CreateEntity(null));

        [TestMethod]
        public void CreateEntity_NoComponents() => Assert.ThrowsException<EntityBlueprintNoComponentsException>(() =>
                                                     Context.EntityManager.CreateEntity(new EntityBlueprint()));

        [TestMethod]
        public void CreateEntity_DuplicateUniqueComponent()
        {
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestUniqueComponent1())));
        }

        [TestMethod]
        public void CreateEntity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntity_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.EntityManager.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityManager.EntityCount() == entities.Length);
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
            var entity = Context.EntityManager.CreateEntity(blueprint);
            Context.EntityManager.DestroyEntity(entity);
            entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
        }

        [TestMethod]
        public void CreateEntity_Reuse_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, blueprint);
            Context.EntityManager.DestroyEntities(entities);
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.EntityManager.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityManager.EntityCount() == entities.Length);
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
            var entities = Context.EntityManager.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));

            Assert.IsTrue(Context.EntityManager.EntityCount() == 2);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[0].Version == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[1].Version == 1);
            Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entities[0]).Prop == 1);
            Assert.IsTrue(Context.EntityManager.GetAllComponents(entities[0]).Length == 1);
            Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entities[1]).Prop == 1);
            Assert.IsTrue(Context.EntityManager.GetAllComponents(entities[1]).Length == 1);
        }

        [TestMethod]
        public void CreateEntities_DuplicateUniqueComponent() => Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                                                                   Context.EntityManager.CreateEntities(2, new EntityBlueprint()
                                                                       .AddComponent(new TestUniqueComponent1())));

        [TestMethod]
        public void CreateEntities_NegativeCount() => Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                                                        Context.EntityManager.CreateEntities(-1, new EntityBlueprint()
                                                            .AddComponent(new TestComponent1())));

        [TestMethod]
        public void CreateEntities_NoComponents() => Assert.ThrowsException<EntityBlueprintNoComponentsException>(() =>
                                                       Context.EntityManager.CreateEntities(1, new EntityBlueprint()));

        [TestMethod]
        public void CreateEntities_Null() => Assert.ThrowsException<ArgumentNullException>(() =>
                                               Context.EntityManager.CreateEntities(1, null));

        [TestMethod]
        public void CreateEntities_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntities_Reuse()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.EntityManager.CreateEntities(2, blueprint);
            Context.EntityManager.DestroyEntities(entities);
            entities = Context.EntityManager.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount() == 2);
            Assert.IsTrue(entities[0].Version == 2);
            Assert.IsTrue(entities[1].Version == 2);
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.EntityManager.DestroyEntity(entity);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsFalse(Context.EntityManager.HasEntity(entity));
            Assert.IsTrue(Context.EntityManager.HasEntity(keepEntity));
        }

        [TestMethod]
        public void DestroyEntity_UniqueComponent()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            Context.EntityManager.DestroyEntity(entity);

            Assert.IsFalse(Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void DestroyEntity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntity_Never() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                               Context.EntityManager.DestroyEntity(Entity.Null));

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent2()))[0];
            Context.EntityManager.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(Context.EntityManager.HasEntity(keepEntity));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.EntityManager.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_UniqueComponent()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            Context.EntityManager.DestroyEntities(new Entity[] { entity });

            Assert.IsFalse(Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void DestroyEntities_Null()
        {
            Entity[] entities = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.DestroyEntities(entities));
        }

        [TestMethod]
        public void DestroyEntities_Never_All()
        {
            var entities = new Entity[] { Entity.Null, Entity.Null };
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.EntityManager.DestroyEntities(entities));
        }

        [TestMethod]
        public void DestroyEntities_Never_Some()
        {
            var entities = new Entity[]
            {
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())),
                Entity.Null
            };
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.EntityManager.DestroyEntities(entities));
        }

        [TestMethod]
        public void DestroyEntities_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.DestroyEntities(new Entity[0]));
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            var entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent2()))[0];
            Context.EntityManager.DestroyEntities(archeType);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(Context.EntityManager.HasEntity(keepEntity));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.EntityManager.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.DestroyEntities(archeType));
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.DestroyEntities(archeType));
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType_UniqueComponent()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>();
            Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            Context.EntityManager.DestroyEntities(archeType);

            Assert.IsFalse(Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery()
        {
            var entityQuery = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            var entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var keepEntity = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent2()))[0];
            Context.EntityManager.DestroyEntities(entityQuery);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(Context.EntityManager.HasEntity(keepEntity));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.EntityManager.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery_Null()
        {
            EntityQuery entityQuery = null;
            ;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.DestroyEntities(entityQuery));
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery_Destroyed()
        {
            var entityQuery = new EntityQuery();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.DestroyEntities(entityQuery));
        }

        [TestMethod]
        public void TransferEntity()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntity = context2.EntityManager.TransferEntity(Context, entity, false);

            Assert.IsTrue(Context.EntityManager.HasEntity(entity));
            Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntity));
            Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntity).Prop == 1);

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntity_DestroyAfterTransfer()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));
            var keepEntity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntity = context2.EntityManager.TransferEntity(Context, entity, true);

            Assert.IsFalse(Context.EntityManager.HasEntity(entity));
            Assert.IsTrue(Context.EntityManager.HasEntity(keepEntity));
            Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntity));
            Assert.IsFalse(context2.EntityManager.HasEntity(keepEntity));
            Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntity).Prop == 1);

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntity_Destroyed()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));
            var context2 = EcsContext.CreateContext("TransferTest");
            var context3 = EcsContext.CreateContext("TransferTestDestroyed");
            EcsContext.DestroyContext(context3);

            // Destination destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context3.EntityManager.TransferEntity(Context, entity, true));

            EcsContext.DestroyContext(Context);

            // Source destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context2.EntityManager.TransferEntity(Context, entity, true));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntity_SameEcsContext()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.ThrowsException<EntityTransferSameEcsContextException>(() =>
                Context.EntityManager.TransferEntity(Context, entity, false));
        }

        [TestMethod]
        public void TransferEntity_NoEntity()
        {
            var context2 = EcsContext.CreateContext("TransferTest");

            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.EntityManager.TransferEntity(context2, Entity.Null, false));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntity_Null() => Assert.ThrowsException<ArgumentNullException>(() =>
                                               Context.EntityManager.TransferEntity(null, Entity.Null, false));

        [TestMethod]
        public void TransferEntities()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var entities = new Entity[]
            {
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(component1)
                    .AddComponent(new TestUniqueComponent1 { Prop = 2 })),
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(component1))
            };
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntities = context2.EntityManager.TransferEntities(Context, entities, false);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 2);
            Assert.IsTrue(context2.EntityManager.EntityCount() == 2);
            for (var i = 0; i < transferedEntities.Length; i++)
            {
                Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
                Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntities[i]).Prop == 1,
                    $"Enity.Id {transferedEntities[i].Id}");
            }
            Assert.IsTrue(context2.EntityManager.GetComponent<TestUniqueComponent1>(transferedEntities[0]).Prop == 2,
                $"Enity.Id {transferedEntities[0].Id}");

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_DestroyAfterTransfer()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entities = TestCreateEntities(Context, 2,
                component);
            var keepEntity = TestCreateEntities(Context, 1,
                component)[0];
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntities = context2.EntityManager.TransferEntities(Context, entities, true);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(context2.EntityManager.EntityCount() == 2);
            Assert.IsFalse(context2.EntityManager.HasEntity(keepEntity));
            for (var i = 0; i < transferedEntities.Length; i++)
            {
                Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
                Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntities[i]).Prop == 1,
                    $"Enity.Id {transferedEntities[i].Id}");
            }

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_Destroyed()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entities = TestCreateEntities(Context, 1,
                component);
            var context2 = EcsContext.CreateContext("TransferTest");
            var context3 = EcsContext.CreateContext("TransferTestDestroyed");
            EcsContext.DestroyContext(context3);

            // Destination destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context3.EntityManager.TransferEntities(Context, entities, true));

            EcsContext.DestroyContext(Context);

            // Source destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context2.EntityManager.TransferEntities(Context, entities, true));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_DuplicateUniqueComponent()
        {
            var entities = new Entity[]
            {
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestUniqueComponent1()))
            };
            var context2 = EcsContext.CreateContext("TransferTest");
            context2.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                context2.EntityManager.TransferEntities(Context, entities, false));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_SameEcsContext()
        {
            var entities = Context.EntityManager.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.ThrowsException<EntityTransferSameEcsContextException>(() =>
                Context.EntityManager.TransferEntities(Context, entities, false));
        }

        [TestMethod]
        public void TransferEntities_NoEntity_All()
        {
            var context2 = EcsContext.CreateContext("TransferTest");

            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.EntityManager.TransferEntities(context2, new[] { Entity.Null, Entity.Null }, false));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_NoEntity_Some()
        {
            var context2 = EcsContext.CreateContext("TransferTest");
            var entity = context2.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.EntityManager.TransferEntities(context2, new[] { entity, Entity.Null }, false));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_Null()
        {
            Entity[] entities = null;
            EcsContext context2 = null;
            var context3 = EcsContext.CreateContext("TransferTest");

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.TransferEntities(context2, new[] { Entity.Null }, false));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.TransferEntities(context3, entities, false));

            EcsContext.DestroyContext(context3);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType()
        {
            var component = new TestComponent1 { Prop = 1 };
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component)
                .AddComponent(new TestUniqueComponent1 { Prop = 2 }));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>();
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntities = context2.EntityManager.TransferEntities(Context, archeType, false);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(context2.EntityManager.EntityCount() == 1);
            for (var i = 0; i < transferedEntities.Length; i++)
            {
                Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
                Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntities[i]).Prop == 1,
                    $"Enity.Id {transferedEntities[i].Id}");
            }
            Assert.IsTrue(context2.EntityManager.GetComponent<TestUniqueComponent1>(transferedEntities[0]).Prop == 2,
                $"Enity.Id {transferedEntities[0].Id}");

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType_DestroyAfterTransfer()
        {
            var component = new TestComponent1 { Prop = 1 };
            TestCreateEntities(Context, 2,
                component);
            var keepEntity = TestCreateEntities(Context, 1,
                new TestComponent2())[0];
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntities = context2.EntityManager.TransferEntities(Context, archeType, true);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(context2.EntityManager.EntityCount() == 2);
            Assert.IsFalse(context2.EntityManager.HasEntity(keepEntity));
            for (var i = 0; i < transferedEntities.Length; i++)
            {
                Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
                Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntities[i]).Prop == 1,
                    $"Enity.Id {transferedEntities[i].Id}");
            }

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType_Destroyed()
        {
            var component = new TestComponent1 { Prop = 1 };
            TestCreateEntities(Context, 1,
                component);
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            var context2 = EcsContext.CreateContext("TransferTest");
            var context3 = EcsContext.CreateContext("TransferTestDestroyed");
            EcsContext.DestroyContext(context3);

            // Destination destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context3.EntityManager.TransferEntities(Context, archeType, true));

            EcsContext.DestroyContext(Context);

            // Source destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context2.EntityManager.TransferEntities(Context, archeType, true));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType_DuplicateUniqueComponent()
        {
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestUniqueComponent1()));
            var context2 = EcsContext.CreateContext("TransferTest");
            context2.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>();

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                context2.EntityManager.TransferEntities(Context, archeType, false));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType_SameEcsContext()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.ThrowsException<EntityTransferSameEcsContextException>(() =>
                Context.EntityManager.TransferEntities(Context, archeType, false));
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;
            EcsContext context2 = null;
            var context3 = EcsContext.CreateContext("TransferTest");

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.TransferEntities(context2, new EntityArcheType(), false));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.TransferEntities(context3, archeType, false));

            EcsContext.DestroyContext(context3);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery()
        {
            var component1 = new TestComponent1 { Prop = 1 };

            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(new TestUniqueComponent1 { Prop = 2 }));
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component1));
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntities = context2.EntityManager.TransferEntities(Context, query, false);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 2);
            Assert.IsTrue(context2.EntityManager.EntityCount() == 2);
            for (var i = 0; i < transferedEntities.Length; i++)
            {
                Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
                Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntities[i]).Prop == 1,
                    $"Enity.Id {transferedEntities[i].Id}");
            }
            Assert.IsTrue(context2.EntityManager.GetComponent<TestUniqueComponent1>(transferedEntities[1]).Prop == 2,
                $"Enity.Id {transferedEntities[1].Id}");

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery_DestroyAfterTransfer()
        {
            var component = new TestComponent1 { Prop = 1 };
            TestCreateEntities(Context, 2,
                component);
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            var keepEntity = TestCreateEntities(Context, 1,
                new TestComponent2())[0];
            var context2 = EcsContext.CreateContext("TransferTest");

            var transferedEntities = context2.EntityManager.TransferEntities(Context, query, true);

            Assert.IsTrue(Context.EntityManager.EntityCount() == 1);
            Assert.IsTrue(context2.EntityManager.EntityCount() == 2);
            Assert.IsFalse(context2.EntityManager.HasEntity(keepEntity));
            for (var i = 0; i < transferedEntities.Length; i++)
            {
                Assert.IsTrue(context2.EntityManager.HasEntity(transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
                Assert.IsTrue(context2.EntityManager.GetComponent<TestComponent1>(transferedEntities[i]).Prop == 1,
                    $"Enity.Id {transferedEntities[i].Id}");
            }

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery_Destroyed()
        {
            var component = new TestComponent1 { Prop = 1 };
            TestCreateEntities(Context, 1,
                component);
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            var context2 = EcsContext.CreateContext("TransferTest");
            var context3 = EcsContext.CreateContext("TransferTestDestroyed");
            EcsContext.DestroyContext(context3);

            // Destination destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context3.EntityManager.TransferEntities(Context, query, true));

            EcsContext.DestroyContext(Context);

            // Source destroyed
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                context2.EntityManager.TransferEntities(Context, query, true));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery_DuplicateUniqueComponent()
        {
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));
            var query = new EntityQuery()
                .WhereAllOf<TestUniqueComponent1>();
            var context2 = EcsContext.CreateContext("TransferTest");
            context2.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1()));

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                context2.EntityManager.TransferEntities(Context, query, false));

            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery_SameEcsContext()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestUniqueComponent1>();

            Assert.ThrowsException<EntityTransferSameEcsContextException>(() =>
                Context.EntityManager.TransferEntities(Context, query, false));
        }

        [TestMethod]
        public void TransferEntities_EntityQuery_Null()
        {
            EntityQuery query = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.TransferEntities(Context, query, false));
        }

        [TestMethod]
        public void HasEntity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasEntity(Entity.Null));
        }

        [TestMethod]
        public void HasEntity()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_EntityArcheType()
        {
            var entity1 = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var entity2 = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            Assert.IsTrue(Context.EntityManager.HasEntity(entity1, archeType));
            Assert.IsFalse(Context.EntityManager.HasEntity(entity2, archeType));
            Assert.IsFalse(Context.EntityManager.HasEntity(Entity.Null, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityArcheType_Null()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.HasEntity(entity, archeType));
        }

        [TestMethod]
        public void HasEntity_EntityArcheType_Destroyed()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasEntity(entity, archeType));
        }

        [TestMethod]
        public void GetEntities()
        {
            var entities = Context.EntityManager.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var getEntities = Context.EntityManager.GetEntities();

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_Destroyed()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetEntities());
        }

        [TestMethod]
        public void GetEntities_EntityArcheType()
        {
            var entities = Context.EntityManager.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            var getEntities = Context.EntityManager.GetEntities(archeType);

            Assert.IsTrue(getEntities.Length == 2);
            Assert.IsTrue(getEntities[0] == entities[0]);
            Assert.IsTrue(getEntities[1] == entities[1]);
        }

        [TestMethod]
        public void GetEntities_EntityArcheType_Null()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent2()));
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.GetEntities(archeType));
        }

        [TestMethod]
        public void GetEntities_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetEntities(archeType));
        }

        [TestMethod]
        public void GetAllComponents_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetAllComponents(Entity.Null));
        }

        [TestMethod]
        public void GetAllComponents_NoEntity() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                     Context.EntityManager.GetAllComponents(Entity.Null));

        [TestMethod]
        public void GetAllComponents_Normal() => AssertGetAllComponents(1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_Normal_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalShared() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalShared_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalSharedUnique() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalUnique() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalUniqueShared() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_Shared() => AssertGetAllComponents(1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_Shared_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedNormal() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedNormal_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedNormalUnique() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedUnique() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedUniqueNormal() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_Unique() => AssertGetAllComponents(1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_UniqueNormal() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_UniqueNormalShared() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testComponent1_1,
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_UniqueShared() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_UniqueSharedNormal() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetComponent_NoEntity() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                 Context.EntityManager.GetComponent<TestComponent1>(Entity.Null));

        [TestMethod]
        public void GetComponent_Normal() => AssertGetComponent(1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Normal_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.GetComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent_NormalShared() => AssertGetComponent(1,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_NormalShared_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_NormalSharedUnique() => AssertGetComponent(1,
                _testComponent1_1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_NormalUnique() => AssertGetComponent(1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_NormalUniqueShared() => AssertGetComponent(1,
                _testComponent1_1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_Shared() => AssertGetComponent(1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_Shared_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testUniqueComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.GetComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent_SharedNormal() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_SharedNormal_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_SharedNormalUnique() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_SharedUnique() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_SharedUniqueNormal() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Unique() => AssertGetComponent(1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.GetComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent_UniqueNormal() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_UniqueNormalShared() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_UniqueShared() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_UniqueSharedNormal() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponents_EntityArcheType()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = i + 1 }));
            }
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            var components = Context.EntityManager.GetComponents<TestComponent1>(archeType);

            Assert.IsTrue(components.Length == UnitTestConsts.SmallCount);
            for (var i = 0; i < components.Length; i++)
            {
                Assert.IsTrue(components[i].Prop == i + 1,
                    $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.GetComponents<TestComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetComponents<TestComponent1>(archeType));
        }

        [TestMethod]
        public void GetComponents_EntityQuery()
        {
            var component = new TestComponent1 { Prop = 1 };
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = i + 1 }));
            }
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            var components = Context.EntityManager.GetComponents<TestComponent1>(query);

            Assert.IsTrue(components.Length == UnitTestConsts.SmallCount);
            for (var i = 0; i < components.Length; i++)
            {
                Assert.IsTrue(components[i].Prop == i + 1,
                    $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void GetComponents_EntityQuery_Null()
        {
            EntityQuery query = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.GetComponents<TestComponent1>(query));
        }

        [TestMethod]
        public void GetComponents_EntityQuery_Destroyed()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetComponents<TestComponent1>(archeType));
        }

        [TestMethod]
        public void HasComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void HasComponent_Normal_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.IsTrue(Context.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.IsFalse(Context.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.IsTrue(Context.EntityManager.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testUniqueComponent1_1));

            Assert.IsFalse(Context.EntityManager.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testUniqueComponent1_1));

            Assert.IsTrue(Context.EntityManager.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.IsFalse(Context.EntityManager.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void Unique_GetUniqueComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.EntityManager.GetUniqueComponent<TestUniqueComponent1>().Prop == component.Prop);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueEntity()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.EntityManager.GetUniqueEntity<TestUniqueComponent1>() == entity);
        }

        [TestMethod]
        public void Unique_HasUniqueComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_HasUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void UpdateComponent_Entity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.UpdateComponent(Entity.Null, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_NoEntity() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                           Context.EntityManager.UpdateComponent(Entity.Null, _testComponent1_1));

        [TestMethod]
        public void UpdateComponent_Entity_Normal() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Normal_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(entity, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_NormalShared() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalShared_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalSharedUnique() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalUnique() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalUniqueShared() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Shared() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Shared_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(entity, _testSharedComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_SharedNormal() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedNormal_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedNormalUnique() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedUnique() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedUniqueNormal() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Unique() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(entity, _testUniqueComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_UniqueNormal() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_UniqueNormalShared() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_UniqueShared() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_UniqueSharedNormal() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.UpdateComponent(new[] { Entity.Null }, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_Null()
        {
            Entity[] entities = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.UpdateComponent(entities, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_NoEntity() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                             Context.EntityManager.UpdateComponent(new[] { Entity.Null }, _testComponent1_1));

        [TestMethod]
        public void UpdateComponent_Entities_Normal() => AssertUpdateComponent_Entities(UnitTestConsts.SmallCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(new[] { entity }, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_NormalShared() => AssertUpdateComponent_Entities(UnitTestConsts.SmallCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalSharedUnique() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalUnique() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalUniqueShared() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Shared() => AssertUpdateComponent_Entities(UnitTestConsts.SmallCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(new[] { entity }, _testSharedComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_SharedNormal() => AssertUpdateComponent_Entities(UnitTestConsts.SmallCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedNormalUnique() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedUnique() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedUniqueNormal() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Unique() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(new[] { entity }, _testUniqueComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_UniqueNormal() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_UniqueNormalShared() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_UniqueShared() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_UniqueSharedNormal() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Destroyed()
        {
            var query = new EntityQuery();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.UpdateComponent(query, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery_Null()
        {
            EntityQuery query = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.UpdateComponent(query, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery_Normal() => AssertUpdateComponent_EntityQuery(UnitTestConsts.SmallCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalShared() => AssertUpdateComponent_EntityQuery(UnitTestConsts.SmallCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalSharedUnique() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalUnique() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalUniqueShared() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Shared() => AssertUpdateComponent_EntityQuery(UnitTestConsts.SmallCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormal() => AssertUpdateComponent_EntityQuery(UnitTestConsts.SmallCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormalUnique() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedUnique() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedUniqueNormal() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Unique() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueNormal() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueNormalShared() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueShared() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueSharedNormal() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Destroyed()
        {
            var archeType = new EntityArcheType();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.UpdateComponent(archeType, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Null()
        {
            EntityArcheType archeType = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.EntityManager.UpdateComponent(archeType, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Normal() => AssertUpdateComponent_EntityArcheType(UnitTestConsts.SmallCount,
            new EntityArcheType()
                .AddComponentType<TestComponent1>(),
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_NormalShared() => AssertUpdateComponent_EntityArcheType(UnitTestConsts.SmallCount,
            new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(_testSharedComponent1_1),
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_NormalUnique() => AssertUpdateComponent_EntityArcheType(1,
            new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>(),
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_NormalUniqueShared() => AssertUpdateComponent_EntityArcheType(1,
            new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>()
                .AddSharedComponent(_testSharedComponent1_1),
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Shared() => AssertUpdateComponent_EntityArcheType(UnitTestConsts.SmallCount,
            new EntityArcheType()
                .AddSharedComponent(_testSharedComponent1_1),
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_Unique() => AssertUpdateComponent_EntityArcheType(1,
            new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>(),
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_UniqueNormal() => AssertUpdateComponent_EntityArcheType(1,
            new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>()
                .AddComponentType<TestComponent1>(),
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_UniqueNormalShared() => AssertUpdateComponent_EntityArcheType(1,
            new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(_testSharedComponent1_1),
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityArcheType_UniqueShared() => AssertUpdateComponent_EntityArcheType(1,
            new EntityArcheType()
                .AddComponentType<TestUniqueComponent1>()
                .AddSharedComponent(_testSharedComponent1_1),
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        private void AssertGetAllComponents<T1>(int entityCount, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var components = Context.EntityManager.GetAllComponents(entity);
                Assert.IsTrue(components.Length == 1,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(components[0] is T1,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(((T1)components[0]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetAllComponents<T1, T2>(int entityCount, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            var entitiesIsOk = new int[entityCount][];
            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = new int[2];
                var entity = entities[i];
                var components = Context.EntityManager.GetAllComponents(entity);
                Assert.IsTrue(components.Length == 2);
                for (var j = 0; j < components.Length; j++)
                {
                    if (components[j] is T1)
                        isOk[0] += ((T1)components[j]).Prop == component1.Prop ? 1 : 0;
                    if (components[j] is T2)
                        isOk[1] += ((T2)components[j]).Prop == component2.Prop ? 1 : 0;
                }

                entitiesIsOk[i] = isOk;
            }

            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = entitiesIsOk[i];
                for (var j = 0; j < isOk.Length; j++)
                {
                    Assert.IsTrue(isOk[j] == 1,
                        $"Enity.Id {entities[i].Id}");
                }
            }
        }

        private void AssertGetAllComponents<T1, T2, T3>(int entityCount, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            var entitiesIsOk = new int[entityCount][];
            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = new int[3];
                var entity = entities[i];
                var components = Context.EntityManager.GetAllComponents(entity);
                Assert.IsTrue(components.Length == 3);
                for (var j = 0; j < components.Length; j++)
                {
                    if (components[j] is T1)
                        isOk[0] += ((T1)components[j]).Prop == component1.Prop ? 1 : 0;
                    if (components[j] is T2)
                        isOk[1] += ((T2)components[j]).Prop == component2.Prop ? 1 : 0;
                    if (components[j] is T3)
                        isOk[2] += ((T3)components[j]).Prop == component3.Prop ? 1 : 0;
                }

                entitiesIsOk[i] = isOk;
            }

            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = entitiesIsOk[i];
                for (var j = 0; j < isOk.Length; j++)
                {
                    Assert.IsTrue(isOk[j] == 1,
                        $"Enity.Id {entities[i].Id}");
                }
            }
        }

        private void AssertGetComponent<T1>(int entityCount,
            T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetComponent<T1, T2>(int entityCount,
            T1 component1,
            T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetComponent<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == component3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entity<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entity<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent2);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entity<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent2);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent3);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entities<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            Context.EntityManager.UpdateComponent(entities, updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entities<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            Context.EntityManager.UpdateComponent(entities, updateComponent1);
            Context.EntityManager.UpdateComponent(entities, updateComponent2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entities<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            Context.EntityManager.UpdateComponent(entities, updateComponent1);
            Context.EntityManager.UpdateComponent(entities, updateComponent2);
            Context.EntityManager.UpdateComponent(entities, updateComponent3);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);
            var query = new EntityQuery()
                .WhereAllOf<T1>();

            Context.EntityManager.UpdateComponent(query, updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);
            var query = new EntityQuery()
                .WhereAllOf<T1, T2>();

            Context.EntityManager.UpdateComponent(query, updateComponent1);
            Context.EntityManager.UpdateComponent(query, updateComponent2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);
            var query = new EntityQuery()
                .WhereAllOf<T1, T2, T3>();

            Context.EntityManager.UpdateComponent(query, updateComponent1);
            Context.EntityManager.UpdateComponent(query, updateComponent2);
            Context.EntityManager.UpdateComponent(query, updateComponent3);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent2);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent3);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityArcheType<T1>(int entityCount,
            EntityArcheType archeType,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            Context.EntityManager.UpdateComponent(archeType, updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityArcheType<T1, T2>(int entityCount,
            EntityArcheType archeType,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            Context.EntityManager.UpdateComponent(archeType, updateComponent1);
            Context.EntityManager.UpdateComponent(archeType, updateComponent2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityArcheType<T1, T2, T3>(int entityCount,
            EntityArcheType archeType,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            Context.EntityManager.UpdateComponent(archeType, updateComponent1);
            Context.EntityManager.UpdateComponent(archeType, updateComponent2);
            Context.EntityManager.UpdateComponent(archeType, updateComponent3);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }
    }
}
