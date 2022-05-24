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
                .GetEntityArcheType();
            _query = new EntityQuery()
                .WhereAllOf(_archeType);
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
            _sourceEntities = _sourceContext.CreateEntities(BenchmarkTestConsts.LargeCount,
                EcsContextSetupCleanup.CreateBlueprint(CompArr));

            _archeType = EcsContextSetupCleanup.CreateBlueprint(CompArr)
                .GetEntityArcheType();
            _query = new EntityQuery()
                .WhereAllOf(_archeType);
        }

        [IterationCleanup()]
        public void IterationCleanup_Create()
        {
            _sourceContext.DestroyEntities(_archeType);
            _destContext.DestroyEntities(_archeType);
        }

        [Benchmark]
        public void TransferEntity_NoDestroy()
        {
            for (var i = 0; i < _sourceEntities.Length; i++)
                _destEntities[i] = _destContext.TransferEntity(_sourceContext, _sourceEntities[i], false);
        }

        [Benchmark]
        public void TransferEntities_NoDestroy()
        {
            _destEntities = _destContext.TransferEntities(_sourceContext, _sourceEntities, false);
        }

        [Benchmark]
        public void TransferEntityArcheType_NoDestroy()
        {
            _destEntities = _destContext.TransferEntities(_sourceContext, _archeType, false);
        }

        [Benchmark]
        public void TransferEntityQuery_NoDestroy()
        {
            _destEntities = _destContext.TransferEntities(_sourceContext, _query, false);
        }

        [Benchmark]
        public void TransferEntity_Destroy()
        {
            for (var i = 0; i < _sourceEntities.Length; i++)
                _destEntities[i] = _destContext.TransferEntity(_sourceContext, _sourceEntities[i], true);
        }

        [Benchmark]
        public void TransferEntities_Destroy()
        {
            _destEntities = _destContext.TransferEntities(_sourceContext, _sourceEntities, true);
        }

        [Benchmark]
        public void TransferEntityArcheType_Destroy()
        {
            _destEntities = _destContext.TransferEntities(_sourceContext, _archeType, true);
        }

        [Benchmark]
        public void TransferEntityQuery_Destroy()
        {
            _destEntities = _destContext.TransferEntities(_sourceContext, _query, true);
        }
    }
}
