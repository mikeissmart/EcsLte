using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Filter_Filtered : BasePerformanceTest
    {
        private Entity[] _entities;
        private Filter _filter;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            _filter = Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1>();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.EntityIsFiltered(_entities[i], _filter);
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
                    var result = _world.EntityManager.EntityIsFiltered(_entities[index], _filter);
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}