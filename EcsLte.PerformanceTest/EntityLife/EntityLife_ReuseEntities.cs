namespace EcsLte.PerformanceTest
{
    internal class EntityLife_ReuseEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            _world.EntityManager.DestroyEntities(_entities);
        }

        public override void Run()
        {
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
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