using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_FilterAll_ : BasePerformanceTest
    {
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = Filter.AllOfComponentIndexes();
        }

        public override void Run()
        {
            EntityFilter entityFilter;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityFilter = _context.FilterBy(_filter);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            EntityFilter entityFilter;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityFilter = _context.FilterBy(_filter); });
        }
    }
}