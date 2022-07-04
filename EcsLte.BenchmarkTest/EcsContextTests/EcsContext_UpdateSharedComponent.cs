using BenchmarkDotNet.Attributes;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_UpdateSharedComponent
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityBlueprint _blueprint;
        private EntityArcheType _archeType1;
        private EntityArcheType _archeType2;
        private EntityArcheType _archeType3;
        private EntityArcheType _archeType4;
        private EntityQuery _query1;
        private EntityQuery _query2;
        private EntityQuery _query3;
        private EntityQuery _query4;

        private TestSharedComponent1 SharedComponent1 = new TestSharedComponent1 { Prop = 6 };
        private TestSharedComponent2 SharedComponent2 = new TestSharedComponent2 { Prop = 7 };
        private TestSharedComponent3 SharedComponent3 = new TestSharedComponent3 { Prop = 8 };
        private TestSharedComponent4 SharedComponent4 = new TestSharedComponent4 { Prop = 9 };

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Shared_x4);

            CreateArcheTypes();
        }

        private EntityArcheType BlueprintUpdateAndArcheType<T>(T component) where T : unmanaged, IComponent =>
            _blueprint.UpdateComponent(component).GetEntityArcheType();

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.DestroyContext(_context);
        }

        [IterationSetup]
        public void IterationSetup() => _entities = _context.CreateEntities(_entities.Length, _blueprint);

        [IterationCleanup]
        public void IterationCleanup() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void UpdateSharedComponent_Entities()
        {
            _context.UpdateSharedComponent(_entities, SharedComponent1);
            _context.UpdateSharedComponent(_entities, SharedComponent2);
            _context.UpdateSharedComponent(_entities, SharedComponent3);
            _context.UpdateSharedComponent(_entities, SharedComponent4);
        }

        [Benchmark]
        public void UpdateSharedComponent_EntityArcheType()
        {
            _context.UpdateSharedComponent(_archeType1, SharedComponent1);
            _context.UpdateSharedComponent(_archeType2, SharedComponent2);
            _context.UpdateSharedComponent(_archeType3, SharedComponent3);
            _context.UpdateSharedComponent(_archeType4, SharedComponent4);
        }

        [Benchmark]
        public void UpdateSharedComponent_EntityQuery()
        {
            _context.UpdateSharedComponent(_query1, SharedComponent1);
            _context.UpdateSharedComponent(_query2, SharedComponent2);
            _context.UpdateSharedComponent(_query3, SharedComponent3);
            _context.UpdateSharedComponent(_query4, SharedComponent4);
        }

        private void CreateArcheTypes()
        {
            _archeType1 = _blueprint.GetEntityArcheType();
            _archeType2 = BlueprintUpdateAndArcheType(SharedComponent1);
            _archeType3 = BlueprintUpdateAndArcheType(SharedComponent2);
            _archeType4 = BlueprintUpdateAndArcheType(SharedComponent3);

            _query1 = new EntityQuery().WhereAllOf(_archeType1);
            _query2 = new EntityQuery().WhereAllOf(_archeType2);
            _query3 = new EntityQuery().WhereAllOf(_archeType3);
            _query4 = new EntityQuery().WhereAllOf(_archeType4);
        }
    }
}
