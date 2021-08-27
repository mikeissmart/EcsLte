using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandPlayback
{
    internal class EntityCommandPlayback_EntityLife_CreateEntity : BasePerformanceTest
    {
        private World _world;
        private Entity[] _entities;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = new Entity[TestConsts.EntityLoopCount];
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _entities[i] = _world.EntityManager.DefaultEntityCommandPlayback.CreateEntity();
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    _entities[index] = _world.EntityManager.DefaultEntityCommandPlayback.CreateEntity();
                });
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}