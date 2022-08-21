using BenchmarkDotNet.Attributes;
using System;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EntityQueryForEachTests
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityQuery _query;
        private EntityCommands _commands;

        [ParamsAllValues]
        public ReadWriteType ReadWrite { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
            _query = _context.Queries
                .SetTracker(_context.Tracking.CreateTracker("Tracker")
                    .SetTrackingState<TestComponent1>(TrackingState.Added)
                    .SetTrackingState<TestComponent2>(TrackingState.Added)
                    .SetTrackingState<TestComponent3>(TrackingState.Added)
                    .SetTrackingState<TestComponent4>(TrackingState.Added)
                    .SetTrackingState<TestSharedComponent1>(TrackingState.Added)
                    .SetTrackingState<TestSharedComponent2>(TrackingState.Added)
                    .SetTrackingState<TestSharedComponent3>(TrackingState.Added)
                    .SetTrackingState<TestSharedComponent4>(TrackingState.Added)
                    .SetTrackingState<TestManagedComponent1>(TrackingState.Added)
                    .SetTrackingState<TestManagedComponent2>(TrackingState.Added)
                    .SetTrackingState<TestManagedComponent3>(TrackingState.Added)
                    .SetTrackingState<TestManagedComponent4>(TrackingState.Added)
                    .StartTracking());
            _commands = _context.Commands.CreateCommands("Commands");
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.DestroyContext(_context);
        }

        [IterationSetup(Target = "ForEach")]
        public void IterationSetup_ForEach()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(ReadWrite);
            _query.SetFilter(_context.Filters
                .WhereAnyOf(blueprint.GetArcheType(_context)));
            _entities = _context.Entities.CreateEntities(blueprint, BenchmarkTestConsts.LargeCount);
        }

        [IterationCleanup(Target = "ForEach")]
        public void IterationCleanup_ForEach()
        {
            _query.Tracker.ClearEntities();
            _context.Entities.DestroyEntities(_entities);
        }

        [Benchmark]
        public void ForEach()
        {
            switch (ReadWrite)
            {
                case ReadWriteType.R0W0:
                    _query.ForEach(
                        (int index, Entity entity) =>
                        {
                        })
                        .Run();
                    break;

                #region Normal

                case ReadWriteType.R0W4_Normal_x4:
                    _query.ForEach(
                        (int index, Entity entity, ref TestComponent1 component1, ref TestComponent2 component2, ref TestComponent3 component3, ref TestComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        })
                        .Run();
                    break;

                case ReadWriteType.R4W0_Normal_x4:
                    _query.ForEach(
                        (int index, Entity entity, in TestComponent1 component1, in TestComponent2 component2, in TestComponent3 component3, in TestComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        })
                        .Run();
                    break;

                #endregion Normal

                #region Managed

                case ReadWriteType.R0W4_Managed_x4:
                    _query.ForEach(
                        (int index, Entity entity, ref TestManagedComponent1 component1, ref TestManagedComponent2 component2, ref TestManagedComponent3 component3, ref TestManagedComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        })
                        .Run();
                    break;

                case ReadWriteType.R4W0_Managed_x4:
                    _query.ForEach(
                        (int index, Entity entity, in TestManagedComponent1 component1, in TestManagedComponent2 component2, in TestManagedComponent3 component3, in TestManagedComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        })
                        .Run();
                    break;

                #endregion Managed

                #region Shared

                case ReadWriteType.R0W4_Shared_x4:
                    _query.ForEach(
                        (int index, Entity entity, ref TestSharedComponent1 component1, ref TestSharedComponent2 component2, ref TestSharedComponent3 component3, ref TestSharedComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        })
                        .Run();
                    break;

                case ReadWriteType.R4W0_Shared_x4:
                    _query.ForEach(
                        (int index, Entity entity, in TestSharedComponent1 component1, in TestSharedComponent2 component2, in TestSharedComponent3 component3, in TestSharedComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        })
                        .Run();
                    break;

                #endregion Shared

                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public enum ReadWriteType
    {
        R0W0,

        R0W4_Normal_x4,
        R4W0_Normal_x4,

        R0W4_Managed_x4,
        R4W0_Managed_x4,

        R0W4_Shared_x4,
        R4W0_Shared_x4,
    }
}
