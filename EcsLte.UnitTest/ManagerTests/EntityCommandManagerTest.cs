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
            var queue = Context.CommandManager.CreateCommandQueue("Test");

            Assert.IsTrue(Context.CommandManager.HasCommandQueue("Test"));
        }

        [TestMethod]
        public void HasCommandQueue_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CommandManager.HasCommandQueue("Test"));
        }

        [TestMethod]
        public void GetCommandQueue()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");

            Assert.IsTrue(queue == Context.CommandManager.GetCommandQueue("Test"));
        }

        [TestMethod]
        public void GetCommandQueue_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CommandManager.GetCommandQueue("Test"));
        }

        [TestMethod]
        public void CreateCommandQueue()
        {
            var queue = Context.CommandManager.CreateCommandQueue("Test");

            Assert.IsTrue(queue.Name == "Test");
            Assert.IsTrue(queue.Context == Context);
        }

        [TestMethod]
        public void CreateCommandQueue_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CommandManager.CreateCommandQueue("Test"));
        }
    }
}
