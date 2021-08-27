using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityLife_DestroyEntity : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = new Entity[TestConsts.EntityLoopCount];

            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _entities[i] = _world.EntityManager.CreateEntity();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.DestroyEntity(_entities[i]);
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    _world.EntityManager.DestroyEntity(_entities[index]);
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}