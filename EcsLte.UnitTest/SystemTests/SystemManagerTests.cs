using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.SystemTests
{
    [TestClass]
    public class SystemManagerTests : BasePrePostTest
    {
        [TestMethod]
        public void HasSystem()
        {
            Context.Systems.AddOrGetSystem<System_A>();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());

            Assert.IsFalse(Context.Systems.HasSystem<System_B>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.HasSystem<System_A>());
        }

        [TestMethod]
        public void GetAllSystems()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 0);
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 1);
            Assert.IsTrue(Context.Systems.GetAllSystems()[0] == systemA);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllSystems());
        }

        [TestMethod]
        public void AddOrGetSystem()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());
            Assert.IsTrue(Context.Systems.AddOrGetSystem<System_A>() == systemA);
            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 1);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AddOrGetSystem<System_A>());
        }

        [TestMethod]
        public void AutoAddSystems()
        {
            Context.Systems.AutoAddSystems();

            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 0);
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 3);
            Assert.IsTrue(Context.Systems.HasSystem<System_A>());
            Assert.IsTrue(Context.Systems.HasSystem<System_B>());
            Assert.IsTrue(Context.Systems.HasSystem<System_C>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AutoAddSystems());
        }

        [TestMethod]
        public void RemoveSystem()
        {
            Context.Systems.AddOrGetSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.RemoveSystem<System_A>();
            Context.Systems.RunSystems();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RemoveSystem<System_A>());
        }

        [TestMethod]
        public void RemoveAllSystems()
        {
            Context.Systems.AddOrGetSystem<System_A>();
            Context.Systems.AddOrGetSystem<System_B>();
            Context.Systems.AddOrGetSystem<System_C>();
            Context.Systems.RunSystems();

            Context.Systems.RemoveAllSystems();
            Context.Systems.RunSystems();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());
            Assert.IsFalse(Context.Systems.HasSystem<System_B>());
            Assert.IsFalse(Context.Systems.HasSystem<System_C>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RemoveAllSystems());
        }

        [TestMethod]
        public void Activate_Deactivate_RunSystem()
        {
            var system = Context.Systems.AddOrGetSystem<System_A>();

            // Object created but not active yet
            Assert.IsFalse(system.IsActive);

            // Becomes active and initialized
            Context.Systems.RunSystems();
            Assert.IsTrue(system.InitializeCalledCount == 1);
            Assert.IsTrue(system.ActivatedCalledCount == 1);
            Assert.IsTrue(system.UpdateCalledCount == 1);
            Assert.IsTrue(system.DeactivatedCalledCount == 0);
            Assert.IsTrue(system.UninitializeCalledCount == 0);
            Assert.IsTrue(system.IsActive);

            // Queue deactivation
            Context.Systems.DeactivateSystem<System_A>();
            Assert.IsTrue(system.IsActive);

            // Run deactivates and doesnt invoke update
            Context.Systems.RunSystems();
            Assert.IsTrue(system.InitializeCalledCount == 1);
            Assert.IsTrue(system.ActivatedCalledCount == 1);
            Assert.IsTrue(system.UpdateCalledCount == 1);
            Assert.IsTrue(system.DeactivatedCalledCount == 1);
            Assert.IsTrue(system.UninitializeCalledCount == 0);
            Assert.IsFalse(system.IsActive);

            // Queue activation
            Context.Systems.ActivateSystem<System_A>();
            Assert.IsFalse(system.IsActive);

            // Run activates and updates
            Context.Systems.RunSystems();
            Assert.IsTrue(system.InitializeCalledCount == 1);
            Assert.IsTrue(system.ActivatedCalledCount == 2);
            Assert.IsTrue(system.UpdateCalledCount == 2);
            Assert.IsTrue(system.DeactivatedCalledCount == 1);
            Assert.IsTrue(system.UninitializeCalledCount == 0);
            Assert.IsTrue(system.IsActive);

            // Queue deactivate(if active) and remove/uninitialize
            Context.Systems.RemoveSystem<System_A>();
            Assert.IsTrue(Context.Systems.HasSystem<System_A>());

            // Queue deactivate(if active) and remove/uninitialize
            Context.Systems.RunSystems();
            Assert.IsTrue(system.InitializeCalledCount == 1);
            Assert.IsTrue(system.ActivatedCalledCount == 2);
            Assert.IsTrue(system.UpdateCalledCount == 2);
            Assert.IsTrue(system.DeactivatedCalledCount == 2);
            Assert.IsTrue(system.UninitializeCalledCount == 1);
            Assert.IsFalse(system.IsActive);
            Assert.IsFalse(Context.Systems.HasSystem<System_A>());

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.ActivateSystem<System_A>());
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.DeactivateSystem<System_A>());
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RunSystems());
        }

        [TestMethod]
        public void RunSystem_Order()
        {
            Context.Systems.AutoAddSystems();
            var systemOrder = new SystemOrder();

            var systemA = Context.Systems.AddOrGetSystem<System_A>();
            var systemB = Context.Systems.AddOrGetSystem<System_B>();
            var systemC = Context.Systems.AddOrGetSystem<System_C>();

            systemA.SystemOrder = systemOrder;
            systemB.SystemOrder = systemOrder;
            systemC.SystemOrder = systemOrder;

            Context.Systems.RunSystems();

            Context.Systems.RunSystems();
            Assert.IsTrue(systemOrder.Order[0] == systemA);
            Assert.IsTrue(systemOrder.Order[1] == systemB);
            Assert.IsTrue(systemOrder.Order[2] == systemC);
        }
    }
}
