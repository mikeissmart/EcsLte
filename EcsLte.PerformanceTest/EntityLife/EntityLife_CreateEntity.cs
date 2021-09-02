using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityLife_CreateEntity : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = new Entity[TestConsts.EntityLoopCount];
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _entities[i] = _world.EntityManager.CreateEntity();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _entities[index] = _world.EntityManager.CreateEntity(); });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}