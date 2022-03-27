using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BencharkTest.EcsContextTests
{
    // Avg Time 2 minutes
    [MemoryDiagnoser]
    public class EcsContext_EntityLifeTest
    {
        private EcsContext _context;
        private Entity[] _entities_CreateEntity;
        private Entity[][] _entities_CreateEntities;

        [ParamsAllValues]
        public EcsContextType ContextType { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context = SetupCleanupTest.EcsContext_Setup(ContextType);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            SetupCleanupTest.EcsContext_Cleanup(_context);
        }

        [IterationSetup(Target = nameof(CreateEntites))]
        public void IterationSetup_Create()
        {
            _entities_CreateEntities = new Entity[BenchmarkTestConsts.MediumCount][];
        }

        [IterationCleanup(Target = nameof(CreateEntites))]
        public void IterationCleanup_Create()
        {
            foreach (var entities in _entities_CreateEntities)
                _context.DestroyEntities(entities);
        }

        [Benchmark]
        public void CreateEntites()
        {
            for (int i = 0; i < BenchmarkTestConsts.MediumCount; i++)
                _entities_CreateEntities[i] = _context.CreateEntities(BenchmarkTestConsts.SmallCount);
        }

        [IterationSetup(Target = nameof(CreateEntity))]
        public void IterationSetup_CreateEntity()
        {
            _entities_CreateEntity = new Entity[BenchmarkTestConsts.LargeCount];
        }

        [IterationCleanup(Target = nameof(CreateEntity))]
        public void IterationCleanup_CreateEntity()
        {
            _context.DestroyEntities(_entities_CreateEntity);
        }

        [Benchmark]
        public void CreateEntity()
        {
            for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
                _entities_CreateEntity[i] = _context.CreateEntity();
        }

        [IterationSetup(Targets = new[] { nameof(DestroyEntities), nameof(DestroyEntity) })]
        public void IterationSetup_Destroy()
        {
            _entities_CreateEntity = _context.CreateEntities(BenchmarkTestConsts.LargeCount);
        }

        [Benchmark]
        public void DestroyEntities()
        {
            _context.DestroyEntities(_entities_CreateEntity);
        }

        [Benchmark]
        public void DestroyEntity()
        {
            for (int i = 0; i < _entities_CreateEntity.Length; i++)
                _context.DestroyEntity(_entities_CreateEntity[i]);
        }
    }
}
