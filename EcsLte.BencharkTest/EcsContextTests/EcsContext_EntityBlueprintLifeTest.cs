using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using EcsLte.BencharkTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BencharkTest.EcsContextTests
{
    [MemoryDiagnoser]
    public class EcsContext_EntityBlueprintLifeTest
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
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            SetupCleanupTest.EcsContext_Cleanup(_context);
        }

        [IterationCleanup]
        public void IterationCleanup_CreateEntites()
        {
            _context.DestroyEntities(_entities);
        }

        [Benchmark]
        public void CreateEntityBlueprint()
        {
            var blueprint = SetupCleanupTest.GetEntityBlueprint(ContextType, ComponentArrangement);
            for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                _entities[i] = _context.CreateEntity(blueprint);
        }

        [Benchmark]
        public void CreateEntitiesBlueprint()
        {
            var blueprint = SetupCleanupTest.GetEntityBlueprint(ContextType, ComponentArrangement);
            _entities = _context.CreateEntities(BenchmarkTestConsts.LargeCount, blueprint);
        }
    }
}
