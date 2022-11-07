using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_UpdateComponents
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
        public void UpdateComponents_Entity()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.Entities.UpdateComponents(entity,
                            Component1,
                            Component2,
                            Component3,
                            Component4);
                    }
                    break;

                case ComponentArrangement.Managed_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.Entities.UpdateComponents(entity,
                            ManagedComponent1,
                            ManagedComponent2,
                            ManagedComponent3,
                            ManagedComponent4);
                    }
                    break;

                case ComponentArrangement.Shared_x4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.Entities.UpdateComponents(entity,
                            SharedComponent1,
                            SharedComponent2,
                            SharedComponent3,
                            SharedComponent4);
                    }
                    break;

                default:
                    throw new Exception();
            }
        }
    }
}
