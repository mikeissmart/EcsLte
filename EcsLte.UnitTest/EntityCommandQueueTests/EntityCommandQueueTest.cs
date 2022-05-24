using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandQueueTests
{
    [TestClass]
    public class EntityCommandQueueTest : BasePrePostTest
    {
        [TestMethod]
        public void CreateEntity()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            queue.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount() == 0);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 1);
        }

        [TestMethod]
        public void CreateEntities()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            queue.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityCount() == 0);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 2);
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            queue.DestroyEntity(Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1())));

            Assert.IsTrue(Context.EntityCount() == 1);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            queue.DestroyEntities(Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1())));

            Assert.IsTrue(Context.EntityCount() == 2);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
        }

        [TestMethod]
        public void DestroyEntities_EntityArcheType()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var entityArcheType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            queue.DestroyEntities(entityArcheType);

            Assert.IsTrue(Context.EntityCount() == 2);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
        }

        [TestMethod]
        public void DestroyEntities_EntityQuery()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var entityQuery = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            queue.DestroyEntities(entityQuery);

            Assert.IsTrue(Context.EntityCount() == 2);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
        }

        [TestMethod]
        public void TransferEntity()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            queue.TransferEntity(Context, entity, false);

            Assert.IsTrue(Context.EntityCount() == 1);
            queue.ExecuteCommands();
            Assert.IsTrue(destContext.EntityCount() == 1);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntity_DestroyEntity()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            var entity = Context.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            queue.TransferEntity(Context, entity, true);

            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
            Assert.IsTrue(destContext.EntityCount() == 1);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntities()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            queue.TransferEntities(Context, entities, false);

            Assert.IsTrue(Context.EntityCount() == 2);
            queue.ExecuteCommands();
            Assert.IsTrue(destContext.EntityCount() == 2);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntities_DestroyEntities()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            var entities = Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            queue.TransferEntities(Context, entities, true);

            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
            Assert.IsTrue(destContext.EntityCount() == 2);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            queue.TransferEntities(Context, archeType, false);

            Assert.IsTrue(Context.EntityCount() == 2);
            queue.ExecuteCommands();
            Assert.IsTrue(destContext.EntityCount() == 2);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntities_EntityArcheType_DestroyEntities()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            queue.TransferEntities(Context, archeType, true);

            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
            Assert.IsTrue(destContext.EntityCount() == 2);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            queue.TransferEntities(Context, query, false);

            Assert.IsTrue(Context.EntityCount() == 2);
            queue.ExecuteCommands();
            Assert.IsTrue(destContext.EntityCount() == 2);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void TransferEntities_EntityQuery_DestroyEntities()
        {
            var destContext = EcsContexts.CreateContext("TransferContext");
            var queue = destContext.Commands.CreateCommandQueue("Test");
            Context.CreateEntities(2, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            queue.TransferEntities(Context, query, true);

            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityCount() == 0);
            Assert.IsTrue(destContext.EntityCount() == 2);

            EcsContexts.DestroyContext(destContext);
        }

        [TestMethod]
        public void UpdateComponent_Entity()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.Commands.CreateCommandQueue("Test");
            var entity = TestCreateEntities(Context, 1, component)[0];

            queue.UpdateComponent(entity, updatedComponent);
            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == component.Prop);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == updatedComponent.Prop);
        }

        [TestMethod]
        public void UpdateComponent_Entities()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.Commands.CreateCommandQueue("Test");
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount, component);

            queue.UpdateComponent(entities, updatedComponent);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[i]).Prop == component.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
            queue.ExecuteCommands();
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[i]).Prop == updatedComponent.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void UpdateComponent_EntityArcheType()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.Commands.CreateCommandQueue("Test");
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount, component);

            queue.UpdateComponent(archeType, updatedComponent);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[i]).Prop == component.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
            queue.ExecuteCommands();
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[i]).Prop == updatedComponent.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.Commands.CreateCommandQueue("Test");
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            TestCreateEntities(Context, UnitTestConsts.SmallCount, component);
            var entities = Context.GetEntities(query);

            queue.UpdateComponent(query, updatedComponent);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[i]).Prop == component.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
            queue.ExecuteCommands();
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entities[i]).Prop == updatedComponent.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ExecuteCommands_Destroyed()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                queue.ExecuteCommands());
        }
    }
}