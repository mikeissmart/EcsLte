using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EntityGroup_GetWatcher_WatchAdded : BasePerformanceTest
	{
		private EntityGroup _entityGroup;
		private Filter _filter;

		public override void PreRun()
		{
			base.PreRun();

			_filter = Filter.AllOf<TestSharedComponent1>();
			_entityGroup = _context.GroupWith(new TestSharedComponent1 { Prop = 1 });
		}

		public override void Run()
		{
			Watcher watcher;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				watcher = _entityGroup.WatchAdded(_filter);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			Watcher watcher;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { watcher = _entityGroup.WatchAdded(_filter); });
		}
	}
}