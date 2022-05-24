using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class SystemManagerTest : BasePrePostTest
    {
        [TestMethod]
        public void HasSystem()
        {
            Context.Systems.AddSystem<System_A>();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());
        }

        [TestMethod]
        public void HasSystem_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.HasSystem<System_A>());
        }

        [TestMethod]
        public void HasSystem_Never() => Assert.IsFalse(Context.Systems.HasSystem<System_A>());

        [TestMethod]
        public void GetSystem()
        {
            var systemA = Context.Systems.AddSystem<System_A>();

            Assert.IsTrue(Context.Systems.GetSystem<System_A>() == systemA);
        }

        [TestMethod]
        public void GetSystem_Destroy()
        {
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
        }

        [TestMethod]
        public void GetAllSystems_Destroy()
        {
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
        }

        [TestMethod]
        public void GetAllInitializeSystems_Destroy()
        {
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
        }

        [TestMethod]
        public void GetAllExecuteSystems_Destroy()
        {
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
        }

        [TestMethod]
        public void GetAllCleanupSystems_Destroy()
        {
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
        }

        [TestMethod]
        public void GetAllTearDownSystems_Destroy()
        {
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
        }

        [TestMethod]
        public void AddSystem_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AddSystem<System_A>());
        }

        [TestMethod]
        public void AddSystem_Duplicate()
        {
            Context.Systems.AddSystem<System_A>();

            Assert.ThrowsException<SystemAlreadyHasException>(() =>
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
        }

        [TestMethod]
        public void AutoAddSystems_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AutoAddSystems());
        }

        [TestMethod]
        public void RunInitializeSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

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
        }

        [TestMethod]
        public void RunInitializeSystems_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunInitializeSystems());
        }

        [TestMethod]
        public void RunInitializeSystems_InOrder()
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
        }

        [TestMethod]
        public void RunExecuteSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

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
        }

        [TestMethod]
        public void RunExecuteSystems_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunExecuteSystems());
        }

        [TestMethod]
        public void RunExecuteSystems_InOrder()
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
        }

        [TestMethod]
        public void RunCleanupSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

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
        }

        [TestMethod]
        public void RunCleanupSystems_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunCleanupSystems());
        }

        [TestMethod]
        public void RunCleanupSystems_InOrder()
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
        }

        [TestMethod]
        public void RunTearDownSystems()
        {
            Context.Systems.AutoAddSystems();
            var systemA = Context.Systems.GetSystem<System_A>();
            var systemB = Context.Systems.GetSystem<System_B>();
            var systemC = Context.Systems.GetSystem<System_C>();

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
        }

        [TestMethod]
        public void RunTearDownSystems_Destroy()
        {
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunTearDownSystems());
        }

        [TestMethod]
        public void RunTearDownSystems_InOrder()
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
        }
    }
}