using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EcsLte.UnitTest.SystemsTests
{
    [TestClass]
    public class SystemsManagerTest : BasePrePostTest
    {
        [TestMethod]
        public void HasSystem()
        {
            Context.Systems.AddSystem<System_A>();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());

            Assert.IsFalse(Context.Systems.HasSystem<System_B>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.HasSystem<System_A>());
        }

        [TestMethod]
        public void GetSystem()
        {
            var systemA = Context.Systems.AddSystem<System_A>();

            Assert.IsTrue(Context.Systems.GetSystem<System_A>() == systemA);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetSystem<System_A>());
        }

        [TestMethod]
        public void GetAllSystems()
        {
            var systemA = Context.Systems.AddSystem<System_A>();

            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 1);
            Assert.IsTrue(Context.Systems.GetAllSystems()[0] == systemA);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllSystems());
        }

        [TestMethod]
        public void GetAllInitializeSystems()
        {
            var systemA = Context.Systems.AddSystem<System_A>();
            var systemInitialize = Context.Systems.AddSystem<System_Initialize>();
            Context.Systems.AddSystem<System_Execute>();

            Assert.IsTrue(Context.Systems.GetAllInitializeSystems().Length == 2);
            Assert.IsTrue(Context.Systems.GetAllInitializeSystems().Contains(systemA));
            Assert.IsTrue(Context.Systems.GetAllInitializeSystems().Contains(systemInitialize));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllInitializeSystems());
        }

        [TestMethod]
        public void GetAllExecuteSystems()
        {
            var systemA = Context.Systems.AddSystem<System_A>();
            var systemExecute = Context.Systems.AddSystem<System_Execute>();
            Context.Systems.AddSystem<System_Initialize>();

            Assert.IsTrue(Context.Systems.GetAllExecuteSystems().Length == 2);
            Assert.IsTrue(Context.Systems.GetAllExecuteSystems().Contains(systemA));
            Assert.IsTrue(Context.Systems.GetAllExecuteSystems().Contains(systemExecute));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllExecuteSystems());
        }

        [TestMethod]
        public void GetAllCleanupSystems()
        {
            var systemA = Context.Systems.AddSystem<System_A>();
            var systemCleanup = Context.Systems.AddSystem<System_Cleanup>();
            Context.Systems.AddSystem<System_Initialize>();

            Assert.IsTrue(Context.Systems.GetAllCleanupSystems().Length == 2);
            Assert.IsTrue(Context.Systems.GetAllCleanupSystems().Contains(systemA));
            Assert.IsTrue(Context.Systems.GetAllCleanupSystems().Contains(systemCleanup));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllCleanupSystems());
        }

        [TestMethod]
        public void GetAllTearDownSystems()
        {
            var systemA = Context.Systems.AddSystem<System_A>();
            var systemTearDown = Context.Systems.AddSystem<System_TearDown>();
            Context.Systems.AddSystem<System_Initialize>();

            Assert.IsTrue(Context.Systems.GetAllTearDownSystems().Length == 2);
            Assert.IsTrue(Context.Systems.GetAllTearDownSystems().Contains(systemA));
            Assert.IsTrue(Context.Systems.GetAllTearDownSystems().Contains(systemTearDown));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllTearDownSystems());
        }

        [TestMethod]
        public void AddSystem()
        {
            var systemA = Context.Systems.AddSystem<System_A>();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());
            Assert.IsTrue(Context.Systems.GetSystem<System_A>() == systemA);
            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 1);

            Assert.ThrowsException<SystemAlreadyHasException>(() =>
                Context.Systems.AddSystem<System_A>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AddSystem<System_A>());
        }

        [TestMethod]
        public void AutoAddSystems()
        {
            Context.Systems.AutoAddSystems();

            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 3);
            Assert.IsTrue(Context.Systems.HasSystem<System_A>());
            Assert.IsTrue(Context.Systems.HasSystem<System_B>());
            Assert.IsTrue(Context.Systems.HasSystem<System_C>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AutoAddSystems());
        }

        [TestMethod]
        public void RunInitializeSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.Systems.RunInitializeSystems();

            Assert.IsTrue(systemA.InitializeCalledCount == 1 &&
                systemA.ExecuteCalledCount == 0 &&
                systemA.CleanupCalledCount == 0 &&
                systemA.TearDownCalledCount == 0);
            Assert.IsTrue(systemB.InitializeCalledCount == 1 &&
                systemB.ExecuteCalledCount == 0 &&
                systemB.CleanupCalledCount == 0 &&
                systemB.TearDownCalledCount == 0);
            Assert.IsTrue(systemC.InitializeCalledCount == 1 &&
                systemC.ExecuteCalledCount == 0 &&
                systemC.CleanupCalledCount == 0 &&
                systemC.TearDownCalledCount == 0);
            Assert.IsTrue(systemOrder.Order[0] == systemA);
            Assert.IsTrue(systemOrder.Order[1] == systemB);
            Assert.IsTrue(systemOrder.Order[2] == systemC);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunInitializeSystems());
        }

        [TestMethod]
        public void RunExecuteSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.Systems.RunExecuteSystems();

            Assert.IsTrue(systemA.InitializeCalledCount == 0 &&
                systemA.ExecuteCalledCount == 1 &&
                systemA.CleanupCalledCount == 0 &&
                systemA.TearDownCalledCount == 0);
            Assert.IsTrue(systemB.InitializeCalledCount == 0 &&
                systemB.ExecuteCalledCount == 1 &&
                systemB.CleanupCalledCount == 0 &&
                systemB.TearDownCalledCount == 0);
            Assert.IsTrue(systemC.InitializeCalledCount == 0 &&
                systemC.ExecuteCalledCount == 1 &&
                systemC.CleanupCalledCount == 0 &&
                systemC.TearDownCalledCount == 0);
            Assert.IsTrue(systemOrder.Order[0] == systemA);
            Assert.IsTrue(systemOrder.Order[1] == systemB);
            Assert.IsTrue(systemOrder.Order[2] == systemC);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunExecuteSystems());
        }

        [TestMethod]
        public void RunCleanupSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.Systems.RunCleanupSystems();

            Assert.IsTrue(systemA.InitializeCalledCount == 0 &&
                systemA.ExecuteCalledCount == 0 &&
                systemA.CleanupCalledCount == 1 &&
                systemA.TearDownCalledCount == 0);
            Assert.IsTrue(systemB.InitializeCalledCount == 0 &&
                systemB.ExecuteCalledCount == 0 &&
                systemB.CleanupCalledCount == 1 &&
                systemB.TearDownCalledCount == 0);
            Assert.IsTrue(systemC.InitializeCalledCount == 0 &&
                systemC.ExecuteCalledCount == 0 &&
                systemC.CleanupCalledCount == 1 &&
                systemC.TearDownCalledCount == 0);
            Assert.IsTrue(systemOrder.Order[0] == systemA);
            Assert.IsTrue(systemOrder.Order[1] == systemB);
            Assert.IsTrue(systemOrder.Order[2] == systemC);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunCleanupSystems());
        }

        [TestMethod]
        public void RunTearDownSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.Systems.RunTearDownSystems();

            Assert.IsTrue(systemA.InitializeCalledCount == 0 &&
                systemA.ExecuteCalledCount == 0 &&
                systemA.CleanupCalledCount == 0 &&
                systemA.TearDownCalledCount == 1);
            Assert.IsTrue(systemB.InitializeCalledCount == 0 &&
                systemB.ExecuteCalledCount == 0 &&
                systemB.CleanupCalledCount == 0 &&
                systemB.TearDownCalledCount == 1);
            Assert.IsTrue(systemC.InitializeCalledCount == 0 &&
                systemC.ExecuteCalledCount == 0 &&
                systemC.CleanupCalledCount == 0 &&
                systemC.TearDownCalledCount == 1);
            Assert.IsTrue(systemOrder.Order[0] == systemA);
            Assert.IsTrue(systemOrder.Order[1] == systemB);
            Assert.IsTrue(systemOrder.Order[2] == systemC);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunTearDownSystems());
        }
    }
}