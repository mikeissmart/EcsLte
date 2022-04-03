using EcsLte.Exceptions;
using EcsLte.UnitTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EcsContextTests
{
    public abstract class BaseEscContext_EntityLifeTest<TEcsContextType> : BasePrePostTest<TEcsContextType>,
        IEntityLifeTest
        where TEcsContextType : IEcsContextType
    {

        [TestMethod]
        public void CreateEntities()
        {
            var entities = Context.CreateEntities(2);

            Assert.IsTrue(Context.HasEntity(entities[0]));
            Assert.IsTrue(Context.HasEntity(entities[1]));
            Assert.IsTrue(Context.GetEntities()[0] == entities[0]);
            Assert.IsTrue(Context.GetEntities()[1] == entities[1]);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0].Version == 1);
            Assert.IsTrue(entities[1].Version == 1);
        }

        [TestMethod]
        public void CreateEntities_Destroy()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntities(2));
        }

        [TestMethod]
        public void CreateEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(entities[i].Id == i + 1, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities_Reuse()
        {
            var entities = Context.CreateEntities(2);
            Context.DestroyEntities(entities);

            entities = Context.CreateEntities(2);
            for (int i = 0, lifoId = 2; i < entities.Length; i++, lifoId--)
            {
                Assert.IsTrue(entities[i].Id == lifoId, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities_Reuse_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount);
            Context.DestroyEntities(entities);

            entities = Context.CreateEntities(UnitTestConsts.LargeCount);
            for (int i = 0, lifoId = UnitTestConsts.LargeCount; i < entities.Length; i++, lifoId--)
            {
                Assert.IsTrue(entities[i].Id == lifoId, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntity()
        {
            var entity = Context.CreateEntity();

            Assert.IsTrue(Context.HasEntity(entity));
            Assert.IsTrue(Context.GetEntities()[0] == entity);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
        }

        [TestMethod]
        public void CreateEntity_Destroy()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.CreateEntity());
        }

        [TestMethod]
        public void CreateEntity_Large()
        {
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity();

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(entities[i].Id == i + 1, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntity_Reuse()
        {
            var entity = Context.CreateEntity();
            Context.DestroyEntity(entity);

            entity = Context.CreateEntity();

            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
            Assert.IsTrue(Context.HasEntity(entity));
            Assert.IsTrue(Context.GetEntities()[0] == entity);
        }

        [TestMethod]
        public void CreateEntity_Reuse_Large()
        {
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity();
            Context.DestroyEntities(entities);

            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.CreateEntity();
            for (int i = 0, lifoId = UnitTestConsts.LargeCount; i < entities.Length; i++, lifoId--)
            {
                Assert.IsTrue(entities[i].Id == lifoId, $"Entity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2, $"Entity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = Context.CreateEntities(2);

            Context.DestroyEntities(entities);

            Assert.IsFalse(Context.HasEntity(entities[0]));
            Assert.IsFalse(Context.HasEntity(entities[1]));
            Assert.IsTrue(Context.GetEntities().Length == 0);
        }

        [TestMethod]
        public void DestroyEntities_Destroy()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntities(new Entity[0]));
        }

        [TestMethod]
        public void DestroyEntities_Large()
        {
            var entities = Context.CreateEntities(UnitTestConsts.LargeCount);

            Context.DestroyEntities(entities);

            Assert.IsTrue(Context.GetEntities().Length == 0);
            for (var i = 0; i < entities.Length; i++)
                Assert.IsFalse(Context.HasEntity(entities[i]), $"Entity.Id {entities[i].Id}");
        }

        [TestMethod]
        public void DestroyEntities_EntityNull() => Assert.ThrowsException<ArgumentNullException>(() =>
                                                      Context.DestroyEntities(null));

        [TestMethod]
        public void DestroyEntities_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                Context.DestroyEntities(new Entity[] { Entity.Null }));

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.CreateEntity();

            Context.DestroyEntity(entity);

            Assert.IsFalse(Context.HasEntity(entity));
            Assert.IsTrue(Context.GetEntities().Length == 0);
        }

        [TestMethod]
        public void DestroyEntity_Destroy()
        {
            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntity_Null() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                              Context.DestroyEntity(Entity.Null));
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed
{
    [TestClass]
    public class EcsContext_Managed_EntityLifeTest : BaseEscContext_EntityLifeTest<EcsContextType_Managed>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed.ArcheType
{

    [TestClass]
    public class EcsContext_Managed_ArcheType_EntityLifeTest : BaseEscContext_EntityLifeTest<EcsContextType_Managed_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native
{

    [TestClass]
    public class EcsContext_Native_EntityLifeTest : BaseEscContext_EntityLifeTest<EcsContextType_Native>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType
{

    [TestClass]
    public class EcsContext_Native_ArcheType_EntityLifeTest : BaseEscContext_EntityLifeTest<EcsContextType_Native_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType.Continuous
{

    [TestClass]
    public class EcsContext_Native_ArcheType_Continuous_EntityLifeTest : BaseEscContext_EntityLifeTest<EcsContextType_Native_ArcheType_Continuous>
    {
    }
}
