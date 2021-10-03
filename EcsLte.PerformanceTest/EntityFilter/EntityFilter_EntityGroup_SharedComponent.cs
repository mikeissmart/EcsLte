using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_EntityGroup_SharedComponent : BasePerformanceTest
    {
        private EntityFilter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
        }

        public override void Run()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            EntityGroup entityGroup;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _filter.GroupWith(component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            EntityGroup entityGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityGroup = _filter.GroupWith(component); });
        }
    }
}