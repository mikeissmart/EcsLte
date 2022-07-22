using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsManagerTests : BasePrePostTest
    {
        [TestMethod]
        public void HasCommands()
        {
           Context.Commands.CreateCommands("Commands");

            Assert.IsTrue(Context.Commands.HasCommands("Commands"));
            Assert.IsFalse(Context.Commands.HasCommands("Commands1"));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Commands.HasCommands(null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.HasCommands("Commands"));
        }

        [TestMethod]
        public void GetCommands()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            Assert.IsTrue(Context.Commands.GetCommands("Commands") == commands);

            Assert.ThrowsException<EntityCommandsNotExistException>(() =>
                Context.Commands.GetCommands("Commands Not Exist"));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Commands.GetCommands(null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.GetCommands("Commands"));
        }

        [TestMethod]
        public void CreateCommands()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            Assert.IsTrue(commands != null);
            Assert.IsTrue(commands.IsDestroyed == false);

            Assert.ThrowsException<EntityCommandsAlreadyExistException>(() =>
                Context.Commands.CreateCommands("Commands"));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Commands.CreateCommands(null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.CreateCommands("Commands"));
        }

        [TestMethod]
        public void RemoveCommands()
        {
            var commands = Context.Commands.CreateCommands("Commands");
            Context.Commands.RemoveCommands(commands);

            Assert.IsFalse(Context.Commands.HasCommands(commands.Name));
            Assert.IsTrue(commands.IsDestroyed);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                Context.Commands.RemoveCommands(commands));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Commands.RemoveCommands(null));

            var commands2 = EcsContexts.CreateContext("Context2")
                .Commands.CreateCommands("Commands");
            Assert.ThrowsException<EcsContextDifferentException>(() =>
                Context.Commands.RemoveCommands(commands2));

            commands2 = Context.Commands.CreateCommands("Commands2");

            EcsContexts.DestroyContext(Context);

            Assert.IsTrue(commands2.IsDestroyed);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.RemoveCommands(commands));
        }
    }
}