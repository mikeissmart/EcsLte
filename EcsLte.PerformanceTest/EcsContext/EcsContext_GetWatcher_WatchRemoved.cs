using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EcsContext_GetWatcher_WatchRemoved : BasePerformanceTest
	{
		private Filter _filter;

		public override void PreRun()
		{
			base.PreRun();

			_filter = Filter.AllOf<TestStandardComponent1>();
		}

		public override void Run()
		{
			Watcher watcher;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				watcher = _context.WatchRemoved(_filter);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			Watcher watcher;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { watcher = _context.WatchRemoved(_filter); });
		}
	}
}