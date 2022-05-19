using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTest_WhereOf : BasePrePostTest
    {
        [TestMethod]
        public void WhereAllOf_Different_WhereOfs()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAnyOf<TestComponent1>());
            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_Duplicate()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAllOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_Duplicate_Distinct()
        {
            var query = new EntityQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.WhereAllOf<TestComponent1, TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_Different_WhereOfs()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_Duplicate()
        {
            var query = new EntityQuery()
                .WhereAnyOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAnyOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_Duplicate_Distinct()
        {
            var query = new EntityQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.WhereAnyOf<TestComponent1, TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_Different_WhereOfs()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereAnyOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_Duplicate()
        {
            var query = new EntityQuery()
                .WhereNoneOf<TestComponent1>();

            Assert.ThrowsException<EntityQueryAlreadyHasWhereException>(() =>
                query.WhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_Duplicate_Distinct()
        {
            var query = new EntityQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.WhereNoneOf<TestComponent1, TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_x1()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(query.AllComponentTypes.Length == 1);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_x2()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1, TestComponent2>();

            Assert.IsTrue(query.AllComponentTypes.Length == 2);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());
        }

        [TestMethod]
        public void WhereAllOf_x3()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestSharedComponent1>();

            Assert.IsTrue(query.AllComponentTypes.Length == 3);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestSharedComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_x4()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            Assert.IsTrue(query.AllComponentTypes.Length == 4);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestSharedComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());

            Assert.IsTrue(query.AllComponentTypes.Any(x => x == typeof(TestSharedComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent2>());
        }

        [TestMethod]
        public void WhereAnyOf_x1()
        {
            var query = new EntityQuery()
                .WhereAnyOf<TestComponent1>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 1);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_x2()
        {
            var query = new EntityQuery()
                .WhereAnyOf<TestComponent1, TestComponent2>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 2);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());
        }

        [TestMethod]
        public void WhereAnyOf_x3()
        {
            var query = new EntityQuery()
                .WhereAnyOf<TestComponent1, TestComponent2, TestSharedComponent1>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 3);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestSharedComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_x4()
        {
            var query = new EntityQuery()
                .WhereAnyOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 4);
            Assert.IsTrue(query.NoneComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestSharedComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());

            Assert.IsTrue(query.AnyComponentTypes.Any(x => x == typeof(TestSharedComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent2>());
        }

        [TestMethod]
        public void WhereNoneOf_x1()
        {
            var query = new EntityQuery()
                .WhereNoneOf<TestComponent1>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 1);
            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_x2()
        {
            var query = new EntityQuery()
                .WhereNoneOf<TestComponent1, TestComponent2>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 2);
            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent2>());
        }

        [TestMethod]
        public void WhereNoneOf_x3()
        {
            var query = new EntityQuery()
                .WhereNoneOf<TestComponent1, TestComponent2, TestSharedComponent1>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 3);
            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestSharedComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestSharedComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_x4()
        {
            var query = new EntityQuery()
                .WhereNoneOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            Assert.IsTrue(query.AllComponentTypes.Length == 0);
            Assert.IsTrue(query.AnyComponentTypes.Length == 0);
            Assert.IsTrue(query.NoneComponentTypes.Length == 4);
            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestSharedComponent1)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestSharedComponent1>());

            Assert.IsTrue(query.NoneComponentTypes.Any(x => x == typeof(TestSharedComponent2)));
            Assert.IsTrue(query.HasWhereOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestSharedComponent2>());
        }
    }
}
