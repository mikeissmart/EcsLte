using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_EntityGroup_SharedComponentX2 : BasePerformanceTest
    {
        private EntityFilter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
        }

        public override void Run()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            EntityGroup entityGroup;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _filter.GroupWith(component1, component2);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 2 };
            EntityGroup entityGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityGroup = _filter.GroupWith(component1, component2); });
        }
    }
}