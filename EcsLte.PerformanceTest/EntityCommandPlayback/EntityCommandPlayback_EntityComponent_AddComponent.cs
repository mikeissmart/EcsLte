using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandPlayback
{
    internal class EntityCommandPlayback_EntityComponent_AddComponent : BasePerformanceTest
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
                _world.EntityManager.DefaultEntityCommandPlayback.AddComponent(_entities[i], new TestComponent1());
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    _world.EntityManager.DefaultEntityCommandPlayback.AddComponent(_entities[index], new TestComponent1());
                });
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}