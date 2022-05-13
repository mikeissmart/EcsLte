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
            Context.SystemManager.AddSystem<System_A>();

            Assert.IsTrue(Context.SystemManager.HasSystem<System_A>());
        }

        [TestMethod]
        public void HasSystem_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.HasSystem<System_A>());
        }

        [TestMethod]
        public void HasSystem_Never() => Assert.IsFalse(Context.SystemManager.HasSystem<System_A>());

        [TestMethod]
        public void GetSystem()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();

            Assert.IsTrue(Context.SystemManager.GetSystem<System_A>() == systemA);
        }

        [TestMethod]
        public void GetSystem_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.GetSystem<System_A>());
        }

        [TestMethod]
        public void GetAllSystems()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();

            Assert.IsTrue(Context.SystemManager.GetAllSystems().Length == 1);
            Assert.IsTrue(Context.SystemManager.GetAllSystems()[0] == systemA);
        }

        [TestMethod]
        public void GetAllSystems_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.GetAllSystems());
        }

        [TestMethod]
        public void GetAllInitializeSystems()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();
            var systemInitialize = Context.SystemManager.AddSystem<System_Initialize>();
            Context.SystemManager.AddSystem<System_Execute>();

            Assert.IsTrue(Context.SystemManager.GetAllInitializeSystems().Length == 2);
            Assert.IsTrue(Context.SystemManager.GetAllInitializeSystems().Contains(systemA));
            Assert.IsTrue(Context.SystemManager.GetAllInitializeSystems().Contains(systemInitialize));
        }

        [TestMethod]
        public void GetAllInitializeSystems_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.GetAllInitializeSystems());
        }

        [TestMethod]
        public void GetAllExecuteSystems()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();
            var systemExecute = Context.SystemManager.AddSystem<System_Execute>();
            Context.SystemManager.AddSystem<System_Initialize>();

            Assert.IsTrue(Context.SystemManager.GetAllExecuteSystems().Length == 2);
            Assert.IsTrue(Context.SystemManager.GetAllExecuteSystems().Contains(systemA));
            Assert.IsTrue(Context.SystemManager.GetAllExecuteSystems().Contains(systemExecute));
        }

        [TestMethod]
        public void GetAllExecuteSystems_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.GetAllExecuteSystems());
        }

        [TestMethod]
        public void GetAllCleanupSystems()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();
            var systemCleanup = Context.SystemManager.AddSystem<System_Cleanup>();
            Context.SystemManager.AddSystem<System_Initialize>();

            Assert.IsTrue(Context.SystemManager.GetAllCleanupSystems().Length == 2);
            Assert.IsTrue(Context.SystemManager.GetAllCleanupSystems().Contains(systemA));
            Assert.IsTrue(Context.SystemManager.GetAllCleanupSystems().Contains(systemCleanup));
        }

        [TestMethod]
        public void GetAllCleanupSystems_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.GetAllCleanupSystems());
        }

        [TestMethod]
        public void GetAllTearDownSystems()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();
            var systemTearDown = Context.SystemManager.AddSystem<System_TearDown>();
            Context.SystemManager.AddSystem<System_Initialize>();

            Assert.IsTrue(Context.SystemManager.GetAllTearDownSystems().Length == 2);
            Assert.IsTrue(Context.SystemManager.GetAllTearDownSystems().Contains(systemA));
            Assert.IsTrue(Context.SystemManager.GetAllTearDownSystems().Contains(systemTearDown));
        }

        [TestMethod]
        public void GetAllTearDownSystems_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.GetAllTearDownSystems());
        }

        [TestMethod]
        public void AddSystem()
        {
            var systemA = Context.SystemManager.AddSystem<System_A>();

            Assert.IsTrue(Context.SystemManager.HasSystem<System_A>());
            Assert.IsTrue(Context.SystemManager.GetSystem<System_A>() == systemA);
            Assert.IsTrue(Context.SystemManager.GetAllSystems().Length == 1);
        }

        [TestMethod]
        public void AddSystem_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.AddSystem<System_A>());
        }

        [TestMethod]
        public void AddSystem_Duplicate()
        {
            Context.SystemManager.AddSystem<System_A>();

            Assert.ThrowsException<SystemAlreadyHasException>(() =>
                Context.SystemManager.AddSystem<System_A>());
        }

        [TestMethod]
        public void AutoAddSystems()
        {
            Context.SystemManager.AutoAddSystems();

            Assert.IsTrue(Context.SystemManager.GetAllSystems().Length == 3);
            Assert.IsTrue(Context.SystemManager.HasSystem<System_A>());
            Assert.IsTrue(Context.SystemManager.HasSystem<System_B>());
            Assert.IsTrue(Context.SystemManager.HasSystem<System_C>());
        }

        [TestMethod]
        public void AutoAddSystems_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.AutoAddSystems());
        }

        [TestMethod]
        public void RunInitializeSystems()
        {
            Context.SystemManager.AutoAddSystems();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            Context.SystemManager.RunInitializeSystems();

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
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.RunInitializeSystems());
        }

        [TestMethod]
        public void RunInitializeSystems_InOrder()
        {
            Context.SystemManager.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.SystemManager.RunInitializeSystems();

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
            Context.SystemManager.AutoAddSystems();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            Context.SystemManager.RunExecuteSystems();

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
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.RunExecuteSystems());
        }

        [TestMethod]
        public void RunExecuteSystems_InOrder()
        {
            Context.SystemManager.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.SystemManager.RunExecuteSystems();

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
            Context.SystemManager.AutoAddSystems();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            Context.SystemManager.RunCleanupSystems();

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
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.RunCleanupSystems());
        }

        [TestMethod]
        public void RunCleanupSystems_InOrder()
        {
            Context.SystemManager.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.SystemManager.RunCleanupSystems();

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
            Context.SystemManager.AutoAddSystems();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            Context.SystemManager.RunTearDownSystems();

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
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.SystemManager.RunTearDownSystems());
        }

        [TestMethod]
        public void RunTearDownSystems_InOrder()
        {
            Context.SystemManager.AutoAddSystems();
            var systemOrder = new SystemOrder();
            var systemA = Context.SystemManager.GetSystem<System_A>();
            var systemB = Context.SystemManager.GetSystem<System_B>();
            var systemC = Context.SystemManager.GetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.SystemManager.RunTearDownSystems();

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
