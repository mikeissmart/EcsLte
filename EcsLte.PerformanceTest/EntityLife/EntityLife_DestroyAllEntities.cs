namespace EcsLte.PerformanceTest
{
    internal class EntityLife_DestroyAllEntities : BasePerformanceTest
    {
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.CreateEntity();
        }

        public override void Run()
        {
            _world.EntityManager.DestroyAllEntities();
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