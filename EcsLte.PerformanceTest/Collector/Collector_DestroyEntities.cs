namespace EcsLte.PerformanceTest
{
    internal class Collector_DestroyEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _world.CollectorManager.GetCollector(CollectorTrigger.Added(Filter.AllOf<TestComponent1>()));
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override void Run()
        {
            _world.EntityManager.DestroyEntities(_entities);
        }

        public override bool CanRunParallel()
        {
            return false;
        }

        public override void RunParallel()
        {
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}