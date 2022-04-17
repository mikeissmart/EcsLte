namespace EcsLte.BencharkTest.Data
{
    /*[MemoryDiagnoser]
    public class ArrayManagedVsNative
    {
        private Entity[] _entities_Managed;
        private unsafe Entity* _entities_Native;

        public IEnumerable<int> EntityCountSource
        {
            get => Enumerable.Range(
                    BenchmarkTestConsts.MediumCount, BenchmarkTestConsts.LargeCount)
                .Where(x => x % (BenchmarkTestConsts.MediumCount * 10) == 0);
        }

        [ParamsSource(nameof(EntityCountSource))]
        public int EntityCount { get; set; }

        [Params(true, false)]
        public bool IsManaged { get; set; }

        [IterationCleanup]
        public unsafe void IterationCleanup_GetEntites()
        {
            _entities_Managed = null;
            MemoryHelper.Free(_entities_Native);
            _entities_Native = null;
        }

        [IterationSetup(Target = nameof(AddEntites))]
        public unsafe void IterationSetup_AddEntites()
        {
            _entities_Managed = new Entity[EntityCount];
            _entities_Native = MemoryHelper.Alloc<Entity>(EntityCount);
        }

        [Benchmark]
        public unsafe void AddEntites()
        {
            if (IsManaged)
            {
                for (int i = 1; i < EntityCount; i++)
                    _entities_Managed[i] = new Entity();
            }
            else
            {
                for (int i = 1; i < EntityCount; i++)
                    _entities_Native[i] = new Entity();
            }
        }

        [IterationSetup(Target = nameof(GetEntites))]
        public unsafe void IterationSetup_GetEntites()
        {
            _entities_Managed = new Entity[EntityCount];
            _entities_Native = MemoryHelper.Alloc<Entity>(EntityCount);
            if (IsManaged)
            {
                for (int i = 1; i < EntityCount; i++)
                    _entities_Managed[i] = new Entity();
            }
            else
            {
                for (int i = 1; i < EntityCount; i++)
                    _entities_Native[i] = new Entity();
            }
        }

        [Benchmark]
        public unsafe void GetEntites()
        {
            Entity entity;
            if (IsManaged)
            {
                for (int i = 1; i < EntityCount; i++)
                    entity = _entities_Managed[i];
            }
            else
            {
                for (int i = 1; i < EntityCount; i++)
                    entity = _entities_Native[i];
            }
        }
    }*/
}
