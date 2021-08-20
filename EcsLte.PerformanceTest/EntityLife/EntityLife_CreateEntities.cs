namespace EcsLte.PerformanceTest
{
    internal class EntityLife_CreateEntities : BasePerformanceTest
    {
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
        }

        public override void Run()
        {
            var a = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override int ParallelRunCount()
        {
            return -1;
        }

        public override void RunParallel(int index, int startIndex, int endIndex)
        {
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}