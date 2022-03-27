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
    public abstract class BaseEscContext_EntityGetTest<TEcsContextType> : BasePrePostTest<TEcsContextType>,
        IEntityGetTest
        where TEcsContextType : IEcsContextType
    {

        [TestMethod]
        public void GetEntities()
        {
            var entity1 = Context.CreateEntity();
            var entity2 = Context.CreateEntity();

            Assert.IsTrue(Context.GetEntities().Length == 2);
            Assert.IsTrue(Context.GetEntities()[0] == entity1);
            Assert.IsTrue(Context.GetEntities()[1] == entity2);
        }

        [TestMethod]
        public void GetEntities_Destroy()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(Entity.Null));
        }

        [TestMethod]
        public void HasEntity()
        {
            var entity = Context.CreateEntity();

            Assert.IsTrue(Context.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_Destroy()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(Entity.Null));
        }
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed
{

    [TestClass]
    public class EcsContext_Managed_EntityGetTest : BaseEscContext_EntityGetTest<EcsContextType_Managed>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed.ArcheType
{

    [TestClass]
    public class EcsContext_Managed_ArcheType_EntityGetTest : BaseEscContext_EntityGetTest<EcsContextType_Managed_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native
{

    [TestClass]
    public class EcsContext_Native_EntityGetTest : BaseEscContext_EntityGetTest<EcsContextType_Native>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType
{

    [TestClass]
    public class EcsContext_Native_ArcheType_EntityGetTest : BaseEscContext_EntityGetTest<EcsContextType_Native_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType.Continuous
{

    [TestClass]
    public class EcsContext_Native_ArcheType_Continuous_EntityGetTest : BaseEscContext_EntityGetTest<EcsContextType_Native_ArcheType_Continuous>
    {
    }
}
