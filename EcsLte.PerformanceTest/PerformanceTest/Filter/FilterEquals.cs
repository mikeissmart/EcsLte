namespace EcsLte.PerformanceTest
{
	internal class FilterEquals : IPerformanceTest
	{
		private Filter _filter1;
		private Filter _filter2;

		public void PreRun()
		{
			_filter1 = Filter.AllOf<TestComponent1, TestComponent2, TestrecordableComponent1>();
			_filter2 = Filter.AllOf<TestComponent1, TestComponent2, TestrecordableComponent1>();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_filter1.Equals(_filter2);
		}

		public void PostRun()
		{
		}
	}
}