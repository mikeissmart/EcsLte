using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityComponent_GetAllComponents : BasePerformanceTest
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
                _world.EntityManager.GetAllComponents(_entities[i]);
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var result = _world.EntityManager.GetAllComponents(_entities[index]);
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}