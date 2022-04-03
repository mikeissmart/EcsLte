using BenchmarkDotNet.Attributes;

namespace EcsLte.BencharkTest.EcsContextTests
{
    // Avg Time 5 minutes
    [MemoryDiagnoser]
    public class EcsContext_EntityGetTest
    {
        private EcsContext _context;
        private Entity[] _entities;

        [ParamsAllValues]
        public EcsContextType ContextType { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context = SetupCleanupTest.EcsContext_Setup(ContextType);
            _entities = _context.CreateEntities(BenchmarkTestConsts.LargeCount);
        }

        [GlobalCleanup]
        public void GlobalCleanup() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [Benchmark]
        public void GetEntities()
        {
            Entity[] entities;
            for (var i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                entities = _context.GetEntities();
        }

        [Benchmark]
        public void HasEntity()
        {
            var hasEntity = false;
            for (var i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                hasEntity = _context.HasEntity(_entities[i]);
        }
    }
}
