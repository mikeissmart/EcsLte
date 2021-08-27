using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Filter_Filtered : BasePerformanceTest
    {
        private Entity _entity;
        private Filter _filter;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entity = _world.EntityManager.CreateEntity();
            _filter = Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1>();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.EntityIsFiltered(_entity, _filter);
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var result = _world.EntityManager.EntityIsFiltered(_entity, _filter);
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}