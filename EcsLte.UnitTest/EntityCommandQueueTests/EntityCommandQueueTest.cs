using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandQueueTests
{
    [TestClass]
    public class EntityCommandQueueTest : BasePrePostTest
    {
        [TestMethod]
        public void CreateEntity()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            queue.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
        }

        [TestMethod]
        public void CreateEntity_Large()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            for (var i = 0; i < UnitTestConsts.LargeCount; i++)
                queue.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == UnitTestConsts.LargeCount);
        }

        [TestMethod]
        public void CreateEntities()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            queue.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
        }

        [TestMethod]
        public void CreateEntities_Large()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            queue.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == UnitTestConsts.LargeCount);
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            queue.DestroyEntity(Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1())));

            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
        }

        [TestMethod]
        public void DestroyEntity_Large()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            for (var i = 0; i < UnitTestConsts.LargeCount; i++)
                queue.DestroyEntity(Context.EntityManager.CreateEntity(blueprint));

            Assert.IsTrue(Context.EntityManager.EntityCount == UnitTestConsts.LargeCount);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            queue.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
        }

        [TestMethod]
        public void DestroyEntities_Large()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            queue.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityManager.EntityCount == UnitTestConsts.LargeCount);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
        }

        [TestMethod]
        public void UpdateComponent_Entity()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var entity = TestCreateEntities(Context, 1, component)[0];

            queue.UpdateComponent(entity, updatedComponent);
            Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == component.Prop);
            queue.ExecuteCommands();
            Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == updatedComponent.Prop);
        }

        [TestMethod]
        public void UpdateComponent_Entities()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var entities = TestCreateEntities(Context, UnitTestConsts.LargeCount, component);

            queue.UpdateComponent(entities, updatedComponent);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entities[i]).Prop == component.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
            queue.ExecuteCommands();
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entities[i]).Prop == updatedComponent.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void UpdateComponent_EntityQuery()
        {
            var component = new TestComponent1 { Prop = 1 };
            var updatedComponent = new TestComponent1 { Prop = 10 };
            var queue = Context.CommandManager.CreateCommandQueue("Test");
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();
            TestCreateEntities(Context, UnitTestConsts.LargeCount, component);
            var entities = query.GetEntities();

            queue.UpdateComponent(query, updatedComponent);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entities[i]).Prop == component.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
            queue.ExecuteCommands();
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entities[i]).Prop == updatedComponent.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }
    }
}
