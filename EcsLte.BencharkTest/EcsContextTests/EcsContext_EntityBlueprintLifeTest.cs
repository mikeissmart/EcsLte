using BenchmarkDotNet.Attributes;

namespace EcsLte.BencharkTest.EcsContextTests
{
    [MemoryDiagnoser]
    public class EcsContext_EntityBlueprintLifeTest
    {
        private EcsContext _context;
        private Entity[] _entities;

        [ParamsAllValues]
        public EntityComponentArrangement ComponentArrangement { get; set; }

        [ParamsAllValues]
        public EcsContextType ContextType { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context = SetupCleanupTest.EcsContext_Setup(ContextType);
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [GlobalCleanup]
        public void GlobalCleanup() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [IterationCleanup]
        public void IterationCleanup() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void CreateEntityBlueprint()
        {
            var blueprint = SetupCleanupTest.GetEntityBlueprint(ContextType, ComponentArrangement);
            for (var i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                _entities[i] = _context.CreateEntity(blueprint);
        }

        [Benchmark]
        public void CreateEntitiesBlueprint()
        {
            var blueprint = SetupCleanupTest.GetEntityBlueprint(ContextType, ComponentArrangement);
            _entities = _context.CreateEntities(BenchmarkTestConsts.LargeCount, blueprint);
        }
    }
}
