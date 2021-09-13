using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Collector_CreateGet : BasePerformanceTest
    {
        private CollectorTrigger _collectorTrigger;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _collectorTrigger = CollectorTrigger.Added(Filter.AllOf<TestComponent1>());
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                var collector = _world.CollectorManager.GetCollector(_collectorTrigger);
            }
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var collector = _world.CollectorManager.GetCollector(_collectorTrigger);
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}