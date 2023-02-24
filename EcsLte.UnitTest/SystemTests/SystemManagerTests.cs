using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.AccessControl;

namespace EcsLte.UnitTest.SystemTests
{
    [TestClass]
    public class SystemManagerTests : BasePrePostTest
    {
        [TestMethod]
        public void HasSystem()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();
            var systemB = new System_B();

            Assert.IsFalse(Context.Systems.HasSystem(systemA));
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem(systemA));

            Assert.IsFalse(Context.Systems.HasSystem(systemB));

            SystemBase systemNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.HasSystem(systemNull));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.HasSystem(systemA));
        }

        [TestMethod]
        public void HasSystem_Type()
        {
            var typeSystemA = typeof(System_A);
            var typeSystemB = typeof(System_B);

            Context.Systems.AddOrGetSystem(typeSystemA);

            Assert.IsFalse(Context.Systems.HasSystem(typeSystemA));
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem(typeSystemA));

            Assert.IsFalse(Context.Systems.HasSystem(typeSystemB));

            Type typeNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.HasSystem(typeNull));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.HasSystem(typeSystemA));
        }

        [TestMethod]
        public void HasSystem_Generic()
        {
            Context.Systems.AddOrGetSystem<System_A>();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());

            Assert.IsFalse(Context.Systems.HasSystem<System_B>());

            EcsContexts.Instance.DestroyContext(Context);
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

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.GetAllSystems());
        }

        [TestMethod]
        public void AddOrGetSystem_Generic()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem<System_A>());
            Assert.IsTrue(Context.Systems.AddOrGetSystem<System_A>() == systemA);
            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 1);

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AddOrGetSystem<System_A>());
        }

        [TestMethod]
        public void AddOrGetSystem_Type()
        {
            var typeSystemA = typeof(System_A);

            var systemA = Context.Systems.AddOrGetSystem(typeSystemA);

            Assert.IsFalse(Context.Systems.HasSystem(typeSystemA));
            Context.Systems.RunSystems();

            Assert.IsTrue(Context.Systems.HasSystem(typeSystemA));
            Assert.IsTrue(Context.Systems.AddOrGetSystem(typeSystemA) == systemA);
            Assert.IsTrue(Context.Systems.GetAllSystems().Length == 1);

            SystemBase systemNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.RemoveSystem(systemNull));
            EcsContexts.Instance.DestroyContext(Context);
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

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.AutoAddSystems());
        }

        [TestMethod]
        public void RemoveSystem()
        {
            var system = Context.Systems.AddOrGetSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.RemoveSystem(system);
            Context.Systems.RunSystems();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());

            SystemBase systemNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.RemoveSystem(systemNull));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RemoveSystem(system));
        }

        [TestMethod]
        public void RemoveSystem_Generic()
        {
            Context.Systems.AddOrGetSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.RemoveSystem<System_A>();
            Context.Systems.RunSystems();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RemoveSystem<System_A>());
        }

        [TestMethod]
        public void RemoveSystem_Type()
        {
            var systemTypeA = typeof(System_A);

            Context.Systems.AddOrGetSystem(systemTypeA);
            Context.Systems.RunSystems();

            Context.Systems.RemoveSystem(systemTypeA);
            Context.Systems.RunSystems();

            Assert.IsFalse(Context.Systems.HasSystem<System_A>());

            SystemBase systemNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.RemoveSystem(systemNull));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RemoveSystem(systemTypeA));
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

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.RemoveAllSystems());
        }

        [TestMethod]
        public void Activate()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();
            var systemB = new System_B();

            // Object created but not active yet
            Assert.IsFalse(systemA.IsActive);
            Assert.IsTrue(systemA.ActivatedCalledCount == 0);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.ActivatedCalledCount == 1);

            Context.Systems.DeactivateSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.ActivateSystem(systemA);
            Assert.IsTrue(systemA.ActivatedCalledCount == 1);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.ActivatedCalledCount == 2);

            SystemBase systemNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.ActivateSystem(systemNull));
            Assert.ThrowsException<SystenNotHaveException>(() =>
                Context.Systems.ActivateSystem(systemB));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.ActivateSystem<System_A>());
        }

        [TestMethod]
        public void Activate_Generic()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            // Object created but not active yet
            Assert.IsFalse(systemA.IsActive);
            Assert.IsTrue(systemA.ActivatedCalledCount == 0);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.ActivatedCalledCount == 1);

            Context.Systems.DeactivateSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.ActivateSystem<System_A>();
            Assert.IsTrue(systemA.ActivatedCalledCount == 1);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.ActivatedCalledCount == 2);

            Assert.ThrowsException<SystenNotHaveException>(() =>
                Context.Systems.ActivateSystem<System_B>());
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.ActivateSystem<System_A>());
        }

        [TestMethod]
        public void Activate_Type()
        {
            var systemAType = typeof(System_A);
            var systemBType = typeof(System_B);

            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            // Object created but not active yet
            Assert.IsFalse(systemA.IsActive);
            Assert.IsTrue(systemA.ActivatedCalledCount == 0);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.ActivatedCalledCount == 1);

            Context.Systems.DeactivateSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.ActivateSystem(systemAType);
            Assert.IsTrue(systemA.ActivatedCalledCount == 1);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.ActivatedCalledCount == 2);

            Type typeNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.ActivateSystem(typeNull));
            Assert.ThrowsException<SystenNotHaveException>(() =>
                Context.Systems.ActivateSystem(systemBType));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.ActivateSystem(systemAType));
        }

        [TestMethod]
        public void Activate_Duplicate()
        {
            var system = Context.Systems.AddOrGetSystem<System_A>();

            Context.Systems.RunSystems();

            Context.Systems.DeactivateSystem<System_A>();
            Context.Systems.RunSystems();

            Context.Systems.ActivateSystem(system);
            Context.Systems.ActivateSystem(system);
            Context.Systems.RunSystems();

            Assert.IsTrue(system.ActivatedCalledCount == 2);
        }

        [TestMethod]
        public void Deactivate()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();
            var systemB = new System_B();

            Context.Systems.RunSystems();

            Context.Systems.DeactivateSystem(systemA);
            Assert.IsTrue(systemA.DeactivatedCalledCount == 0);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.DeactivatedCalledCount == 1);

            Assert.ThrowsException<SystenNotHaveException>(() =>
                Context.Systems.DeactivateSystem(systemB));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.DeactivateSystem(systemA));
        }

        [TestMethod]
        public void Deactivate_Generic()
        {
            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            Context.Systems.RunSystems();

            Context.Systems.DeactivateSystem<System_A>();
            Assert.IsTrue(systemA.DeactivatedCalledCount == 0);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.DeactivatedCalledCount == 1);

            SystemBase systemNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.DeactivateSystem(systemNull));
            Assert.ThrowsException<SystenNotHaveException>(() =>
                Context.Systems.DeactivateSystem<System_B>());
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.DeactivateSystem<System_A>());
        }

        [TestMethod]
        public void Deactivate_Type()
        {
            var systemAType = typeof(System_A);
            var systemBType = typeof(System_B);

            var systemA = Context.Systems.AddOrGetSystem<System_A>();

            Context.Systems.RunSystems();

            Context.Systems.DeactivateSystem(systemAType);
            Assert.IsTrue(systemA.DeactivatedCalledCount == 0);

            Context.Systems.RunSystems();
            Assert.IsTrue(systemA.DeactivatedCalledCount == 1);

            Type typeNull = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Systems.DeactivateSystem(typeNull));
            Assert.ThrowsException<SystenNotHaveException>(() =>
                Context.Systems.DeactivateSystem(systemBType));
            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Systems.DeactivateSystem(systemAType));
        }

        [TestMethod]
        public void Activate_Deactivate_RunSystem()
        {
            var system = Context.Systems.AddOrGetSystem<System_A>();

            // Object created but not active yet
            Assert.IsFalse(system.IsActive);

            // Becomes initialized and active
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

            EcsContexts.Instance.DestroyContext(Context);
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
