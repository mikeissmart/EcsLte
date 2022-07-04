using BenchmarkDotNet.Attributes;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_UpdateComponent
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityBlueprint _blueprint;

        private TestComponent1 Component1 = new TestComponent1 { Prop = 2 };
        private TestComponent2 Component2 = new TestComponent2 { Prop = 3 };
        private TestComponent3 Component3 = new TestComponent3 { Prop = 4 };
        private TestComponent4 Component4 = new TestComponent4 { Prop = 5 };
        private TestSharedComponent1 SharedComponent1 = new TestSharedComponent1 { Prop = 6 };
        private TestSharedComponent2 SharedComponent2 = new TestSharedComponent2 { Prop = 7 };
        private TestSharedComponent3 SharedComponent3 = new TestSharedComponent3 { Prop = 8 };
        private TestSharedComponent4 SharedComponent4 = new TestSharedComponent4 { Prop = 9 };

        [ParamsAllValues]
        public ComponentArrangement CompArr { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.DestroyContext(_context);
        }

        [IterationSetup]
        public void IterationSetup() => _entities = _context.CreateEntities(_entities.Length, _blueprint);

        [IterationCleanup]
        public void IterationCleanup() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void UpdateComponent_Entity()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, Component1);
                        _context.UpdateComponent(entity, Component2);
                        _context.UpdateComponent(entity, Component3);
                        _context.UpdateComponent(entity, Component4);
                    }
                    break;

                case ComponentArrangement.Shared_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, SharedComponent1);
                        _context.UpdateComponent(entity, SharedComponent2);
                        _context.UpdateComponent(entity, SharedComponent3);
                        _context.UpdateComponent(entity, SharedComponent4);
                    }
                    break;
            }
        }
    }
}