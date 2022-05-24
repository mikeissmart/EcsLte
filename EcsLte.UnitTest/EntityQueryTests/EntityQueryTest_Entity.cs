using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTest_Entity : BasePrePostTest
    {
        [TestMethod]
        public void GetEntities_Destroyed()
        {
            var query = new EntityQuery();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.GetEntities(query));
        }

        [TestMethod]
        public void GetEntities()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.SmallCount, new TestComponent1());
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            var getEntities = Context.GetEntities(query);
            Assert.IsTrue(getEntities.Length == entities.Length);
            Assert.IsTrue(getEntities.Length == Context.EntityCount(query));
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(getEntities[i] == entities[i],
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void HasEntity_Destroyed()
        {
            var query = new EntityQuery();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.HasEntity(Entity.Null, query));
        }

        [TestMethod]
        public void HasEntity_Different_ArcheTypes()
        {
            var entity1 = TestCreateEntities(Context, 1,
                new TestComponent1())[0];
            var entity2 = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestComponent2())[0];
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(Context.GetEntities(query).Length == 2);
            Assert.IsTrue(Context.HasEntity(entity1, query));
            Assert.IsTrue(Context.HasEntity(entity2, query));
        }

        [TestMethod]
        public void HasEntity_Different_ArcheTypes_Add()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();
            Assert.IsTrue(Context.GetEntities(query).Length == 0);

            var entity1 = TestCreateEntities(Context, 1,
                new TestComponent1())[0];
            Assert.IsTrue(Context.GetEntities(query).Length == 1);
            Assert.IsTrue(Context.HasEntity(entity1, query));

            var entity2 = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestComponent2())[0];

            Assert.IsTrue(Context.GetEntities(query).Length == 2);
            Assert.IsTrue(Context.HasEntity(entity1, query));
            Assert.IsTrue(Context.HasEntity(entity2, query));
        }

        [TestMethod]
        public void HasEntity_Large()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.LargeCount, new TestComponent1());
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.HasEntity(entities[i], query),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void HasEntity_Single()
        {
            var entity = TestCreateEntities(Context, 1, new TestComponent1())[0];
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(Context.HasEntity(entity, query));
            Assert.IsFalse(Context.HasEntity(Entity.Null, query));
        }
    }
}