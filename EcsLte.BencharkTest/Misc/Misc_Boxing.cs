using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace EcsLte.BencharkTest
{
	/*[MemoryDiagnoser]
	public class Misc_Boxing
	{
		private interface ITest { }

		private class Test : ITest { }

		private ITest[] _tests;
		private Test _test;

		[GlobalSetup]
		public void Setup()
		{
			_test = new Test();
			_tests = new Test[TestConsts.LargeCount];
		}

		[GlobalCleanup]
		public void Cleanup()
		{

		}

		[Benchmark]
		public void Boxing()
		{
			for (int i = 0; i < _tests.Length; i++)
				_tests[i] = _test;
		}
	}*/
}
