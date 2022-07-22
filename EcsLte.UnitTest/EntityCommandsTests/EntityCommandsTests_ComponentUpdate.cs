using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_ComponentUpdate : BasePrePostTest
    {
        [TestMethod]
        public void UpdateComponent()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 }),
                EntityState.Active);

            commands.UpdateComponent(entity, new TestComponent1 { Prop = 2 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 2);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void UpdateComponents_T12()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2()));
        }

        [TestMethod]
        public void UpdateComponents_T123()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3()));
        }

        [TestMethod]
        public void UpdateComponents_T1234()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateComponents_T12345()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2()));
        }

        [TestMethod]
        public void UpdateComponents_T123456()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3()));
        }

        [TestMethod]
        public void UpdateComponents_T1234567()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 },
                new TestUniqueComponent1 { Prop = 7 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 7);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1()));
        }

        [TestMethod]
        public void UpdateComponents_T1234568()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = CreateTestEntity();

            commands.UpdateComponents(entity,
                new TestComponent1 { Prop = 1 },
                new TestComponent2 { Prop = 2 },
                new TestComponent3 { Prop = 3 },
                new TestSharedComponent1 { Prop = 4 },
                new TestSharedComponent2 { Prop = 5 },
                new TestSharedComponent3 { Prop = 6 },
                new TestUniqueComponent1 { Prop = 7 },
                new TestUniqueComponent2 { Prop = 8 });

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 0);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent2>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(entity).Prop == 1);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent2>(entity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetComponent<TestComponent3>(entity).Prop == 3);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 4);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent2>(entity).Prop == 5);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent3>(entity).Prop == 6);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 7);
            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent2>(entity).Prop == 8);

            Assert.ThrowsException<ComponentDuplicateException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1(),
                    new TestComponent1()));

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateComponents(entity,
                    new TestComponent1(),
                    new TestComponent2(),
                    new TestComponent3(),
                    new TestSharedComponent1(),
                    new TestSharedComponent2(),
                    new TestSharedComponent3(),
                    new TestUniqueComponent1(),
                    new TestUniqueComponent2()));
        }

        private Entity CreateTestEntity()
        {
            return Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 0 })
                    .SetComponent(new TestComponent2 { Prop = 0 })
                    .SetComponent(new TestComponent3 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent2 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent3 { Prop = 0 })
                    .SetUniqueComponent(new TestUniqueComponent1 { Prop = 0 })
                    .SetUniqueComponent(new TestUniqueComponent2 { Prop = 0 }),
                EntityState.Active);
        }
    }
}
