using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_CreateGet : BasePerformanceTest
    {
        private World _world;
        private Filter _filter;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _filter = Filter.AllOf<TestComponent1>();
        }

        public override void Run()
        {
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.GroupManager.GetGroup(_filter);
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var group = _world.GroupManager.GetGroup(_filter);
                });
        }

        public override void PostRun()
            => World.DestroyWorld(_world);
    }
}