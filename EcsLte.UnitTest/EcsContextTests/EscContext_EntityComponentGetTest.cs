using EcsLte.Exceptions;
using EcsLte.UnitTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    public abstract class BaseEscContext_EntityComponentGetTest<TEcsContextType> : BasePrePostTest<TEcsContextType>,
        IEntityComponentGetTest
        where TEcsContextType : IEcsContextType
    {

        [TestMethod]
        public void GetAllComponents_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetAllComponents(Entity.Null));
        }

        [TestMethod]
        public void GetAllComponents_None()
        {
            var entity = Context.CreateEntity();

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 0);
        }

        [TestMethod]
        public void GetAllComponents_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                 Context.GetAllComponents(Entity.Null));

        [TestMethod]
        public void GetComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetComponent_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                             Context.GetComponent<TestComponent1>(Entity.Null));

        [TestMethod]
        public void GetUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void GetUniqueComponent_None() => Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                                                   Context.GetUniqueComponent<TestUniqueComponent1>());

        [TestMethod]
        public void GetUniqueEntity_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void GetUniqueEntity_None() => Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                                                Context.GetUniqueEntity<TestUniqueComponent1>());

        [TestMethod]
        public void HasComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void HasComponent_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                             Context.HasComponent<TestComponent1>(Entity.Null));

        [TestMethod]
        public void HasUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void HasUniqueComponent_None() => Assert.IsFalse(Context.HasUniqueComponent<TestUniqueComponent1>());
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed
{

    [TestClass]
    public class EcsContext_Managed_EntityComponentGetTest : BaseEscContext_EntityComponentGetTest<EcsContextType_Managed>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed.ArcheType
{

    [TestClass]
    public class EcsContext_Managed_ArcheType_EntityComponentGetTest : BaseEscContext_EntityComponentGetTest<EcsContextType_Managed_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native
{

    [TestClass]
    public class EcsContext_Native_EntityComponentGetTest : BaseEscContext_EntityComponentGetTest<EcsContextType_Native>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType
{

    [TestClass]
    public class EcsContext_Native_ArcheType_EntityComponentGetTest : BaseEscContext_EntityComponentGetTest<EcsContextType_Native_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType.Continuous
{

    [TestClass]
    public class EcsContext_Native_ArcheType_Continuous_EntityComponentGetTest : BaseEscContext_EntityComponentGetTest<EcsContextType_Native_ArcheType_Continuous>
    {
    }
}