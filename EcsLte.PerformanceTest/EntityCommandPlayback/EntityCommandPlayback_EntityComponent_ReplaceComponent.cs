using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandPlayback
{
    internal class EntityCommandPlayback_EntityComponent_ReplaceComponent : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = new Entity[TestConsts.EntityLoopCount];

            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _entities[i] = _world.EntityManager.CreateEntity();
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
            }
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.DefaultEntityCommandPlayback.ReplaceComponent(_entities[i], new TestComponent1());
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    _world.EntityManager.DefaultEntityCommandPlayback.ReplaceComponent(_entities[index],
                        new TestComponent1());
                });
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}