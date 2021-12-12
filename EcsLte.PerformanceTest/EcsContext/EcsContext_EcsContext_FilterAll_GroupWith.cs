using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EcsContext_EcsContext_FilterAll_GroupWith : BasePerformanceTest
	{
		private TestSharedComponent1 _component;
		private Filter _filter;

		public override void PreRun()
		{
			base.PreRun();

			_component = new TestSharedComponent1();
			_filter = Filter.AllOfComponentIndexes();
		}

		public override void Run()
		{
			EntityFilterGroup filterGroup;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				filterGroup = _context.FilterByGroupWith(_filter, _component);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			EntityFilterGroup filterGroup;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { filterGroup = _context.FilterByGroupWith(_filter, _component); });
		}
	}
}