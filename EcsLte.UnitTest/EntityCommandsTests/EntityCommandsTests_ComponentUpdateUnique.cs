using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_ComponentUpdateUnique : BasePrePostTest
    {
        [TestMethod]
        public void UpdateUniqueComponent()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetUniqueComponent(new TestUniqueComponent1 { Prop = 0 }),
                EntityState.Active);

            commands.UpdateUniqueComponent(entity, new TestUniqueComponent1 { Prop = 1 });

            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>(entity).Prop == 1);

            commands.UpdateUniqueComponent(new TestUniqueComponent1 { Prop = 2 });

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetUniqueComponent<TestUniqueComponent1>().Prop == 2);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateUniqueComponent(entity, new TestUniqueComponent1()));
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateUniqueComponent(new TestUniqueComponent1()));
        }
    }
}
