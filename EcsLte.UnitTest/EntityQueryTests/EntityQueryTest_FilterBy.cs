using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTest_FilterBy : BasePrePostTest
    {
        [TestMethod]
        public void FilterBy_Duplicate()
        {
            var query = new EntityQuery()
                .FilterBy(new TestSharedComponent1());

            Assert.ThrowsException<EntityQueryAlreadyFilteredByException>(() =>
                query.FilterBy(new TestSharedComponent1()));
        }

        [TestMethod]
        public void FilterBy_Duplicate_Distinct()
        {
            var query = new EntityQuery();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.FilterBy(new TestSharedComponent1(), new TestSharedComponent1()));
        }

        [TestMethod]
        public void FilterBy_GetFilterBy_Multiple()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            var query = new EntityQuery()
                .FilterBy(component1, component2);

            Assert.IsTrue(query.FilterComponents.Length == 2);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent2>().Prop == component2.Prop);
        }

        [TestMethod]
        public void FilterBy_GetFilterBy_Single()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var query = new EntityQuery()
                .FilterBy(component1);

            Assert.IsTrue(query.FilterComponents.Length == 1);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void FilterBy_EntityArcheType()
        {
            var archeType = new EntityArcheType()
                .AddSharedComponent(new TestSharedComponent1());
            var query = new EntityQuery()
                .FilterBy(archeType);

            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void FilterBy_WhereAllOf()
        {
            var query = new EntityQuery()
                .WhereAllOf<TestSharedComponent1>()
                .FilterBy(new TestSharedComponent1());

            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void FilterBy_WhereAllOf_None()
        {
            var query = new EntityQuery()
                .FilterBy(new TestSharedComponent1());

            Assert.IsTrue(query.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(query.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void FilterByReplace_EntityArcheType()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            var archeType = new EntityArcheType()
                .AddSharedComponent(component1)
                .AddSharedComponent(component2);
            var query = new EntityQuery()
                .FilterBy(new TestSharedComponent1(), new TestSharedComponent2())
                .FilterByReplace(archeType);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent2>().Prop == component2.Prop);
        }

        [TestMethod]
        public void FilterByReplace_Multiple()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            var query = new EntityQuery()
                .FilterBy(new TestSharedComponent1(), new TestSharedComponent2())
                .FilterByReplace(component1, component2);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
            Assert.IsTrue(query.GetFilterBy<TestSharedComponent2>().Prop == component2.Prop);
        }

        [TestMethod]
        public void FilterByReplace_Single()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var query = new EntityQuery()
                .FilterBy(new TestSharedComponent1())
                .FilterByReplace(component1);

            Assert.IsTrue(query.GetFilterBy<TestSharedComponent1>().Prop == component1.Prop);
        }
    }
}