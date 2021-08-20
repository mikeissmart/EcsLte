namespace EcsLte.PerformanceTest
{
    internal class EntityLife_DestroyEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            _world.EntityManager.DestroyEntities(_entities);
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