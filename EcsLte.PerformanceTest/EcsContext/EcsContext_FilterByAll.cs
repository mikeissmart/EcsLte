using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_FilterByAll : BasePerformanceTest
    {
        public override void Run()
        {
            var filter = Filter.AllOfComponentIndexes();
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
            var filter = Filter.AllOfComponentIndexes();
            EntityFilter entityFilter;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityFilter = _context.FilterBy(filter); });
        }
    }
}