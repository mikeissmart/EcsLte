using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EcsContext_EcsContext_FilterOne_GroupWithX2 : BasePerformanceTest
	{
		private TestSharedComponent1 _component1;
		private TestSharedComponent2 _component2;
		private Filter _filter;

		public override void PreRun()
		{
			base.PreRun();

			_component1 = new TestSharedComponent1 { Prop = 1 };
			_component2 = new TestSharedComponent2 { Prop = 2 };
			_filter = Filter.AllOf<TestStandardComponent1>();
		}

		public override void Run()
		{
			EntityFilterGroup filterGroup;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				filterGroup = _context.FilterByGroupWith(_filter, _component1, _component2);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			EntityFilterGroup filterGroup;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { filterGroup = _context.FilterByGroupWith(_filter, _component1, _component2); });
		}
	}
}