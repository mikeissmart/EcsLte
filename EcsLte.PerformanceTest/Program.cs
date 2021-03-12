using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EcsLte.PerformanceTest
{
	internal class Program
	{
		private readonly static string _fileName = "TestHistory.json";
		private static Stopwatch _stopwatch;
		private static List<TestHistory> _pastTestHistories;
		private static List<TestHistory> _currentTestHistories;

		private static void Main(string[] args)
		{
			Console.WriteLine("//Running performance tests...");
			Console.WriteLine();
			Console.WriteLine("//Name".PadRight(40) + "Cur Time".PadRight(12) + "Pre Time".PadRight(12) + "Diff");
			_stopwatch = new Stopwatch();
			_pastTestHistories = SaveLoadTestHistory.Load(_fileName);
			_currentTestHistories = new List<TestHistory>();

			Run<EntityAddComponent>();
			Run<EntityGetComponent>();
			Run<EntityGetComponents>();
			Run<EntityHasComponent>();
			Run<EntityRemoveComponent>();
			Run<EntityRemoveComponents>();
			Run<EmptyTest>();

			Run<GroupCreate>();
			Run<GroupAutoUpdateAfterEntityCreate>();
			Run<GroupAutoUpdateBeforeEntityCreate>();
			Run<EmptyTest>();

			Run<WorldCreateEntity>();
			Run<WorldDestroyEntity>();
			Run<WorldDestroyAllEntities>();
			Run<WorldGetEntity>();
			Run<WorldHasEntity>();
			//Run<WorldJsonSerialize>();
			Run<EmptyTest>();

			//SaveLoadTestHistory.Save(_fileName, _currentTestHistories);
			Console.WriteLine("\nPress any key...");
			Console.Read();

			//Running performance tests...
			//Name                                  Cur Time    Pre Time    Diff
			//EntityAddComponent:                   296 ms      336 ms      -40
			//EntityGetComponent:                   169 ms      139 ms      30
			//EntityGetComponents:                  878 ms      989 ms      -111
			//EntityHasComponent:                   94 ms       65 ms       29
			//EntityRemoveComponent:                309 ms      286 ms      23
			//EntityRemoveComponents:               344 ms      290 ms      54

			//GroupCreate:                          512 ms      515 ms      -3
			//GroupAutoUpdateAfterEntityCreate:     103 ms      206 ms      -103
			//GroupAutoUpdateBeforeEntityCreate:    2100 ms     2020 ms     80

			//WorldCreateEntity:                    1381 ms     1433 ms     -52
			//WorldDestroyEntity:                   547 ms      531 ms      16
			//WorldDestroyAllEntities:              537 ms      607 ms      -70
			//WorldGetEntity:                       44 ms       64 ms       -20
			//WorldHasEntity:                       51 ms       32 ms       19
		}

		private static void Run<TPerformanceTest>()
			where TPerformanceTest : IPerformanceTest, new()
		{
			if (typeof(TPerformanceTest) == typeof(EmptyTest))
			{
				Console.WriteLine();
				return;
			}

			TPerformanceTest test = default;

			long avgTime = 0;
			int loops = 5;
			for (int i = 0; i < loops; i++)
			{
				test = new TPerformanceTest();
				test.PreRun();
				_stopwatch.Reset();
				_stopwatch.Start();
				test.Run();
				_stopwatch.Stop();
				test.PostRun();
				avgTime += _stopwatch.ElapsedMilliseconds;
			}
			avgTime /= loops;

			var testHistory = new TestHistory
			{
				Name = test.GetType().Name,
				TimeMs = avgTime
			};
			_currentTestHistories.Add(testHistory);

			long timeDiff = 0;
			var prevTestHistory = _pastTestHistories.FirstOrDefault(x => x.Name == testHistory.Name);
			if (prevTestHistory != null)
				timeDiff = testHistory.TimeMs - prevTestHistory.TimeMs;

			Console.WriteLine(
				($"//{typeof(TPerformanceTest).Name}: ").PadRight(40) +
				($"{avgTime} ms ").PadRight(12) +
				($"{prevTestHistory?.TimeMs ?? 0} ms ").PadRight(12) +
				$"{timeDiff}");
		}
	}
}