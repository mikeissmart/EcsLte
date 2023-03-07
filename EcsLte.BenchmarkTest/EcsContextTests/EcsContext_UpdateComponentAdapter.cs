using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_UpdateComponentAdapter
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityBlueprint _blueprint;

        private TestComponent1 Component1 = new TestComponent1 { Prop = 2 };
        private TestComponent2 Component2 = new TestComponent2 { Prop = 3 };
        private TestManagedComponent1 ManagedComponent1 = new TestManagedComponent1 { Prop = 11 };
        private TestManagedComponent2 ManagedComponent2 = new TestManagedComponent2 { Prop = 12 };

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.Instance.HasContext("Test"))
                EcsContexts.Instance.DestroyContext(EcsContexts.Instance.GetContext("Test"));
            _context = EcsContexts.Instance.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _blueprint = EcsContextSetupCleanup.CreateAllBluprint();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.Instance.DestroyContext(_context);
        }

        [IterationSetup]
        public void IterationSetup() => _entities = _context.Entities.CreateEntities(_blueprint, _entities.Length);

        [IterationCleanup]
        public void IterationCleanup() => _context.Entities.DestroyEntities(_entities);

        [Benchmark]
        public void UpdateComponent_Entity()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                _context.Entities.UpdateComponent(entity, Component1);
                _context.Entities.UpdateComponent(entity, Component2);
            }
        }

        [Benchmark]
        public void UpdateComponent_Entity_Adapter()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                _context.Entities.UpdateComponents(entity, Component1, Component2);
            }
        }

        [Benchmark]
        public void UpdateManagedComponent_Entity()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                _context.Entities.UpdateManagedComponent(entity, ManagedComponent1);
                _context.Entities.UpdateManagedComponent(entity, ManagedComponent2);
            }
        }

        [Benchmark]
        public void UpdateManagedComponent_Entity_Adapter()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                _context.Entities.UpdateComponents(entity, ManagedComponent1, ManagedComponent2);
            }
        }
    }
}
