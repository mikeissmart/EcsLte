using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EntityFilterGroup_GetWatcher_WatchUpdated : BasePerformanceTest
	{
		private EntityFilterGroup _entityFilterGroup;
		private Filter _filter;

		public override void PreRun()
		{
			base.PreRun();

			_filter = Filter.AllOf<TestSharedComponent1, TestStandardComponent1>();
			_entityFilterGroup = _context.FilterByGroupWith(_filter, new TestSharedComponent1());
		}

		public override void Run()
		{
			Watcher watcher;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				watcher = _entityFilterGroup.WatchAdded(_filter);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			Watcher watcher;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { watcher = _entityFilterGroup.WatchAdded(_filter); });
		}
	}
}