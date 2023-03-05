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
            if (EcsContexts.Instance.HasContext("Test"))
                EcsContexts.Instance.DestroyContext(EcsContexts.Instance.GetContext("Test"));
            _context = EcsContexts.Instance.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Shared_x4);

            CreateArcheTypes();
        }

        private EntityArcheType BlueprintUpdateAndArcheType<T>(T component) where T : unmanaged, ISharedComponent =>
            _blueprint.SetSharedComponent(component).GetArcheType(_context);

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
        public void UpdateSharedComponent_EntityArcheType()
        {
            _context.Entities.UpdateSharedComponent(_archeType1, SharedComponent1);
            _context.Entities.UpdateSharedComponent(_archeType2, SharedComponent2);
            _context.Entities.UpdateSharedComponent(_archeType3, SharedComponent3);
            _context.Entities.UpdateSharedComponent(_archeType4, SharedComponent4);
        }

        // todo might not want to update with queries because of chunk lvl changes
        /*[Benchmark]
        public void UpdateSharedComponent_EntityQuery()
        {
            _context.Entities.UpdateSharedComponents(_query1, SharedComponent1);
            _context.Entities.UpdateSharedComponents(_query2, SharedComponent2);
            _context.Entities.UpdateSharedComponents(_query3, SharedComponent3);
            _context.Entities.UpdateSharedComponents(_query4, SharedComponent4);
        }*/

        private void CreateArcheTypes()
        {
            _archeType1 = _blueprint.GetArcheType(_context);
            _archeType2 = BlueprintUpdateAndArcheType(SharedComponent1);
            _archeType3 = BlueprintUpdateAndArcheType(SharedComponent2);
            _archeType4 = BlueprintUpdateAndArcheType(SharedComponent3);

            _query1 = _context.Queries
                .SetFilter(_context.Filters
                    .WhereAllOf(_archeType1));
            _query2 = _context.Queries
                .SetFilter(_context.Filters
                    .WhereAllOf(_archeType2));
            _query3 = _context.Queries
                .SetFilter(_context.Filters
                    .WhereAllOf(_archeType3));
            _query4 = _context.Queries
                .SetFilter(_context.Filters
                    .WhereAllOf(_archeType4));
        }
    }
}
