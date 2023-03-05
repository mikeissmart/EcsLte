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
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAllOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAllOf()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAllOf<TestComponent1>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 1);
            Assert.IsTrue(filter.AllOfConfigs[0] == ComponentConfig<TestComponent1>.Config);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 0);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereAnyOf<TestComponent1>());
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereNoneOf<TestComponent1>());

            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            filter = Context.Filters
                .WhereAllOf(archeType);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereAnyOf(archeType));
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAllOfException>(() =>
                filter.WhereNoneOf(archeType));

            Assert.IsTrue(filter.HasWhereAllOf<TestComponent2>());
            Assert.IsTrue(filter.HasWhereAllOf<TestComponent3>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 2);
            Assert.IsTrue(filter.AllOfConfigs.Where(x => x == ComponentConfig<TestComponent2>.Config).Count() == 1);
            Assert.IsTrue(filter.AllOfConfigs.Where(x => x == ComponentConfig<TestComponent3>.Config).Count() == 1);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 0);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.WhereAllOf(x)
                });
        }

        [TestMethod]
        public void HasWhereAnyOf()
        {
            var filter = Context.Filters
                .WhereAnyOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereAnyOf()
        {
            var filter = Context.Filters
                .WhereAnyOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent1>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 0);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 1);
            Assert.IsTrue(filter.AnyOfConfigs[0] == ComponentConfig<TestComponent1>.Config);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereNoneOf<TestComponent1>());

            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            filter = Context.Filters
                .WhereAnyOf(archeType);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereAllOf(archeType));
            Assert.ThrowsException<EntityFilterAlreadyHasWhereAnyOfException>(() =>
                filter.WhereNoneOf(archeType));

            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent2>());
            Assert.IsTrue(filter.HasWhereAnyOf<TestComponent3>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 0);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 2);
            Assert.IsTrue(filter.AnyOfConfigs.Where(x => x == ComponentConfig<TestComponent2>.Config).Count() == 1);
            Assert.IsTrue(filter.AnyOfConfigs.Where(x => x == ComponentConfig<TestComponent3>.Config).Count() == 1);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.WhereAnyOf(x)
                });
        }

        [TestMethod]
        public void HasWhereNoneOf()
        {
            var filter = Context.Filters
                .WhereNoneOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent1>());
        }

        [TestMethod]
        public void WhereNoneOf()
        {
            var filter = Context.Filters
                .WhereNoneOf<TestComponent1>();

            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent1>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 0);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 0);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 1);
            Assert.IsTrue(filter.NoneOfConfigs[0] == ComponentConfig<TestComponent1>.Config);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAllOf<TestComponent1>());
            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAnyOf<TestComponent1>());

            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            filter = Context.Filters
                .WhereNoneOf(archeType);

            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAllOf(archeType));
            Assert.ThrowsException<EntityFilterAlreadyHasWhereNoneOfException>(() =>
                filter.WhereAnyOf(archeType));

            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent2>());
            Assert.IsTrue(filter.HasWhereNoneOf<TestComponent3>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 0);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 0);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 2);
            Assert.IsTrue(filter.NoneOfConfigs.Where(x => x == ComponentConfig<TestComponent2>.Config).Count() == 1);
            Assert.IsTrue(filter.NoneOfConfigs.Where(x => x == ComponentConfig<TestComponent3>.Config).Count() == 1);
            Assert.IsTrue(filter.FilterByComponents.Length == 0);

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.WhereNoneOf(x)
                });
        }

        [TestMethod]
        public void Multi_WhereAllAnyNoneOf()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>()
                .WhereAnyOf<TestComponent2>()
                .WhereNoneOf<TestComponent3>();

            var archeType1 = Context.ArcheTypes
                .AddComponentType<TestComponent1>();
            var archeType2 = Context.ArcheTypes
                .AddComponentType<TestComponent2>();
            var archeType3 = Context.ArcheTypes
                .AddComponentType<TestComponent3>();
            var archeType12 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>();
            var archeType23 = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            var archeType123 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();

            var entity1 = Context.Entities.CreateEntity(archeType1);
            var entity2 = Context.Entities.CreateEntity(archeType2);
            var entity3 = Context.Entities.CreateEntity(archeType3);
            var entity12 = Context.Entities.CreateEntity(archeType12);
            var entity23 = Context.Entities.CreateEntity(archeType23);
            var entity123 = Context.Entities.CreateEntity(archeType123);

            var entities = Context.Entities.GetEntities(filter);

            Assert.IsTrue(!entities.Any(x => x.Id == entity1.Id));      // No TestComponent2
            Assert.IsTrue(!entities.Any(x => x.Id == entity2.Id));      // No TestComponent1
            Assert.IsTrue(!entities.Any(x => x.Id == entity3.Id));      // Has TestCommponent3
            Assert.IsTrue(entities.Any(x => x.Id == entity12.Id));      // Ok
            Assert.IsTrue(!entities.Any(x => x.Id == entity23.Id));     // No TestComponent1, Has TestComponent3
            Assert.IsTrue(!entities.Any(x => x.Id == entity123.Id));    // Has TestComponent3
        }

        [TestMethod]
        public void Multi_WhereAllAnyOf()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>()
                .WhereAnyOf<TestComponent2>();

            var archeType1 = Context.ArcheTypes
                .AddComponentType<TestComponent1>();
            var archeType2 = Context.ArcheTypes
                .AddComponentType<TestComponent2>();
            var archeType3 = Context.ArcheTypes
                .AddComponentType<TestComponent3>();
            var archeType12 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>();
            var archeType23 = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            var archeType123 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();

            var entity1 = Context.Entities.CreateEntity(archeType1);
            var entity2 = Context.Entities.CreateEntity(archeType2);
            var entity3 = Context.Entities.CreateEntity(archeType3);
            var entity12 = Context.Entities.CreateEntity(archeType12);
            var entity23 = Context.Entities.CreateEntity(archeType23);
            var entity123 = Context.Entities.CreateEntity(archeType123);

            var entities = Context.Entities.GetEntities(filter);

            Assert.IsTrue(!entities.Any(x => x.Id == entity1.Id));      // No TestComponent2
            Assert.IsTrue(!entities.Any(x => x.Id == entity2.Id));      // No TestComponent1
            Assert.IsTrue(!entities.Any(x => x.Id == entity3.Id));      // No TestComponent1 or TestComponent2
            Assert.IsTrue(entities.Any(x => x.Id == entity12.Id));      // Ok
            Assert.IsTrue(!entities.Any(x => x.Id == entity23.Id));     // No TestComponent1
            Assert.IsTrue(entities.Any(x => x.Id == entity123.Id));     // Ok
        }

        [TestMethod]
        public void Multi_WhereAllNoneOf()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>()
                .WhereNoneOf<TestComponent3>();

            var archeType1 = Context.ArcheTypes
                .AddComponentType<TestComponent1>();
            var archeType2 = Context.ArcheTypes
                .AddComponentType<TestComponent2>();
            var archeType3 = Context.ArcheTypes
                .AddComponentType<TestComponent3>();
            var archeType12 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>();
            var archeType23 = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            var archeType123 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();

            var entity1 = Context.Entities.CreateEntity(archeType1);
            var entity2 = Context.Entities.CreateEntity(archeType2);
            var entity3 = Context.Entities.CreateEntity(archeType3);
            var entity12 = Context.Entities.CreateEntity(archeType12);
            var entity23 = Context.Entities.CreateEntity(archeType23);
            var entity123 = Context.Entities.CreateEntity(archeType123);

            var entities = Context.Entities.GetEntities(filter);

            Assert.IsTrue(entities.Any(x => x.Id == entity1.Id));       // Ok
            Assert.IsTrue(!entities.Any(x => x.Id == entity2.Id));      // No TestComponent1
            Assert.IsTrue(!entities.Any(x => x.Id == entity3.Id));      // Has TestComponent3
            Assert.IsTrue(entities.Any(x => x.Id == entity12.Id));      // Ok
            Assert.IsTrue(!entities.Any(x => x.Id == entity23.Id));     // Has TestComponent3
            Assert.IsTrue(!entities.Any(x => x.Id == entity123.Id));    // Has TestComponent3
        }

        [TestMethod]
        public void Multi_WhereAnyNoneOf()
        {
            var filter = Context.Filters
                .WhereAnyOf<TestComponent2>()
                .WhereNoneOf<TestComponent3>();

            var archeType1 = Context.ArcheTypes
                .AddComponentType<TestComponent1>();
            var archeType2 = Context.ArcheTypes
                .AddComponentType<TestComponent2>();
            var archeType3 = Context.ArcheTypes
                .AddComponentType<TestComponent3>();
            var archeType12 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>();
            var archeType23 = Context.ArcheTypes
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();
            var archeType123 = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>()
                .AddComponentType<TestComponent3>();

            var entity1 = Context.Entities.CreateEntity(archeType1);
            var entity2 = Context.Entities.CreateEntity(archeType2);
            var entity3 = Context.Entities.CreateEntity(archeType3);
            var entity12 = Context.Entities.CreateEntity(archeType12);
            var entity23 = Context.Entities.CreateEntity(archeType23);
            var entity123 = Context.Entities.CreateEntity(archeType123);

            var entities = Context.Entities.GetEntities(filter);

            Assert.IsTrue(!entities.Any(x => x.Id == entity1.Id));      // No TestComponent2
            Assert.IsTrue(entities.Any(x => x.Id == entity2.Id));       // Ok
            Assert.IsTrue(!entities.Any(x => x.Id == entity3.Id));      // Has TestCommponent3
            Assert.IsTrue(entities.Any(x => x.Id == entity12.Id));      // Ok
            Assert.IsTrue(!entities.Any(x => x.Id == entity23.Id));     // Has TestComponent3
            Assert.IsTrue(!entities.Any(x => x.Id == entity123.Id));    // Has TestComponent3
        }

        [TestMethod]
        public void HasFilterBy()
        {
            var filter = Context.Filters
                .FilterBy(new TestSharedComponent1());

            Assert.IsTrue(filter.HasFilterBy<TestSharedComponent1>());
        }

        [TestMethod]
        public void GetFilterBy()
        {
            var filter = Context.Filters
                .FilterBy(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(filter.GetFilterBy<TestSharedComponent1>().Prop == 1);

            Assert.ThrowsException<EntityFilterNotFilterByException>(() =>
                filter.GetFilterBy<TestSharedComponent2>());
        }

        [TestMethod]
        public void FilterBy()
        {
            var filter = Context.Filters
                .FilterBy(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(filter.HasWhereAllOf<TestSharedComponent1>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 1);
            Assert.IsTrue(filter.AllOfConfigs[0] == ComponentConfig<TestSharedComponent1>.Config);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 0);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Length == 1);
            Assert.IsTrue(filter.FilterByComponents[0].Equals(new TestSharedComponent1 { Prop = 1 }));

            var archeType = Context.ArcheTypes
                .AddSharedComponent(new TestSharedComponent2 { Prop = 2 })
                .AddSharedComponent(new TestSharedComponent3 { Prop = 3 });
            filter = Context.Filters
                .FilterBy(archeType);

            Assert.IsTrue(filter.HasWhereAllOf<TestSharedComponent2>());
            Assert.IsTrue(filter.HasWhereAllOf<TestSharedComponent3>());
            Assert.IsTrue(filter.AllOfConfigs.Length == 2);
            Assert.IsTrue(filter.AllOfConfigs.Where(x => x == ComponentConfig<TestSharedComponent2>.Config).Count() == 1);
            Assert.IsTrue(filter.AllOfConfigs.Where(x => x == ComponentConfig<TestSharedComponent3>.Config).Count() == 1);
            Assert.IsTrue(filter.AnyOfConfigs.Length == 0);
            Assert.IsTrue(filter.NoneOfConfigs.Length == 0);
            Assert.IsTrue(filter.FilterByComponents.Where(x => x.Equals(new TestSharedComponent2 { Prop = 2 })).Count() == 1);
            Assert.IsTrue(filter.FilterByComponents.Where(x => x.Equals(new TestSharedComponent3 { Prop = 3 })).Count() == 1);

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => filter.FilterBy(x)
                });
        }
    }
}
