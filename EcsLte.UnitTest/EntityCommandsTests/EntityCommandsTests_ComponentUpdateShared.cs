using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandsTests
{
    [TestClass]
    public class EntityCommandsTests_ComponentUpdateShared : BasePrePostTest
    {
        [TestMethod]
        public void UpdateComponent()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 }),
                EntityState.Active);

            commands.UpdateSharedComponent(entity, new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 0);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(entity).Prop == 1);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateSharedComponent(entity, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponent_ArcheType()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var archeType1 = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });
            var archeType2 = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });
            var orgArcheType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Context.Entities.CreateEntities(
                archeType1,
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.UpdateSharedComponent(archeType1, new TestSharedComponent1 { Prop = 2 });

            Assert.IsTrue(Context.Entities.EntityCount(archeType1) == UnitTestConsts.SmallCount);

            archeType1.AddComponentType<TestComponent2>();

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.EntityCount(archeType1) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(archeType2) == UnitTestConsts.SmallCount);
            Assert.IsTrue(Context.Entities.EntityCount(orgArcheType) == 0);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                commands.UpdateSharedComponent(archeType1, new TestSharedComponent1()));
        }

        [TestMethod]
        public void UpdateSharedComponents_Filter()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var filter1 = new EntityFilter()
                .WhereAllOf<TestComponent1>()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });
            var filter2 = new EntityFilter()
                .WhereAllOf<TestComponent1>()
                .FilterBy(new TestSharedComponent1 { Prop = 2 });
            var orgFilter = new EntityFilter()
                .WhereAllOf<TestComponent1>()
                .FilterBy(new TestSharedComponent1 { Prop = 1 });

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.UpdateSharedComponents(filter1, new TestSharedComponent1 { Prop = 2 });

            Assert.IsTrue(Context.Entities.EntityCount(filter1) == UnitTestConsts.SmallCount);

            filter1.FilterBy(new TestSharedComponent1 { Prop = 3 });

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.EntityCount(filter1) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(filter2) == UnitTestConsts.SmallCount);
            Assert.IsTrue(Context.Entities.EntityCount(orgFilter) == 0);
        }

        [TestMethod]
        public void UpdateSharedComponents_Tracker()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestSharedComponent1>(EntityTrackerState.Added)
                .StartTracking();

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.UpdateSharedComponents(tracker, new TestSharedComponent1 { Prop = 2 });

            Assert.IsTrue(Context.Entities.EntityCount(tracker) == UnitTestConsts.SmallCount);

            tracker.ClearComponentState<TestComponent1>()
                .SetComponentState<TestComponent2>(EntityTrackerState.Added);
            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddSharedComponent(new TestSharedComponent2 { Prop = 2 }),
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.EntityCount(tracker) == UnitTestConsts.SmallCount);
        }

        [TestMethod]
        public void UpdateSharedComponents_Query()
        {
            var commands = Context.Commands.CreateCommands("Commands");

            var queryFilter1 = new EntityQuery(Context,
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()
                    .FilterBy(new TestSharedComponent1 { Prop = 1 }));
            var queryFilter2 = new EntityQuery(Context,
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()
                    .FilterBy(new TestSharedComponent1 { Prop = 2 }));
            var orgQueryFilter = new EntityQuery(Context,
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()
                    .FilterBy(new TestSharedComponent1 { Prop = 1 }));

            Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 1 }),
                EntityState.Creating,
                UnitTestConsts.SmallCount);

            commands.UpdateSharedComponents(queryFilter1, new TestSharedComponent1 { Prop = 2 });

            Assert.IsTrue(Context.Entities.EntityCount(queryFilter1) == UnitTestConsts.SmallCount);

            queryFilter1.Filter.FilterBy(new TestSharedComponent1 { Prop = 3 });

            commands.ExecuteCommands();

            Assert.IsTrue(Context.Entities.EntityCount(queryFilter1) == 0);
            Assert.IsTrue(Context.Entities.EntityCount(queryFilter2) == UnitTestConsts.SmallCount);
            Assert.IsTrue(Context.Entities.EntityCount(orgQueryFilter) == 0);
        }
    }
}
