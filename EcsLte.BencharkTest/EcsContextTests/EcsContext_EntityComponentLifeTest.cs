using BenchmarkDotNet.Attributes;
using System;

namespace EcsLte.BencharkTest.EcsContextTests
{
    // Avg Time 70 minutes
    [MemoryDiagnoser]
    public class EcsContext_EntityComponentLifeTest
    {
        private EcsContext _context;
        private Entity[] _entities;

        private TestComponent1 _replaceComponent1 = new TestComponent1 { Prop = 5 };
        private TestComponent2 _replaceComponent2 = new TestComponent2 { Prop = 6 };
        private TestSharedComponent1 _replaceComponentShared1 = new TestSharedComponent1 { Prop = 7 };
        private TestSharedComponent2 _replaceComponentShared2 = new TestSharedComponent2 { Prop = 5 };

        [ParamsAllValues]
        public EntityComponentArrangement ComponentArrangement { get; set; }

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

        [IterationSetup]
        public void IterationSetup() => SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context, _entities);

        [IterationCleanup]
        public void IterationCleanup()
        {
            for (var i = 0; i < _entities.Length; i++)
                _context.RemoveAllComponents(_entities[i]);
        }

        [IterationSetup(Target = nameof(AddComponent))]
        public void AddComponent_Setup()
        {
        }

        [Benchmark]
        public void AddComponent() => SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context, _entities);

        [Benchmark]
        public void ReplaceComponent()
        {
            switch (ComponentArrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        _context.ReplaceComponent(_entities[i], _replaceComponent1);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.ReplaceComponent(_entities[i], _replaceComponent1);
                        _context.ReplaceComponent(_entities[i], _replaceComponent2);
                    }
                    break;
                case EntityComponentArrangement.Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared1);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared1);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared2);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.ReplaceComponent(_entities[i], _replaceComponent1);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared1);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.ReplaceComponent(_entities[i], _replaceComponent1);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared1);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared2);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.ReplaceComponent(_entities[i], _replaceComponent1);
                        _context.ReplaceComponent(_entities[i], _replaceComponent2);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared1);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.ReplaceComponent(_entities[i], _replaceComponent1);
                        _context.ReplaceComponent(_entities[i], _replaceComponent2);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared1);
                        _context.ReplaceComponent(_entities[i], _replaceComponentShared2);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [Benchmark]
        public void RemoveComponent()
        {
            switch (ComponentArrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        _context.RemoveComponent<TestComponent1>(_entities[i]);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.RemoveComponent<TestComponent1>(_entities[i]);
                        _context.RemoveComponent<TestComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        _context.RemoveComponent<TestSharedComponent1>(_entities[i]);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.RemoveComponent<TestSharedComponent1>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.RemoveComponent<TestComponent1>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent1>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.RemoveComponent<TestComponent1>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent1>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.RemoveComponent<TestComponent1>(_entities[i]);
                        _context.RemoveComponent<TestComponent2>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent1>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        _context.RemoveComponent<TestComponent1>(_entities[i]);
                        _context.RemoveComponent<TestComponent2>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent1>(_entities[i]);
                        _context.RemoveComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [Benchmark]
        public void RemoveAllComponents()
        {
            for (var i = 0; i < _entities.Length; i++)
                _context.RemoveAllComponents(_entities[i]);
        }
    }
}
