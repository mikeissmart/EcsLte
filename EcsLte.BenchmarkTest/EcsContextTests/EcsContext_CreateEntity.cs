using BenchmarkDotNet.Attributes;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_CreateEntity
    {
        private EcsContext _context;
        private Entity[] _entities;

        [ParamsAllValues]
        public ComponentArrangement CompArr { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.Instance.HasContext("Test"))
                EcsContexts.Instance.DestroyContext(EcsContexts.Instance.GetContext("Test"));
            _context = EcsContexts.Instance.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.Instance.DestroyContext(_context);
        }

        #region Create

        [IterationSetup(Targets = new[]
        {
            nameof(CreateEntity),
            nameof(CreateEntities)
        })]
        public void IterationSetup_Create()
        {
            _context = EcsContexts.Instance.CreateContext("Test_Create");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [IterationCleanup(Targets = new[]
        {
            nameof(CreateEntity),
            nameof(CreateEntities)
        })]
        public void IterationCleanup_Create() => EcsContexts.Instance.DestroyContext(_context);

        [Benchmark]
        public void CreateEntity()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);
            for (var i = 0; i < _entities.Length; i++)
                _entities[i] = _context.Entities.CreateEntity(blueprint);
        }

        [Benchmark]
        public void CreateEntities()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);
            _entities = _context.Entities.CreateEntities(blueprint, _entities.Length);
        }

        #endregion Create

        #region Create Reuse

        [IterationCleanup(Targets = new[]
        {
            nameof(CreateEntity_Reuse),
            nameof(CreateEntities_Reuse)
        })]
        public void IterationCleanup_Create_Reuse() => _context.Entities.DestroyEntities(_entities);

        [Benchmark]
        public void CreateEntity_Reuse()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);
            for (var i = 0; i < _entities.Length; i++)
                _entities[i] = _context.Entities.CreateEntity(blueprint);
        }

        [Benchmark]
        public void CreateEntities_Reuse()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);
            _entities = _context.Entities.CreateEntities(blueprint, _entities.Length);
        }

        #endregion Create Reuse

        #region Destroy

        [IterationSetup(Targets = new[]
        {
            nameof(DestroyEntity),
            nameof(DestroyEntities)
        })]
        public void IterationSetup_Destroy()
        {
            _context = EcsContexts.Instance.CreateContext("Test_Create");
            _entities = _context.Entities.CreateEntities(
                EcsContextSetupCleanup.CreateBlueprint(CompArr),
                _entities.Length);
        }

        [IterationCleanup(Targets = new[]
        {
            nameof(DestroyEntity),
            nameof(DestroyEntities)
        })]
        public void IterationCleanup_Destroy() => EcsContexts.Instance.DestroyContext(_context);

        [Benchmark]
        public void DestroyEntity()
        {
            for (var i = 0; i < _entities.Length; i++)
                _context.Entities.DestroyEntity(_entities[i]);
        }

        [Benchmark]
        public void DestroyEntities() => _context.Entities.DestroyEntities(_entities);

        #endregion Destroy

        #region Destroy Reuse

        [IterationSetup(Targets = new[]
        {
            nameof(DestroyEntity_Reuse),
            nameof(DestroyEntities_Reuse)
        })]
        public void IterationSetup_Destroy_Reuse() => _entities = _context.Entities.CreateEntities(
                EcsContextSetupCleanup.CreateBlueprint(CompArr),
                _entities.Length);

        [Benchmark]
        public void DestroyEntity_Reuse()
        {
            for (var i = 0; i < _entities.Length; i++)
                _context.Entities.DestroyEntity(_entities[i]);
        }

        [Benchmark]
        public void DestroyEntities_Reuse() => _context.Entities.DestroyEntities(_entities);

        #endregion Destroy Reuse
    }
}