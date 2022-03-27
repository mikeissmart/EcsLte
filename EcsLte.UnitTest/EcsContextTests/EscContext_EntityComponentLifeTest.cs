using EcsLte.Exceptions;
using EcsLte.UnitTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.EcsContextTests
{
    public abstract class BaseEscContext_EntityComponentLifeTest<TEcsContextType> : BasePrePostTest<TEcsContextType>,
        IEntityComponentLifeTest
        where TEcsContextType : IEcsContextType
    {

        [TestMethod]
        public void AddComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.AddComponent(Entity.Null, new TestComponent1()));
        }

        [TestMethod]
        public void AddComponent_Duplicate_Normal()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestComponent1());

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.AddComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void AddComponent_Duplicate_Shared()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestSharedComponent1());

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.AddComponent(entity, new TestSharedComponent1()));
        }

        [TestMethod]
        public void AddComponent_Duplicate_Unique_Different()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestUniqueComponent1());

            var entity2 = Context.CreateEntity();
            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.AddComponent(entity2, new TestUniqueComponent1()));
        }

        [TestMethod]
        public void AddComponent_Duplicate_Unique_Same()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestUniqueComponent1());

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.AddComponent(entity, new TestUniqueComponent1()));
        }

        [TestMethod]
        public void AddComponent_Null()
        {
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.AddComponent(Entity.Null, new TestComponent1()));
        }

        [TestMethod]
        public void AddUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void AddUniqueComponent_Duplicate()
        {
            Context.AddUniqueComponent(new TestUniqueComponent1());

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                Context.AddUniqueComponent(new TestUniqueComponent1()));
        }

        [TestMethod]
        public void RemoveAllComponents_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.RemoveAllComponents(Entity.Null));
        }

        [TestMethod]
        public void RemoveAllComponents_Null()
        {
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.RemoveAllComponents(Entity.Null));
        }

        [TestMethod]
        public void RemoveComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.RemoveComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void RemoveComponent_Duplicate_Normal()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestComponent1());
            Context.RemoveComponent<TestComponent1>(entity);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.RemoveComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponent_Duplicate_Shared()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestSharedComponent1());
            Context.RemoveComponent<TestSharedComponent1>(entity);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.RemoveComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponent_Duplicate_Unique()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestUniqueComponent1());
            Context.RemoveComponent<TestUniqueComponent1>(entity);

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.RemoveComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponent_Null()
        {
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.RemoveComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void RemoveUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.RemoveUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void RemoveUniqueComponent_Duplicate()
        {
            Context.AddUniqueComponent(new TestUniqueComponent1());
            Context.RemoveUniqueComponent<TestUniqueComponent1>();

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.RemoveUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void RemoveUniqueComponent_None()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.RemoveUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void ReplaceComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.ReplaceComponent(Entity.Null, new TestComponent1()));
        }

        [TestMethod]
        public void ReplaceComponent_Null()
        {
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                Context.ReplaceComponent(Entity.Null, new TestComponent1()));
        }

        [TestMethod]
        public void ReplaceUniqueComponent_Destroyed()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.ReplaceUniqueComponent(new TestUniqueComponent1()));
        }
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed
{

    [TestClass]
    public class EcsContext_Managed_EntityComponentLifeTest : BaseEscContext_EntityComponentLifeTest<EcsContextType_Managed>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed.ArcheType
{

    [TestClass]
    public class EcsContext_Managed_ArcheType_EntityComponentLifeTest : BaseEscContext_EntityComponentLifeTest<EcsContextType_Managed_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native
{

    [TestClass]
    public class EcsContext_Native_EntityComponentLifeTest : BaseEscContext_EntityComponentLifeTest<EcsContextType_Native>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType
{

    [TestClass]
    public class EcsContext_Native_ArcheType_EntityComponentLifeTest : BaseEscContext_EntityComponentLifeTest<EcsContextType_Native_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType.Continuous
{

    [TestClass]
    public class EcsContext_Native_ArcheType_Continuous_EntityComponentLifeTest : BaseEscContext_EntityComponentLifeTest<EcsContextType_Native_ArcheType_Continuous>
    {
    }
}
