using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Filter_Filtered_ManyComponents : BasePerformanceTest
    {
        private Entity _entity;
        private Filter _filter;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddAllComponents(_entity);
            _filter = Filter.AllOf(ComponentIndexes.Instance.AllComponentIndexes);
        }

        public override void Run()
        {
            bool result;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                result = _world.EntityManager.EntityIsFiltered(_entity, _filter);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool result;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { result = _world.EntityManager.EntityIsFiltered(_entity, _filter); });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}