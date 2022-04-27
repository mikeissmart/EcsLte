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
    public class EntityQuery_WhereOfTest : BasePrePostTest
    {
        [TestMethod]
        public void WhereAllOf_x1()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_x2()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1, TestComponent2>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());
        }

        [TestMethod]
        public void WhereAllOf_x3()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestSharedComponent1>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());
        }

        [TestMethod]
        public void WhereAllOf_x4()
        {
            var query = Context.CreateQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent2>());
            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent2>());
        }

        [TestMethod]
        public void WhereAnyOf_x1()
        {
            var query = Context.CreateQuery()
                .WhereAnyOf<TestComponent1>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_x2()
        {
            var query = Context.CreateQuery()
                .WhereAnyOf<TestComponent1, TestComponent2>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());
        }

        [TestMethod]
        public void WhereAnyOf_x3()
        {
            var query = Context.CreateQuery()
                .WhereAnyOf<TestComponent1, TestComponent2, TestSharedComponent1>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf_x4()
        {
            var query = Context.CreateQuery()
                .WhereAnyOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsTrue(query.HasWhereAnyOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereNoneOf<TestSharedComponent2>());
        }

        [TestMethod]
        public void WhereNoneOf_x1()
        {
            var query = Context.CreateQuery()
                .WhereNoneOf<TestComponent1>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_x2()
        {
            var query = Context.CreateQuery()
                .WhereNoneOf<TestComponent1, TestComponent2>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent2>());
        }

        [TestMethod]
        public void WhereNoneOf_x3()
        {
            var query = Context.CreateQuery()
                .WhereNoneOf<TestComponent1, TestComponent2, TestSharedComponent1>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestSharedComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf_x4()
        {
            var query = Context.CreateQuery()
                .WhereNoneOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            Assert.IsTrue(query.HasWhereOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestComponent2>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasWhereNoneOf<TestSharedComponent1>());

            Assert.IsTrue(query.HasWhereOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsFalse(query.HasWhereAnyOf<TestSharedComponent2>());
            Assert.IsTrue(query.HasWhereNoneOf<TestSharedComponent2>());
        }
    }
}
