namespace EcsLte.PerformanceTest.EntityCommandPlayback
{
    internal class EntityCommandPlayback_EntityLife_DestroyEntities : BasePerformanceTest
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
            _entities = _world.EntityManager.DefaultEntityCommandPlayback.CreateEntities(TestConsts.EntityLoopCount);
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override bool CanRunParallel()
            => false;

        public override void RunParallel()
        {
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}