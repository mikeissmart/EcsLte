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
        [Params(
            BenchmarkTestConsts.MediumCount,
            BenchmarkTestConsts.LargeCount)]
        public int EntityCount { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Normal_Bx4);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _entities = _entities = _context.CreateEntities(EntityCount, _blueprint);
            _archeType = _blueprint.GetEntityArcheType();
            _query = new EntityQuery().WhereAllOf(_archeType);
        }

        [IterationCleanup]
        public void IterationCleanup() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void GetComponent_Entity()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                _context.GetComponent<TestComponent1>(entity);
                _context.GetComponent<TestComponent2>(entity);
                _context.GetComponent<TestComponent3>(entity);
                _context.GetComponent<TestComponent4>(entity);
            }
        }

        [Benchmark]
        public void GetComponents_Entities()
        {
            _context.GetComponents<TestComponent1>(_entities);
            _context.GetComponents<TestComponent2>(_entities);
            _context.GetComponents<TestComponent3>(_entities);
            _context.GetComponents<TestComponent4>(_entities);
        }

        [Benchmark]
        public void GetComponents_EntityArcheType()
        {
            _context.GetComponents<TestComponent1>(_archeType);
            _context.GetComponents<TestComponent2>(_archeType);
            _context.GetComponents<TestComponent3>(_archeType);
            _context.GetComponents<TestComponent4>(_archeType);
        }

        [Benchmark]
        public void GetComponents_EntityQuery()
        {
            _context.GetComponents<TestComponent1>(_query);
            _context.GetComponents<TestComponent2>(_query);
            _context.GetComponents<TestComponent3>(_query);
            _context.GetComponents<TestComponent4>(_query);
        }
    }
}
