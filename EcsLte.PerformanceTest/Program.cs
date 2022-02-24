using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class Program
	{
		private static Stopwatch _stopwatch;
		private static readonly StringBuilder _toCsv = new StringBuilder();
		private static int _testsRan = 0;

		private static void Main(string[] args)
		{
			Console.WriteLine("//Running performance tests...");
			Console.WriteLine();
			Console.WriteLine("//Name".PadRight(80) + "Time".PadRight(10) + "ParallelTime".PadRight(15) + "Memory MB");
			_toCsv.Append("Name,Time,ParallelTime,Memory MB" + Environment.NewLine);
			_stopwatch = new Stopwatch();

			var baseTestType = typeof(BasePerformanceTest);
			var tests = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x =>
					baseTestType.IsAssignableFrom(x) &&
					x != baseTestType)
				.OrderBy(x => x.Name)
				.GroupBy(x => x.Name.Split('_').Length >= 3
					? $"{x.Name.Split('_')[0]}_{x.Name.Split('_')[1]}"
					: x.Name.Split('_').Length == 2
						? x.Name.Split('_')[0]
						: x.Name)
				.ToList();
			var resultsCount = tests
				.Where(x => !x.Key.Contains("Misc"))
				.SelectMany(x => x)
				.Count();

			ObjectCache.IsCacheEnabled = false;

			foreach (var testGrouping in tests)
				if (!testGrouping.Key.Contains("Misc"))
				{
					foreach (var test in testGrouping)
						Run(test);
					Console.WriteLine("");
				}

			if (_testsRan == resultsCount)
			{
				var resultDir = $"{Directory.GetCurrentDirectory()}/Results/";
				if (!Directory.Exists(resultDir))
					Directory.CreateDirectory(resultDir);
				File.WriteAllText($"{resultDir}/PerformanceTest {DateTime.Now.ToString("yyyy-MM-dd HHmm")}.csv", _toCsv.ToString());
			}

			//Run(typeof(EcsContext_ComponentLife_ReplaceComponent_StandardX3));

			Console.WriteLine("");
			Console.WriteLine("Press any key to continue...");
			//#if RELEASE
			Console.ReadKey();
			//#endif
		}

		private static void Run(Type testType)
		{
			//Console.WriteLine($"//{testType.Name}");
			var loops = 5;
			var times = new long[loops];
			var memories = new long[loops];
			var paralleltimes = new long[loops];
			long avgTime = 0;
			long avgMemory = 0;
			long avgParallelTime = 0;
			for (var i = 0; i < loops; i++)
			{
				var test = (BasePerformanceTest)Activator.CreateInstance(testType);
				test.PreRun();
				_stopwatch.Reset();
				_stopwatch.Start();
				test.Run();
				_stopwatch.Stop();
				memories[i] = GetMemoryUsage();
				test.PostRun();

				GC.Collect();
				GC.WaitForPendingFinalizers();

				times[i] = _stopwatch.ElapsedMilliseconds;
				avgTime += _stopwatch.ElapsedMilliseconds;
				avgMemory += memories[i];

				test = (BasePerformanceTest)Activator.CreateInstance(testType);
				var parallelCount = test.CanRunParallel();
				if (test.CanRunParallel())
				{
					test.PreRun();
					_stopwatch.Reset();
					_stopwatch.Start();
					test.RunParallel();
					_stopwatch.Stop();
					test.PostRun();

					paralleltimes[i] = _stopwatch.ElapsedMilliseconds;
					avgParallelTime += _stopwatch.ElapsedMilliseconds;
				}
				else
				{
					avgParallelTime = -1;
				}

				/*Console.WriteLine(
                    "//".PadRight(60) +
                    $"{times[i]} ms".PadRight(10) +
                    $"{paralleltimes[i]} ms");*/
			}

			avgTime /= loops;
			avgMemory /= loops;
			avgParallelTime = avgParallelTime > 0
				? avgParallelTime /= loops
				: -1;
			Console.WriteLine(
				//"//" + new String('-', 57) + " " +
				$"//{testType.Name}".PadRight(80) +
				$"{avgTime} ms".PadRight(10) +
				$"{avgParallelTime} ms".PadRight(15) +
				$"{GetMemoryUsageReadable(avgMemory)}");
			_toCsv.Append($"{testType.Name},{avgTime},{avgParallelTime},{GetMemoryUsageReadable(avgMemory)}{Environment.NewLine}");
			_testsRan++;
		}

		private static string GetMemoryUsageReadable(long bytes) => string.Format("{0:0.##}", bytes / 1048576.0);

		private static long GetMemoryUsage()
		{
			var process = Process.GetCurrentProcess();
			return process.WorkingSet64;
		}
	}
}