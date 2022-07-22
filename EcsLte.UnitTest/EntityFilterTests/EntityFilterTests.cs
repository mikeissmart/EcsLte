using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityFilterTests
{
    [TestClass]
    public class EntityFilterTests : BasePrePostTest
    {
        [TestMethod]
        public void HasWhereAllOf()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAllOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 1);
            Assert.IsTrue(filter.AllOfComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereAnyOf<TestComponent1>());
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereNoneOf<TestComponent1>());

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            filter = new EntityFilter()
                .WhereAllOf(archeType);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereAnyOf(archeType));
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereNoneOf(archeType));

            Assert.IsTrue(filter.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(filter.HasWhereAllOf<TestComponent3>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 2);
            Assert.IsTrue(filter.AllOfComponentTypes.Where(x => x == typeof(TestComponent2)).Count() == 1);
            Assert.IsTrue(filter.AllOfComponentTypes.Where(x => x == typeof(TestComponent3)).Count() == 1);
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.WhereAllOf(x)
                });
        }

        [TestMethod]
        public void HasWhereAnyOf()
        {
            var filter = new EntityFilter()
                .WhereAnyOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf()
        {
            var filter = new EntityFilter()
                .WhereAnyOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 1);
            Assert.IsTrue(filter.AnyOfComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereNoneOf<TestComponent1>());

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            filter = new EntityFilter()
                .WhereAnyOf(archeType);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereAllOf(archeType));
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereNoneOf(archeType));

            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent3>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 2);
            Assert.IsTrue(filter.AnyOfComponentTypes.Where(x => x == typeof(TestComponent2)).Count() == 1);
            Assert.IsTrue(filter.AnyOfComponentTypes.Where(x => x == typeof(TestComponent3)).Count() == 1);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.WhereAnyOf(x)
                });
        }

        [TestMethod]
        public void HasWhereNoneOf()
        {
            var filter = new EntityFilter()
                .WhereNoneOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf()
        {
            var filter = new EntityFilter()
                .WhereNoneOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent1>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 1);
            Assert.IsTrue(filter.NoneOfComponentTypes[0] == typeof(TestComponent1));
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAnyOf<TestComponent1>());

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            filter = new EntityFilter()
                .WhereNoneOf(archeType);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAllOf(archeType));
            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAnyOf(archeType));

            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent2>());
            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent3>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 2);
            Assert.IsTrue(filter.NoneOfComponentTypes.Where(x => x == typeof(TestComponent2)).Count() == 1);
            Assert.IsTrue(filter.NoneOfComponentTypes.Where(x => x == typeof(TestComponent3)).Count() == 1);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.WhereNoneOf(x)
                });
        }

        [TestMethod]
        public void HasFilterBy()
        {
            var filter = new EntityFilter()
                .FilterBy(new TestSharedComponent1());

            Assert.IsTrue(filter.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void GetFilterBy()
        {
            var filter = new EntityFilter()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(filter.GetFilterBy<TestSharedComponent1>().Prop == 1);

            Assert.ThrowsException<EntityFilterNotFilterByException>(() =>
                filter.GetFilterBy<TestSharedComponent2>());
        }

        [TestMethod]
        public void FilterBy()
        {
            var filter = new EntityFilter()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(filter.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 1);
            Assert.IsTrue(filter.AllOfComponentTypes[0] == typeof(TestSharedComponent1));
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 1);
            Assert.IsTrue(filter.FilterByComponents[0].Equals(new TestSharedComponent1 { Prop = 1 }));

            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent2 { Prop = 2 })
                .AddSharedComponent(new TestSharedComponent3 { Prop = 3 });
            filter = new EntityFilter()
                .FilterBy(archeType);

            Assert.IsTrue(filter.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsTrue(filter.HasWhereAllOf<TestSharedComponent3>());
            Assert.IsTrue(filter.AllOfComponentTypes.Length == 2);
            Assert.IsTrue(filter.AllOfComponentTypes.Where(x => x == typeof(TestSharedComponent2)).Count() == 1);
            Assert.IsTrue(filter.AllOfComponentTypes.Where(x => x == typeof(TestSharedComponent3)).Count() == 1);
            Assert.IsTrue(filter.AnyOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.NoneOfComponentTypes.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Where(x => x.Equals(new TestSharedComponent2 { Prop = 2 })).Count() == 1);
            Assert.IsTrue(filter.FilterByComponents.Where(x => x.Equals(new TestSharedComponent3 { Prop = 3 })).Count() == 1);

            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.FilterBy(x)
                });
        }
    }
}
