using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_CreateGetBeforeEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _world.EntityManager.AddComponent(_entities[index], new TestComponent1()); });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}