using BenchmarkDotNet.Attributes;

namespace EcsLte.BencharkTest.EcsContextTests
{
    // Avg Time 2 minutes
    [MemoryDiagnoser]
    public class EcsContext_EntityLifeTest
    {
        private EcsContext _context;
        private Entity[] _entities;
        private Entity[][] _entities_CreateEntities;

        [ParamsAllValues]
        public EcsContextType ContextType { get; set; }

        #region Create

        [IterationSetup(Target = nameof(CreateEntites))]
        public void IterationSetup_CreateEntites()
        {
            _context = SetupCleanupTest.EcsContext_Setup(ContextType);
            _entities_CreateEntities = new Entity[BenchmarkTestConsts.MediumCount][];
        }

        [IterationCleanup(Target = nameof(CreateEntites))]
        public void IterationCleanup_CreateEntites() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [Benchmark]
        public void CreateEntites()
        {
            for (var i = 0; i < BenchmarkTestConsts.MediumCount; i++)
                _entities_CreateEntities[i] = _context.CreateEntities(BenchmarkTestConsts.SmallCount);
        }

        [IterationSetup(Target = nameof(CreateEntity))]
        public void IterationSetup_CreateEntity()
        {
            _context = SetupCleanupTest.EcsContext_Setup(ContextType);
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [IterationCleanup(Target = nameof(CreateEntity))]
        public void IterationCleanup_CreateEntity() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [Benchmark]
        public void CreateEntity()
        {
            for (var i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                _entities[i] = _context.CreateEntity();
        }

        #endregion

        #region Create_Reused

        [GlobalSetup(Targets = new[]
        {
            nameof(CreateEntites_Reused),
            nameof(CreateEntity_Reused)
        })]
        public void GlobalSetup_Reused() => _context = SetupCleanupTest.EcsContext_Setup(ContextType);

        [GlobalCleanup(Targets = new[]
        {
            nameof(CreateEntites_Reused),
            nameof(CreateEntity_Reused)
        })]
        public void GlobalCleanup_Reused() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [IterationSetup(Target = nameof(CreateEntites_Reused))]
        public void IterationSetup_CreateEntites_Reused() => _entities_CreateEntities = new Entity[BenchmarkTestConsts.MediumCount][];

        [IterationCleanup(Target = nameof(CreateEntites_Reused))]
        public void IterationCleanup_CreateEntites_Reused()
        {
            foreach (var entities in _entities_CreateEntities)
                _context.DestroyEntities(entities);
        }

        [Benchmark]
        public void CreateEntites_Reused()
        {
            for (var i = 0; i < BenchmarkTestConsts.MediumCount; i++)
                _entities_CreateEntities[i] = _context.CreateEntities(BenchmarkTestConsts.SmallCount);
        }

        [IterationSetup(Target = nameof(CreateEntity_Reused))]
        public void IterationSetup_CreateEntity_Reused() => _entities = new Entity[BenchmarkTestConsts.LargeCount];

        [IterationCleanup(Target = nameof(CreateEntity_Reused))]
        public void IterationCleanup_CreateEntity_Reused() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void CreateEntity_Reused()
        {
            for (var i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                _entities[i] = _context.CreateEntity();
        }

        #endregion

        #region Destroy

        [GlobalSetup(Targets = new[]
        {
            nameof(DestroyEntities),
            nameof(DestroyEntity)
        })]
        public void GlobalSetup_Destroy() => _context = SetupCleanupTest.EcsContext_Setup(ContextType);

        [GlobalCleanup(Targets = new[]
        {
            nameof(DestroyEntities),
            nameof(DestroyEntity)
        })]
        public void GlobalCleanup_Destroy() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [IterationSetup(Targets = new[] { nameof(DestroyEntities), nameof(DestroyEntity) })]
        public void IterationSetup_Destroy() => _entities = _context.CreateEntities(BenchmarkTestConsts.LargeCount);

        [Benchmark]
        public void DestroyEntities() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void DestroyEntity()
        {
            for (var i = 0; i < _entities.Length; i++)
                _context.DestroyEntity(_entities[i]);
        }

        #endregion
    }
}
