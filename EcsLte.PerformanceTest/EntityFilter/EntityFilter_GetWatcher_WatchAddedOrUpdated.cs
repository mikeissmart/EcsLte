using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_GetWatcher_WatchAddedOrUpdated : BasePerformanceTest
    {
        private EntityFilter _entityFilter;
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = Filter.AllOf<TestStandardComponent1>();
            _entityFilter = _context.FilterBy(_filter);
        }

        public override void Run()
        {
            Watcher watcher;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                watcher = _entityFilter.WatchAddedOrUpdated(_filter);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            Watcher watcher;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { watcher = _entityFilter.WatchAddedOrUpdated(_filter); });
        }
    }
}