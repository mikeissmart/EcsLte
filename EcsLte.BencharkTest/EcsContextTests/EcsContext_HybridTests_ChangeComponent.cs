using BenchmarkDotNet.Attributes;
using EcsLte.HybridArcheType;

namespace EcsLte.BencharkTest.EcsContextTests
{
    [MemoryDiagnoser]
    public class EcsContext_HybridTests_ChangeComponent
    {
        private EcsContext_Hybrid _context_Hybrid;
        private EcsContext _context_Managed;
        private EcsContext _context_Native;
        private EcsContext _context_Native_Cont;
        private Entity[] _entities;

        private TestComponent1 _component1_1 = new TestComponent1 { Prop = 1 };
        private TestComponent2 _component2_1 = new TestComponent2 { Prop = 2 };
        private TestSharedComponent1 _sharedComponent1_1 = new TestSharedComponent1 { Prop = 3 };
        private TestSharedComponent2 _sharedComponent2_1 = new TestSharedComponent2 { Prop = 4 };

        private TestComponent1 _component1_2 = new TestComponent1 { Prop = 5 };
        private TestComponent2 _component2_2 = new TestComponent2 { Prop = 6 };
        private TestSharedComponent1 _sharedComponent1_2 = new TestSharedComponent1 { Prop = 7 };
        private TestSharedComponent2 _sharedComponent2_2 = new TestSharedComponent2 { Prop = 8 };

        public EntityComponentArrangement ComponentArrangement { get; set; } = EntityComponentArrangement.Normal_x2_Shared_x2;

        [IterationCleanup]
        public void IterationCleanup()
        {
            if (_context_Hybrid != null && _context_Hybrid.EntityCount > 0)
                EcsContexts.DestroyContext_Hybrid(_context_Hybrid);
            if (_context_Managed != null && _context_Managed.EntityCount > 0)
                EcsContexts.DestroyContext(_context_Managed);
            if (_context_Native != null && _context_Native.EntityCount > 0)
                EcsContexts.DestroyContext(_context_Native);
            if (_context_Native_Cont != null && _context_Native_Cont.EntityCount > 0)
                EcsContexts.DestroyContext(_context_Native_Cont);
            _entities = null;
        }

        [IterationSetup(Target = nameof(UpdateComponent_Managed))]
        public void IterationSetup_Managed()
        {
            _context_Managed = EcsContexts.CreateEcsContext_ArcheType_Managed("Managed_Test");
            _entities = _context_Managed.CreateEntities(BenchmarkTestConsts.LargeCount);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Managed, _entities);
        }

        [Benchmark]
        public void UpdateComponent_Managed()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                _context_Managed.ReplaceComponent(_entities[i], _component1_2);
                _context_Managed.ReplaceComponent(_entities[i], _component2_2);
                _context_Managed.ReplaceComponent(_entities[i], _sharedComponent1_2);
                _context_Managed.ReplaceComponent(_entities[i], _sharedComponent2_2);
            }
        }

        [IterationSetup(Target = nameof(UpdateComponent_Native))]
        public void IterationSetup_Native()
        {
            _context_Native = EcsContexts.CreateEcsContext_ArcheType_Native("Native_Test");
            _entities = _context_Native.CreateEntities(BenchmarkTestConsts.LargeCount);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Native, _entities);
        }

        [Benchmark]
        public void UpdateComponent_Native()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                _context_Native.ReplaceComponent(_entities[i], _component1_2);
                _context_Native.ReplaceComponent(_entities[i], _component2_2);
                _context_Native.ReplaceComponent(_entities[i], _sharedComponent1_2);
                _context_Native.ReplaceComponent(_entities[i], _sharedComponent2_2);
            }
        }

        [IterationSetup(Target = nameof(UpdateComponent_Native_Cont))]
        public void IterationSetup_Native_Cont()
        {
            _context_Native_Cont = EcsContexts.CreateEcsContext_ArcheType_Native_Continuous("Native_Cont_Test");
            _entities = _context_Native_Cont.CreateEntities(BenchmarkTestConsts.LargeCount);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Native_Cont, _entities);
        }

        [Benchmark]
        public void UpdateComponent_Native_Cont()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                _context_Native_Cont.ReplaceComponent(_entities[i], _component1_2);
                _context_Native_Cont.ReplaceComponent(_entities[i], _component2_2);
                _context_Native_Cont.ReplaceComponent(_entities[i], _sharedComponent1_2);
                _context_Native_Cont.ReplaceComponent(_entities[i], _sharedComponent2_2);
            }
        }

        [IterationSetup(Target = nameof(UpdateComponent_Hybrid))]
        public void IterationSetup_Hybrid()
        {
            _context_Hybrid = EcsContexts.CreateEcsContext_Hybrid("Hybrid_Test");
            _entities = _context_Hybrid.CreateEntities(BenchmarkTestConsts.LargeCount, CreateBlueprint());
        }

        [Benchmark]
        public void UpdateComponent_Hybrid()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                _context_Hybrid.UpdateComponent(_entities[i], _component1_2);
                _context_Hybrid.UpdateComponent(_entities[i], _component2_2);
                _context_Hybrid.UpdateComponent(_entities[i], _sharedComponent1_2);
                _context_Hybrid.UpdateComponent(_entities[i], _sharedComponent2_2);
            }
        }

        private EntityBlueprint_Hybrid CreateBlueprint()
        {
            var blueprint = new EntityBlueprint_Hybrid();

            switch (ComponentArrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    blueprint = blueprint.AddComponent(_component1_1);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    blueprint = blueprint.AddComponent(_component1_1);
                    blueprint = blueprint.AddComponent(_component2_1);
                    break;
                case EntityComponentArrangement.Shared_x1:
                    blueprint = blueprint.AddComponent(_sharedComponent1_1);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    blueprint = blueprint.AddComponent(_sharedComponent1_1);
                    blueprint = blueprint.AddComponent(_sharedComponent2_1);
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    blueprint = blueprint.AddComponent(_component1_1);
                    blueprint = blueprint.AddComponent(_sharedComponent1_1);
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    blueprint = blueprint.AddComponent(_component1_1);
                    blueprint = blueprint.AddComponent(_sharedComponent1_1);
                    blueprint = blueprint.AddComponent(_sharedComponent2_1);
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    blueprint = blueprint.AddComponent(_component1_1);
                    blueprint = blueprint.AddComponent(_component2_1);
                    blueprint = blueprint.AddComponent(_sharedComponent1_1);
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    blueprint = blueprint.AddComponent(_sharedComponent1_1);
                    blueprint = blueprint.AddComponent(_sharedComponent2_1);
                    blueprint = blueprint.AddComponent(_component1_1);
                    blueprint = blueprint.AddComponent(_component2_1);
                    break;
            }

            return blueprint;
        }
    }
}
