namespace EcsLte.BenchmarkTest.EcsContextTests
{
    // Tests to quick
    /*public class EcsContext_UpdateComponent
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityBlueprint _blueprint;
        private EntityQuery _entityQuery;
        private TestComponent1 _updateComponent1;
        private TestComponent2 _updateComponent2;
        private TestSharedComponent1 _updateSharedComponent1;
        private TestSharedComponent2 _updateSharedComponent2;

        [ParamsAllValues]
        public ComponentArrangement CompArr { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context = EcsContext.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);
            _entityQuery = _context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();

            _updateComponent1 = EcsContextSetupCleanup.Component1;
            _updateComponent2 = EcsContextSetupCleanup.Component2;
            _updateSharedComponent1 = EcsContextSetupCleanup.SharedComponent1;
            _updateSharedComponent2 = EcsContextSetupCleanup.SharedComponent2;

            _updateComponent1.Prop++;
            _updateComponent2.Prop++;
            _updateSharedComponent1.Prop++;
            _updateSharedComponent2.Prop++;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContext.DestroyContext(_context);
        }

        [IterationSetup]
        public void IterationSetup() => _entities = _context.EntityManager.CreateEntities(_entities.Length, _blueprint);

        [IterationCleanup]
        public void IterationCleanup() => _context.EntityManager.DestroyEntities(_entities);

        [Benchmark]
        public void UpdateComponent_Entity()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateComponent1);
                    }
                    break;
                case ComponentArrangement.Normal_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateComponent2);
                    }
                    break;
                case ComponentArrangement.Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent1);
                    }
                    break;
                case ComponentArrangement.Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent2);
                    }
                    break;
                case ComponentArrangement.Normal_x1_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent1);
                    }
                    break;
                case ComponentArrangement.Normal_x1_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent2);
                    }
                    break;
                case ComponentArrangement.Normal_x2_Shared_x1:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateComponent2);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent1);
                    }
                    break;
                case ComponentArrangement.Normal_x2_Shared_x2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.EntityManager.UpdateComponent(entity, _updateComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateComponent2);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent1);
                        _context.EntityManager.UpdateComponent(entity, _updateSharedComponent2);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [Benchmark]
        public void UpdateComponent_Entities()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_x1:
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent1);
                    break;
                case ComponentArrangement.Normal_x2:
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent2);
                    break;
                case ComponentArrangement.Shared_x1:
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent1);
                    break;
                case ComponentArrangement.Shared_x2:
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent2);
                    break;
                case ComponentArrangement.Normal_x1_Shared_x1:
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent1);
                    break;
                case ComponentArrangement.Normal_x1_Shared_x2:
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent2);
                    break;
                case ComponentArrangement.Normal_x2_Shared_x1:
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent2);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent1);
                    break;
                case ComponentArrangement.Normal_x2_Shared_x2:
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateComponent2);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent1);
                    _context.EntityManager.UpdateComponent(_entities, _updateSharedComponent2);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [Benchmark]
        public void UpdateComponent_EntityQuery()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_x1:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent1);
                    break;
                case ComponentArrangement.Normal_x2:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent2);
                    break;
                case ComponentArrangement.Shared_x1:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent1);
                    break;
                case ComponentArrangement.Shared_x2:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent2);
                    break;
                case ComponentArrangement.Normal_x1_Shared_x1:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent1);
                    break;
                case ComponentArrangement.Normal_x1_Shared_x2:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent2);
                    break;
                case ComponentArrangement.Normal_x2_Shared_x1:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent2);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent1);
                    break;
                case ComponentArrangement.Normal_x2_Shared_x2:
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateComponent2);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent1);
                    _context.EntityManager.UpdateComponent(_entityQuery, _updateSharedComponent2);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }*/
}
