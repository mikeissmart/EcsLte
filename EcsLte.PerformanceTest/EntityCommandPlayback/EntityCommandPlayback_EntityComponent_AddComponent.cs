using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandPlayback_EntityComponent_AddComponent : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;
        private TestComponent1 _component;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            _component = new TestComponent1();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.DefaultEntityCommandPlayback.AddComponent(_entities[i], _component);
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
                    _world.EntityManager.DefaultEntityCommandPlayback.AddComponent(_entities[index],
                        _component);
                });
            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}