using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityGroup_GetWatcher_Added : BasePerformanceTest
    {
        private EntityGroup _entityGroup;
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = Filter.AllOf<TestSharedKeyComponent1>();
            _entityGroup = _context.GroupWith(new TestSharedKeyComponent1 { Prop = 1 });
        }

        public override void Run()
        {
            Watcher watcher;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                watcher = _entityGroup.Added(_filter);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            Watcher watcher;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { watcher = _entityGroup.Added(_filter); });
        }
    }
}