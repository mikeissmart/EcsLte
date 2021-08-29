using System;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Collector_CreateGet : BasePerformanceTest
    {
        private World _world;
        private Group _group;
        private CollectorTrigger _collectorTrigger;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            _collectorTrigger = CollectorTrigger.Added<TestComponent1>();
        }

        public override void Run()
        {
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            { var collector = _group.GetCollector(_collectorTrigger); }
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var collector = _group.GetCollector(_collectorTrigger);
                });
        }

        public override void PostRun()
            => World.DestroyWorld(_world);
    }
}