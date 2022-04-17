using BenchmarkDotNet.Attributes;
using EcsLte.HybridArcheType;
using System;

namespace EcsLte.BencharkTest.EcsContextTests
{
    [MemoryDiagnoser]
    public class EcsContext_HybridTests_CreateEntityAddComponent_Reuse
    {
        private EcsContext_Hybrid _context_Hybrid;
        private EcsContext _context_Managed;
        private EcsContext _context_Native;
        private EcsContext _context_Native_Cont;
        private Entity[] _entities;

        private TestComponent1 _component1 = new TestComponent1 { Prop = 1 };
        private TestComponent2 _component2 = new TestComponent2 { Prop = 2 };
        private TestSharedComponent1 _sharedComponent1 = new TestSharedComponent1 { Prop = 3 };
        private TestSharedComponent2 _sharedComponent2 = new TestSharedComponent2 { Prop = 4 };

        public EntityComponentArrangement ComponentArrangement { get; set; } = EntityComponentArrangement.Normal_x2_Shared_x2;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context_Hybrid = EcsContexts.CreateEcsContext_Hybrid("Hybrid_Test");
            _context_Managed = EcsContexts.CreateEcsContext_ArcheType_Managed("Managed_Test");
            _context_Native = EcsContexts.CreateEcsContext_ArcheType_Native("Native_Test");
            _context_Native_Cont = EcsContexts.CreateEcsContext_ArcheType_Native_Continuous("Native_Cont_Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            EcsContexts.DestroyContext_Hybrid(_context_Hybrid);
            EcsContexts.DestroyContext(_context_Managed);
            EcsContexts.DestroyContext(_context_Native);
            EcsContexts.DestroyContext(_context_Native_Cont);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            if (_context_Hybrid.EntityCount > 0)
                _context_Hybrid.DestroyEntities(_entities);
            if (_context_Managed.EntityCount > 0)
                _context_Managed.DestroyEntities(_entities);
            if (_context_Native.EntityCount > 0)
                _context_Native.DestroyEntities(_entities);
            if (_context_Native_Cont.EntityCount > 0)
                _context_Native_Cont.DestroyEntities(_entities);
            Array.Clear(_entities, 0, _entities.Length);
        }

        [Benchmark]
        public void CreateEntity_AddComponent_Managed()
        {
            for (var i = 0; i < _entities.Length; i++)
                _entities[i] = _context_Managed.CreateEntity();
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Managed, _entities);
        }

        [Benchmark]
        public void CreateEntity_AddComponent_Native()
        {
            for (var i = 0; i < _entities.Length; i++)
                _entities[i] = _context_Native.CreateEntity();
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Native, _entities);
        }

        [Benchmark]
        public void CreateEntity_AddComponent_Native_Cont()
        {
            for (var i = 0; i < _entities.Length; i++)
                _entities[i] = _context_Native_Cont.CreateEntity();
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Native_Cont, _entities);
        }

        [Benchmark]
        public void CreateEntity_AddComponent_Hybrid()
        {
            var blueprint = CreateBlueprint();
            for (var i = 0; i < _entities.Length; i++)
                _entities[i] = _context_Hybrid.CreateEntity(blueprint);
        }

        [Benchmark]
        public void CreateEntities_AddComponent_Managed()
        {
            _entities = _context_Managed.CreateEntities(_entities.Length);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Managed, _entities);
        }

        [Benchmark]
        public void CreateEntities_AddComponent_Native()
        {
            _entities = _context_Native.CreateEntities(_entities.Length);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Native, _entities);
        }

        [Benchmark]
        public void CreateEntities_AddComponent_Native_Cont()
        {
            _entities = _context_Native_Cont.CreateEntities(_entities.Length);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context_Native_Cont, _entities);
        }

        [Benchmark]
        public void CreateEntities_AddComponent_Hybrid()
        {
            var blueprint = CreateBlueprint();
            _entities = _context_Hybrid.CreateEntities(_entities.Length, blueprint);
        }

        private EntityBlueprint_Hybrid CreateBlueprint()
        {
            var blueprint = new EntityBlueprint_Hybrid();

            switch (ComponentArrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    blueprint = blueprint.AddComponent(_component1);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    blueprint = blueprint.AddComponent(_component1);
                    blueprint = blueprint.AddComponent(_component2);
                    break;
                case EntityComponentArrangement.Shared_x1:
                    blueprint = blueprint.AddComponent(_sharedComponent1);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    blueprint = blueprint.AddComponent(_sharedComponent1);
                    blueprint = blueprint.AddComponent(_sharedComponent2);
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    blueprint = blueprint.AddComponent(_component1);
                    blueprint = blueprint.AddComponent(_sharedComponent1);
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    blueprint = blueprint.AddComponent(_component1);
                    blueprint = blueprint.AddComponent(_sharedComponent1);
                    blueprint = blueprint.AddComponent(_sharedComponent2);
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    blueprint = blueprint.AddComponent(_component1);
                    blueprint = blueprint.AddComponent(_component2);
                    blueprint = blueprint.AddComponent(_sharedComponent1);
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    blueprint = blueprint.AddComponent(_sharedComponent1);
                    blueprint = blueprint.AddComponent(_sharedComponent2);
                    blueprint = blueprint.AddComponent(_component1);
                    blueprint = blueprint.AddComponent(_component2);
                    break;
            }

            return blueprint;
        }
    }
}
