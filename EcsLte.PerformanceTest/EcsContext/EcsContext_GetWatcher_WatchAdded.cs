using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetWatcher_WatchAdded : BasePerformanceTest
    {
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = Filter.AllOf<TestComponent1>();
        }

        public override void Run()
        {
            Watcher watcher;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                watcher = _context.WatchAdded(_filter);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            Watcher watcher;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { watcher = _context.WatchAdded(_filter); });
        }
    }
}