using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_DestroyEntity : BasePerformanceTest
    {
        private Entity[] _entities;
        private Filter _filter;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _filter = Filter.AllOf<TestComponent1>();
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.DestroyEntity(_entities[i]);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _world.EntityManager.DestroyEntity(_entities[index]); });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}