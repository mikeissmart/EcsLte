using BenchmarkDotNet.Attributes;
using System;

namespace EcsLte.BencharkTest.EcsContextTests
{
    // Avg Time 40 minutes
    [MemoryDiagnoser]
    public class EcsContext_EntityComponentGetTest
    {
        private EcsContext _context;
        private Entity[] _entities;

        [ParamsAllValues]
        public EntityComponentArrangement ComponentArrangement { get; set; }

        [ParamsAllValues]
        public EcsContextType ContextType { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context = SetupCleanupTest.EcsContext_Setup(ContextType);
            _entities = _context.CreateEntities(BenchmarkTestConsts.LargeCount);
            SetupCleanupTest.EntityComponent_AddComponent(ComponentArrangement, _context, _entities);
        }

        [GlobalCleanup]
        public void GlobalCleanup() => SetupCleanupTest.EcsContext_Cleanup(_context);

        [Benchmark]
        public void HasComponent()
        {
            var hasComponent = false;
            switch (ComponentArrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        hasComponent = _context.HasComponent<TestComponent1>(_entities[i]);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        hasComponent = _context.HasComponent<TestComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        hasComponent = _context.HasComponent<TestSharedComponent1>(_entities[i]);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        hasComponent = _context.HasComponent<TestSharedComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        hasComponent = _context.HasComponent<TestComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent1>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        hasComponent = _context.HasComponent<TestComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        hasComponent = _context.HasComponent<TestComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestComponent2>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent1>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        hasComponent = _context.HasComponent<TestComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestComponent2>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent1>(_entities[i]);
                        hasComponent = _context.HasComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [Benchmark]
        public void GetComponent()
        {
            IComponent getComponent;
            switch (ComponentArrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        getComponent = _context.GetComponent<TestComponent1>(_entities[i]);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        getComponent = _context.GetComponent<TestComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                        getComponent = _context.GetComponent<TestSharedComponent1>(_entities[i]);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        getComponent = _context.GetComponent<TestSharedComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        getComponent = _context.GetComponent<TestComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent1>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        getComponent = _context.GetComponent<TestComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        getComponent = _context.GetComponent<TestComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestComponent2>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent1>(_entities[i]);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        getComponent = _context.GetComponent<TestComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestComponent2>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent1>(_entities[i]);
                        getComponent = _context.GetComponent<TestSharedComponent2>(_entities[i]);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [Benchmark]
        public void GetAllComponents()
        {
            IComponent[] components;
            for (var i = 0; i < _entities.Length; i++)
                components = _context.GetAllComponents(_entities[i]);
        }
    }
}
