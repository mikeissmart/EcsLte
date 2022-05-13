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
            var query = Context.QueryManager.CreateQuery();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                query.GetEntities());
        }

        [TestMethod]
        public void GetEntities_Large()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.LargeCount, new TestComponent1());
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();

            var getEntities = query.GetEntities();
            Assert.IsTrue(getEntities.Length == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(getEntities[i] == entities[i],
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void GetEntities_Single()
        {
            var entity = TestCreateEntities(Context, 1, new TestComponent1())[0];
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();

            var entities = query.GetEntities();
            Assert.IsTrue(entities.Length == 1);
            Assert.IsTrue(entities[0] == entity);
        }

        [TestMethod]
        public void HasEntity_Destroyed()
        {
            var query = Context.QueryManager.CreateQuery();
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                query.HasEntity(Entity.Null));
        }

        [TestMethod]
        public void HasEntity_Different_ArcheTypes()
        {
            var entity1 = TestCreateEntities(Context, 1,
                new TestComponent1())[0];
            var entity2 = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestComponent2())[0];
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(query.GetEntities().Length == 2);
            Assert.IsTrue(query.HasEntity(entity1));
            Assert.IsTrue(query.HasEntity(entity2));
        }

        [TestMethod]
        public void HasEntity_Different_ArcheTypes_Add()
        {
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();
            Assert.IsTrue(query.GetEntities().Length == 0);

            var entity1 = TestCreateEntities(Context, 1,
                new TestComponent1())[0];
            Assert.IsTrue(query.GetEntities().Length == 1);
            Assert.IsTrue(query.HasEntity(entity1));

            var entity2 = TestCreateEntities(Context, 1,
                new TestComponent1(),
                new TestComponent2())[0];

            Assert.IsTrue(query.GetEntities().Length == 2);
            Assert.IsTrue(query.HasEntity(entity1));
            Assert.IsTrue(query.HasEntity(entity2));
        }

        [TestMethod]
        public void HasEntity_Large()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.LargeCount, new TestComponent1());
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(query.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void HasEntity_Single()
        {
            var entity = TestCreateEntities(Context, 1, new TestComponent1())[0];
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(query.HasEntity(entity));
            Assert.IsFalse(query.HasEntity(Entity.Null));
        }
    }
}
