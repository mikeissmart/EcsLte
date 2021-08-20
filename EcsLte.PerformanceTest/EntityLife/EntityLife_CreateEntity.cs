namespace EcsLte.PerformanceTest
{
    internal class EntityLife_CreateEntity : BasePerformanceTest
    {
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.CreateEntity();
        }

        public override int ParallelRunCount()
        {
            return TestConsts.EntityLoopCount;
        }

        public override void RunParallel(int index, int startIndex, int endIndex)
        {
            _world.EntityManager.CreateEntity();
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}