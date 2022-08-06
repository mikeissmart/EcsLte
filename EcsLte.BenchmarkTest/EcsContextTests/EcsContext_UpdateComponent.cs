using BenchmarkDotNet.Attributes;
using System;

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
        private TestManagedComponent1 ManagedComponent1 = new TestManagedComponent1 { Prop = 10 };
        private TestManagedComponent2 ManagedComponent2 = new TestManagedComponent2 { Prop = 11 };
        private TestManagedComponent3 ManagedComponent3 = new TestManagedComponent3 { Prop = 12 };
        private TestManagedComponent4 ManagedComponent4 = new TestManagedComponent4 { Prop = 13 };

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
        public void IterationSetup() => _entities = _context.Entities.CreateEntities(_blueprint, _entities.Length);

        [IterationCleanup]
        public void IterationCleanup() => _context.Entities.DestroyEntities(_entities);

        [Benchmark]
        public void UpdateComponent_Entity()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.Entities.UpdateComponent(entity, Component1);
                        _context.Entities.UpdateComponent(entity, Component2);
                        _context.Entities.UpdateComponent(entity, Component3);
                        _context.Entities.UpdateComponent(entity, Component4);
                    }
                    break;

                case ComponentArrangement.Managed_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.Entities.UpdateManagedComponent(entity, ManagedComponent1);
                        _context.Entities.UpdateManagedComponent(entity, ManagedComponent2);
                        _context.Entities.UpdateManagedComponent(entity, ManagedComponent3);
                        _context.Entities.UpdateManagedComponent(entity, ManagedComponent4);
                    }
                    break;

                case ComponentArrangement.Shared_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.Entities.UpdateSharedComponent(entity, SharedComponent1);
                        _context.Entities.UpdateSharedComponent(entity, SharedComponent2);
                        _context.Entities.UpdateSharedComponent(entity, SharedComponent3);
                        _context.Entities.UpdateSharedComponent(entity, SharedComponent4);
                    }
                    break;

                default:
                    throw new Exception();
            }
        }
    }
}