using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_GetEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private Group _group;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override void Run()
        {
            _entities = _group.GetEntities();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _entities = _group.GetEntities(); });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}