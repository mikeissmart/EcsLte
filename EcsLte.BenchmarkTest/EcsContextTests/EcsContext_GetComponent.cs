using BenchmarkDotNet.Attributes;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_GetComponent
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityBlueprint _blueprint;
        private EntityArcheType _archeType;
        private EntityQuery _query;

        // Didnt use ComponentArrangement because all the time were the same
        public int EntityCount { get; set; } = BenchmarkTestConsts.LargeCount;

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Normal_x4);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _entities = _entities = _context.Entities.CreateEntities(_blueprint, EntityState.Active, EntityCount);
            _archeType = _blueprint.GetArcheType();
            _query = new EntityQuery(
                _context,
                new EntityFilter()
                    .WhereAllOf(_archeType));
        }

        [IterationCleanup]
        public void IterationCleanup() => _context.Entities.DestroyEntities(_entities);

        [Benchmark]
        public void GetComponent_Entity()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                _context.Entities.GetComponent<TestComponent1>(entity);
                _context.Entities.GetComponent<TestComponent2>(entity);
                _context.Entities.GetComponent<TestComponent3>(entity);
                _context.Entities.GetComponent<TestComponent4>(entity);
            }
        }

        [Benchmark]
        public void GetComponents_EntityArcheType()
        {
            _context.Entities.GetComponents<TestComponent1>(_archeType);
            _context.Entities.GetComponents<TestComponent2>(_archeType);
            _context.Entities.GetComponents<TestComponent3>(_archeType);
            _context.Entities.GetComponents<TestComponent4>(_archeType);
        }

        [Benchmark]
        public void GetComponents_EntityQuery()
        {
            _context.Entities.GetComponents<TestComponent1>(_query);
            _context.Entities.GetComponents<TestComponent2>(_query);
            _context.Entities.GetComponents<TestComponent3>(_query);
            _context.Entities.GetComponents<TestComponent4>(_query);
        }
    }
}
