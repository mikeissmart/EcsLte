using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandPlayback
{
    internal class EntityCommandPlayback_EntityLife_CreateEntities : BasePerformanceTest
    {
        private World _world;
        private Entity[] _entities;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
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