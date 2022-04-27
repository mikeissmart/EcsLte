using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQuery_FunctionTest : BasePrePostTest
    {
        [TestMethod]
        public void FilterBy_Duplicate()
        {
            var query = Context.CreateQuery()
                .FilterBy(new TestSharedComponent1());

            Assert.ThrowsException<EntityQueryAlreadyFilteredByException>(() =>
                query.FilterBy(new TestSharedComponent1()));
        }

        [TestMethod]
        public void FilterBy_Duplicate_Distinct()
        {
            var query = Context.CreateQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.FilterBy(new TestSharedComponent1(), new TestSharedComponent1()));
        }

        [TestMethod]
        public void FilterBy_GetFilterBy_Multiple()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            var query = Context.CreateQuery()
                .FilterBy(component1, component2);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent2>().Prop == component2.Prop);
        }

        [TestMethod]
        public void FilterBy_GetFilterBy_Single()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var query = Context.CreateQuery()
                .FilterBy(component1);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void FilterBy_WhereAllOf()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestSharedComponent1>()
                .FilterBy(new TestSharedComponent1());

            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void FilterBy_WhereAllOf_None()
        {
            var query = Context.CreateQuery()
                .FilterBy(new TestSharedComponent1());

            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void FilterByReplace_Large()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            var query = Context.CreateQuery()
                .FilterBy(new TestSharedComponent1(), new TestSharedComponent2())
                .FilterByReplace(component1, component2);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent2>().Prop == component2.Prop);
        }

        [TestMethod]
        public void FilterByReplace_Single()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var query = Context.CreateQuery()
                .FilterBy(new TestSharedComponent1())
                .FilterByReplace(component1);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void GetEntities_Destroyed()
        {
            var query = Context.CreateQuery();
            EcsContexts.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                query.GetEntities());
        }

        [TestMethod]
        public void GetEntities_Large()
        {
            var entities = TestCreateEntities(Context, UnitTestConsts.LargeCount, new TestComponent1());
            var query = Context.CreateQuery()
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
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            var entities = query.GetEntities();
            Assert.IsTrue(entities.Length == 1);
            Assert.IsTrue(entities[0] == entity);
        }

        [TestMethod]
        public void HasEntity_Destroyed()
        {
            var query = Context.CreateQuery();
            EcsContexts.DestroyContext(Context);

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
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(query.GetEntities().Length == 2);
            Assert.IsTrue(query.HasEntity(entity1));
            Assert.IsTrue(query.HasEntity(entity2));
        }

        [TestMethod]
        public void HasEntity_Different_ArcheTypes_Add()
        {
            var query = Context.CreateQuery()
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
            var query = Context.CreateQuery()
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
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(query.HasEntity(entity));
        }

        [TestMethod]
        public void WhereAllOf_Different_WhereOfs()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAnyOf<TestComponent1>());
            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_Duplicate()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAllOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_Duplicate_Distinct()
        {
            var query = Context.CreateQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.WhereAllOf<TestComponent1, TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_Different_WhereOfs()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_Duplicate()
        {
            var query = Context.CreateQuery()
                .WhereAnyOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAnyOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_Duplicate_Distinct()
        {
            var query = Context.CreateQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.WhereAnyOf<TestComponent1, TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_Different_WhereOfs()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAnyOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_Duplicate()
        {
            var query = Context.CreateQuery()
                .WhereNoneOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_Duplicate_Distinct()
        {
            var query = Context.CreateQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.WhereNoneOf<TestComponent1, TestComponent1>());
        }
    }
}
