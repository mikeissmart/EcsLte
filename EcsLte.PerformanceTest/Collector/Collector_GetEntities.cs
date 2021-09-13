using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Collector_GetEntities : BasePerformanceTest
    {
        private Collector _collector;
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _collector = _world.CollectorManager.GetCollector(CollectorTrigger.Added(Filter.AllOf<TestComponent1>()));
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override void Run()
        {
            _entities = _collector.GetEntities();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _entities = _collector.GetEntities(); });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}