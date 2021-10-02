using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandQueueTests
{
    [TestClass]
    public class EntityCommandQueue_EntityCommandQueue : BasePrePostTest
    {
        [TestMethod]
        public void Name()
        {
            var commandQueue = _context.CommandQueue("Test");

            // Correct name
            Assert.IsTrue(commandQueue.Name == "Test");
        }

        [TestMethod]
        public void RunCommands()
        {
            var commandQueue = _context.CommandQueue("Test");
            var entity = commandQueue.CreateEntity();

            commandQueue.RunCommands();

            // Correct entity
            Assert.IsTrue(_context.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                commandQueue.RunCommands());
        }

        [TestMethod]
        public void ClearCommands()
        {
            var commandQueue = _context.CommandQueue("Test");
            var entity = commandQueue.CreateEntity();

            commandQueue.ClearCommands();
            commandQueue.RunCommands();

            // Correct entity
            Assert.IsFalse(_context.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                commandQueue.ClearCommands());
        }
    }
}