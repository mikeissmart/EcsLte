using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_FilterByOne : BasePerformanceTest
    {
        public override void Run()
        {
            var filter = Filter.AllOf<TestComponent1>();
            EntityFilter entityFilter;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityFilter = _context.FilterBy(filter);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var filter = Filter.AllOf<TestComponent1>();
            EntityFilter entityFilter;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityFilter = _context.FilterBy(filter); });
        }
    }
}