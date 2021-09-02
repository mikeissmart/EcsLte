using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandPlayback
{
    internal class EntityCommandPlayback_EntityLife_DestroyEntity : BasePerformanceTest
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
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.DefaultEntityCommandPlayback.DestroyEntity(_entities[i]);
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _world.EntityManager.DefaultEntityCommandPlayback.DestroyEntity(_entities[index]); });
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}