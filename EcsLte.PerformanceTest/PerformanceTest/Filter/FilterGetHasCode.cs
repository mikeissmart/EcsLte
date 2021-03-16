namespace EcsLte.PerformanceTest
{
	internal class FilterGetHasCode : IPerformanceTest
	{
		private Filter _filter;

		public void PreRun()
		{
			_filter = Filter.AllOf<TestComponent1, TestComponent2, TestrecordableComponent1>();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_filter.GetHashCode();
		}

		public void PostRun()
		{
		}
	}
}