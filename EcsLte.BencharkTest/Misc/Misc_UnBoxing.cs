namespace EcsLte.BencharkTest
{
    /*[MemoryDiagnoser]
	public class Misc_UnBoxing
	{
		private interface ITest { }

		private class Test : ITest	{ }

		private ITest[] _tests;

		[GlobalSetup]
		public void Setup()
		{
			_tests = new Test[TestConsts.LargeCount];
			for (int i = 0; i < _tests.Length; i++)
				_tests[i] = new Test();
		}

		[GlobalCleanup]
		public void Cleanup()
		{

		}

		[Benchmark]
		public void Unboxing()
		{
			Test test;
			for (int i = 0; i < _tests.Length;i++)
				test = (Test)_tests[i];
		}
	}*/
}
