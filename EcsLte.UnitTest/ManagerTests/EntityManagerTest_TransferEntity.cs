using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest_TransferEntity2 : BasePrePostTest
    {
        #region Entity

        [TestMethod]
        public void TransferEntity_NoEntity() => AssertTransfer_EntityDoesNotExist(
                (src, dest) => dest.TransferEntity(src, Entity.Null, false));

        [TestMethod]
        public void TransferEntity_Null_Source() => AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntity(null, Entity.Null, false));

        [TestMethod]
        public void TransferEntity_SameEcsContext() => AssertTransfer_SameEcsContext(
                (src, dest) => dest.TransferEntity(dest, Entity.Null, false));

        [TestMethod]
        public void TransferEntity()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 1 })),
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 2 }))
            };

            AssertTransfer(2, 1,
                (src, dest) => new[] { dest.TransferEntity(src, entities[0], false) },
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntity_DestroyAfter()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 1 })),
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 2 }))
            };

            AssertTransfer(1, 1,
                (src, dest) => new[] { dest.TransferEntity(src, entities[0], true) },
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntity_Unique()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestUniqueComponent1 { Prop = 1 }))
            };

            AssertTransfer(1, 1,
                (src, dest) => new[] { dest.TransferEntity(src, entities[0], false) },
                (context, entity) => context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void TransferEntity_Unique_AlreadyHasUnique()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestUniqueComponent1 { Prop = 1 }))
            };

            AssertTransfer_AlreadyHasUnique<TestUniqueComponent1>(
                (src, dest) => dest.TransferEntity(src, entities[0], false));
        }

        [TestMethod]
        public void TransferEntity_Unique_DestroyAfter()
        {
            var entity1 = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            AssertTransfer(0, 1,
                (src, dest) => new[] { dest.TransferEntity(src, entity1, true) },
                (context, entity) => context.HasUniqueComponent<TestUniqueComponent1>());
        }

        #endregion Entity

        #region Entities

        [TestMethod]
        public void TransferEntities_NoEntity_All() => AssertTransfer_EntityDoesNotExist(
                (src, dest) => dest.TransferEntities(src, new[] { Entity.Null, Entity.Null }, false));

        [TestMethod]
        public void TransferEntities_NoEntity_Some()
        {
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            AssertTransfer_EntityDoesNotExist(
                (src, dest) => dest.TransferEntities(src, new[] { entity, Entity.Null }, false));
        }

        [TestMethod]
        public void TransferEntities_Null_Entities()
        {
            Entity[] entities = null;
            AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntities(src, entities, false));
        }

        [TestMethod]
        public void TransferEntities_Null_Source() => AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntities(null, new[] { Entity.Null, Entity.Null }, false));

        [TestMethod]
        public void TransferEntities_SameEcsContext() => AssertTransfer_SameEcsContext(
                (src, dest) => dest.TransferEntities(dest, new[] { Entity.Null, Entity.Null }, false));

        [TestMethod]
        public void TransferEntities()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                blueprint = blueprint.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                entities[i] = Context.CreateEntity(blueprint);
            }

            AssertTransfer(UnitTestConsts.SmallCount, UnitTestConsts.SmallCount,
                (src, dest) => dest.TransferEntities(src, entities, false),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntities_DestroyAfter()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                blueprint = blueprint.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                entities[i] = Context.CreateEntity(blueprint);
            }

            AssertTransfer(0, UnitTestConsts.SmallCount,
                (src, dest) => dest.TransferEntities(src, entities, true),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntities_Unique()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 1 })),
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 2 })
                    .AddComponent(new TestUniqueComponent1 { Prop = 1 }))
            };

            AssertTransfer(2, 2,
                (src, dest) => dest.TransferEntities(src, entities, false),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id &&
                    context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void TransferEntities_Unique_AlreadyHasUnique()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 1 })),
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 1 })
                    .AddComponent(new TestUniqueComponent1 { Prop = 1 }))
            };

            AssertTransfer_AlreadyHasUnique<TestUniqueComponent1>(
                (src, dest) => dest.TransferEntities(src, entities, false));
        }

        [TestMethod]
        public void TransferEntities_Unique_DestroyAfter()
        {
            var entities = new Entity[]
            {
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 1 })),
                Context.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1 { Prop = 2 })
                    .AddComponent(new TestUniqueComponent1 { Prop = 1 }))
            };

            AssertTransfer(0, 2,
                (src, dest) => dest.TransferEntities(src, entities, true),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id &&
                    context.HasUniqueComponent<TestUniqueComponent1>());
            Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        #endregion Entities

        #region EntityArcheType

        [TestMethod]
        public void TransferEntityArcheType_Null_EntityArcheType()
        {
            EntityArcheType archeType = null;
            AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntities(src, archeType, false));
        }

        [TestMethod]
        public void TransferEntityArcheType_Null_Source()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntities(null, archeType, false));
        }

        [TestMethod]
        public void TransferEntityArcheType_SameEcsContext()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            AssertTransfer_SameEcsContext(
                (src, dest) => dest.TransferEntities(dest, archeType, false));
        }

        [TestMethod]
        public void TransferEntityArcheType()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                blueprint = blueprint.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                entities[i] = Context.CreateEntity(blueprint);
            }

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            AssertTransfer(UnitTestConsts.SmallCount, UnitTestConsts.SmallCount,
                (src, dest) => dest.TransferEntities(src, archeType, false),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntityArcheType_DestroyAfter()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                blueprint = blueprint.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                entities[i] = Context.CreateEntity(blueprint);
            }

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            AssertTransfer(0, UnitTestConsts.SmallCount,
                (src, dest) => dest.TransferEntities(src, archeType, true),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntityArcheType_Unique()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>();
            AssertTransfer(1, 1,
                (src, dest) => dest.TransferEntities(src, archeType, false),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id &&
                    context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void TransferEntityArcheType_Unique_AlreadyHasUnique()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>();
            AssertTransfer_AlreadyHasUnique<TestUniqueComponent1>(
                (src, dest) => dest.TransferEntities(src, archeType, false));
        }

        [TestMethod]
        public void TransferEntityArcheType_Unique_DestroyAfter()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestUniqueComponent1>();
            AssertTransfer(0, 1,
                (src, dest) => dest.TransferEntities(src, archeType, true),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id &&
                    context.HasUniqueComponent<TestUniqueComponent1>());
            Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        #endregion EntityArcheType

        #region EntityQuery

        [TestMethod]
        public void TransferEntityQuery_Null_EntityQuery()
        {
            EntityQuery query = null;
            AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntities(src, query, false));
        }

        [TestMethod]
        public void TransferEntityQuery_Null_Source()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer_ArgumentNull(
                (src, dest) => dest.TransferEntities(null, query, false));
        }

        [TestMethod]
        public void TransferEntityQuery_SameEcsContext()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer_SameEcsContext(
                (src, dest) => dest.TransferEntities(dest, query, false));
        }

        [TestMethod]
        public void TransferEntityQuery()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                blueprint = blueprint.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                entities[i] = Context.CreateEntity(blueprint);
            }

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer(UnitTestConsts.SmallCount, UnitTestConsts.SmallCount,
                (src, dest) => dest.TransferEntities(src, query, false),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntityQuery_DestroyAfter()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.SmallCount];
            for (var i = 0; i < entities.Length; i++)
            {
                blueprint = blueprint.UpdateComponent(new TestComponent1 { Prop = i + 1 });
                entities[i] = Context.CreateEntity(blueprint);
            }

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer(0, UnitTestConsts.SmallCount,
                (src, dest) => dest.TransferEntities(src, query, true),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id);
        }

        [TestMethod]
        public void TransferEntityQuery_Unique()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 2 })
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer(2, 2,
                (src, dest) => dest.TransferEntities(src, query, false),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id &&
                    context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void TransferEntityQuery_Unique_AlreadyHasUnique()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 2 })
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer_AlreadyHasUnique<TestUniqueComponent1>(
                (src, dest) => dest.TransferEntities(src, query, false));
        }

        [TestMethod]
        public void TransferEntityQuery_Unique_DestroyAfter()
        {
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 }));
            Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 2 })
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            AssertTransfer(0, 2,
                (src, dest) => dest.TransferEntities(src, query, true),
                (context, entity) => context.GetComponent<TestComponent1>(entity).Prop == entity.Id &&
                    context.HasUniqueComponent<TestUniqueComponent1>());
            Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        #endregion EntityQuery

        private void AssertTransfer_EntityDoesNotExist(Action<EcsContext, EcsContext> assertAction)
        {
            var destContext = EcsContexts.CreateContext("TransferTest");

            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                assertAction.Invoke(Context, destContext));
        }

        private void AssertTransfer_ArgumentNull(Action<EcsContext, EcsContext> assertAction)
        {
            var destContext = EcsContexts.CreateContext("TransferTest");

            Assert.ThrowsException<ArgumentNullException>(() =>
                assertAction.Invoke(Context, destContext));
        }

        private void AssertTransfer_SameEcsContext(Action<EcsContext, EcsContext> assertAction)
        {
            var destContext = EcsContexts.CreateContext("TransferTest");

            Assert.ThrowsException<EntityTransferSameEcsContextException>(() =>
                assertAction.Invoke(Context, destContext));
        }

        private void AssertTransfer_AlreadyHasUnique<TUniqueComponent>(
            Action<EcsContext, EcsContext> assertAction)
            where TUniqueComponent : unmanaged, IUniqueComponent
        {
            var destContext = EcsContexts.CreateContext("TransferTest");

            destContext.CreateEntity(new EntityBlueprint().AddComponent(new TUniqueComponent()));
            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                assertAction.Invoke(Context, destContext));
        }

        private void AssertTransfer(int keepCount, int transferCount,
            Func<EcsContext, EcsContext, Entity[]> assertTransferAction,
            Func<EcsContext, Entity, bool> assertTransferedEntityAction)
        {
            var destContext = EcsContexts.CreateContext("TransferTest");
            var transferedEntities = assertTransferAction.Invoke(Context, destContext);

            Assert.IsTrue(Context.EntityCount() == keepCount);
            Assert.IsTrue(destContext.EntityCount() == transferCount);
            Assert.IsTrue(transferedEntities.Length == transferCount);
            for (var i = 0; i < transferCount; i++)
            {
                Assert.IsTrue(assertTransferedEntityAction.Invoke(destContext, transferedEntities[i]),
                    $"Enity.Id {transferedEntities[i].Id}");
            }
        }
    }
}