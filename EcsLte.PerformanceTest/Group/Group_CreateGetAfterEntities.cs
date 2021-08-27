using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_CreateGetAfterEntities : BasePerformanceTest
    {
        private World _world;
        private Filter _filter;
        private Entity[] _entities;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _filter = Filter.AllOf<TestComponent1>();
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override void Run()
        {
            var group = _world.GroupManager.GetGroup(_filter);
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var group = _world.GroupManager.GetGroup(_filter);
                });
        }

        public override void PostRun()
            => World.DestroyWorld(_world);
    }
}