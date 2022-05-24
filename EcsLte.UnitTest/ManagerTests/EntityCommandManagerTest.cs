using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityCommandManagerTest : BasePrePostTest
    {
        [TestMethod]
        public void HasCommandQueue()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");

            Assert.IsTrue(Context.Commands.HasCommandQueue("Test"));
        }

        [TestMethod]
        public void HasCommandQueue_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.HasCommandQueue("Test"));
        }

        [TestMethod]
        public void GetCommandQueue()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");

            Assert.IsTrue(queue == Context.Commands.GetCommandQueue("Test"));
        }

        [TestMethod]
        public void GetCommandQueue_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.GetCommandQueue("Test"));
        }

        [TestMethod]
        public void CreateCommandQueue()
        {
            var queue = Context.Commands.CreateCommandQueue("Test");

            Assert.IsTrue(queue.Name == "Test");
            Assert.IsTrue(queue.Context == Context);
        }

        [TestMethod]
        public void CreateCommandQueue_Destroyed()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Commands.CreateCommandQueue("Test"));
        }
    }
}