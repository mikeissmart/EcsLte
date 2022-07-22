using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_EntityCreate : BasePrePostTest
    {
        [TestMethod]
        public void CreateEntity_ArcheType()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });
            var orgArcheType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            commands.CreateEntity(archeType, EntityState.Active);
            archeType.AddComponentType<TestComponent2>();

            Assert.IsTrue(Context.Entities.EntityCount(archeType) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(orgArcheType) == 0);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(archeType) == 0,
                "ArcheType not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(orgArcheType) == 1);

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => commands.CreateEntity(x, EntityState.Active)
               });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CreateEntity(archeType, EntityState.Active));
        }

        [TestMethod]
        public void CreateEntity_Blueprint()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });
            var orgBlueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            commands.CreateEntity(blueprint, EntityState.Active);
            blueprint.SetComponent(new TestComponent2());

            Assert.IsTrue(Context.Entities.EntityCount(blueprint.GetArcheType()) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(orgBlueprint.GetArcheType()) == 0);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(blueprint.GetArcheType()) == 0,
                "Blueprint not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(orgBlueprint.GetArcheType()) == 1);

            AssertBlueprint_Invalid_Null(
               new Action<EntityBlueprint>[]
               {
                   x => commands.CreateEntity(x, EntityState.Active)
               });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CreateEntity(orgBlueprint, EntityState.Active));
        }

        [TestMethod]
        public void CreateEntities_ArcheType()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });
            var orgArcheType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            commands.CreateEntities(archeType, EntityState.Active, UnitTestConsts.SmallCount);
            archeType.AddComponentType<TestComponent2>();

            Assert.IsTrue(Context.Entities.EntityCount(archeType) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(orgArcheType) == 0);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(archeType) == 0,
                "ArcheType not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(orgArcheType) == UnitTestConsts.SmallCount);

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => commands.CreateEntities(x, EntityState.Active, UnitTestConsts.SmallCount)
               });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CreateEntities(archeType, EntityState.Active, UnitTestConsts.SmallCount));
        }

        [TestMethod]
        public void CreateEntities_Blueprint()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });
            var orgBlueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            commands.CreateEntities(blueprint, EntityState.Active, UnitTestConsts.SmallCount);
            blueprint.SetComponent(new TestComponent2());

            Assert.IsTrue(Context.Entities.EntityCount(blueprint.GetArcheType()) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(orgBlueprint.GetArcheType()) == 0);

            commands.ExecuteCommands();
            Assert.IsTrue(Context.Entities.EntityCount(blueprint.GetArcheType()) == 0,
                "Blueprint not snapshoted when creating command");
            Assert.IsTrue(Context.Entities.EntityCount(orgBlueprint.GetArcheType()) == UnitTestConsts.SmallCount);

            AssertBlueprint_Invalid_Null(
               new Action<EntityBlueprint>[]
               {
                   x => commands.CreateEntities(x, EntityState.Active, UnitTestConsts.SmallCount)
               });

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.CreateEntities(blueprint, EntityState.Active, UnitTestConsts.SmallCount));
        }

        private void AssertBlueprint_Invalid_Null(
            params Action<EntityBlueprint>[] assertActions)
        {
            var invalid = new EntityBlueprint();
            EntityBlueprint nullable = null;

            foreach (var action in assertActions)
            {
                Assert.ThrowsException<ComponentsNoneException>(() => action(invalid),
                    "Invalid");

                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");
            }
        }
    }
}
