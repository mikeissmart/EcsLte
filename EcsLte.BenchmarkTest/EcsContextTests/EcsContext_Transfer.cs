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

        [ParamsAllValues]
        public ComponentArrangement CompArr { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Source"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Source"));
            _sourceContext = EcsContexts.CreateContext("Source");
            if (EcsContexts.HasContext("Dest"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Dest"));
            _destContext = EcsContexts.CreateContext("Dest");
            _destEntities = new Entity[BenchmarkTestConsts.LargeCount];

            _archeType = EcsContextSetupCleanup.CreateBlueprint(CompArr)
                .GetArcheType(_sourceContext);
            _query = _sourceContext.Queries
                .SetFilter(_sourceContext.Filters
                    .WhereAllOf(_archeType));
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_sourceContext.IsDestroyed)
                EcsContexts.DestroyContext(_sourceContext);
            if (!_destContext.IsDestroyed)
                EcsContexts.DestroyContext(_destContext);
        }

        [IterationSetup()]
        public void IterationSetup_Create()
        {
            _sourceEntities = _sourceContext.Entities.CreateEntities(
                EcsContextSetupCleanup.CreateBlueprint(CompArr),
                BenchmarkTestConsts.LargeCount);

            _archeType = EcsContextSetupCleanup.CreateBlueprint(CompArr)
                .GetArcheType(_sourceContext);
            _query = _sourceContext.Queries
                .SetFilter(_sourceContext.Filters
                    .WhereAllOf(_archeType));
        }

        [IterationCleanup()]
        public void IterationCleanup_Create()
        {
            _sourceContext.Entities.DestroyEntities(
                _sourceEntities);
            _destContext.Entities.DestroyEntities(
                _destEntities);
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