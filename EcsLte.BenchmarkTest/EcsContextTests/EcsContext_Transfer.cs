using BenchmarkDotNet.Attributes;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_Transfer
    {
        private EcsContext _sourceContext;
        private EcsContext _destContext;
        private Entity[] _sourceEntities;
        private Entity[] _destEntities;
        private EntityArcheType _archeType;
        private EntityQuery _query;
        private EntityTracker _tracker;

        [ParamsAllValues]
        public ComponentArrangement CompArr { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.Instance.HasContext("Source"))
                EcsContexts.Instance.DestroyContext(EcsContexts.Instance.GetContext("Source"));
            _sourceContext = EcsContexts.Instance.CreateContext("Source");
            if (EcsContexts.Instance.HasContext("Dest"))
                EcsContexts.Instance.DestroyContext(EcsContexts.Instance.GetContext("Dest"));
            _destContext = EcsContexts.Instance.CreateContext("Dest");
            _destEntities = new Entity[BenchmarkTestConsts.LargeCount];
            _tracker = _sourceContext.Tracking.CreateTracker("Tracker")
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
                .StartTracking();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_sourceContext.IsDestroyed)
                EcsContexts.Instance.DestroyContext(_sourceContext);
            if (!_destContext.IsDestroyed)
                EcsContexts.Instance.DestroyContext(_destContext);
        }

        [IterationSetup()]
        public void IterationSetup_Create()
        {
            _archeType = EcsContextSetupCleanup.CreateBlueprint(CompArr)
                .GetArcheType(_sourceContext);
            _query = _sourceContext.Queries
                .SetFilter(_sourceContext.Filters
                    .WhereAllOf(_archeType))
                .SetTracker(_tracker);

            _sourceEntities = _sourceContext.Entities.CreateEntities(
                EcsContextSetupCleanup.CreateBlueprint(CompArr),
                BenchmarkTestConsts.LargeCount);
        }

        [IterationCleanup()]
        public void IterationCleanup_Create()
        {
            _sourceContext.Entities.DestroyEntities(
                _sourceEntities);
            _destContext.Entities.DestroyEntities(
                _destEntities);
            _tracker.ClearEntities();
        }

        [Benchmark]
        public void TransferEntity_NoDestroy()
        {
            for (var i = 0; i < _sourceEntities.Length; i++)
                _destEntities[i] = _destContext.Entities.CopyEntityTo(_sourceContext.Entities, _sourceEntities[i]);
        }

        [Benchmark]
        public void TransferEntities_NoDestroy() =>
            _destEntities = _destContext.Entities.CopyEntitiesTo(_sourceContext.Entities, _sourceEntities);

        [Benchmark]
        public void TransferEntityArcheType_NoDestroy() =>
            _destEntities = _destContext.Entities.CopyEntitiesTo(_archeType);

        [Benchmark]
        public void TransferEntityQuery_NoDestroy() =>
            _destEntities = _destContext.Entities.CopyEntitiesTo(_query);
    }
}