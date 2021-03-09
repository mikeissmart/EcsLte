using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EcsLte.PerformanceTest
{
	internal class Program
	{
		private static string _fileName = "TestHistory.json";
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

			Run<WorldCreateEntity>();
			Run<WorldDestroyEntity>();
			Run<WorldDestroyAllEntities>();
			Run<WorldGetEntity>();
			Run<WorldHasEntity>();
			//Run<WorldJsonSerialize>();
			Run<EmptyTest>();

			Run<EntityAddComponent>();
			Run<EntityGetComponent>();
			Run<EntityGetComponents>();
			Run<EntityHasComponent>();
			Run<EntityRemoveComponent>();
			Run<EntityRemoveComponents>();
			Run<EmptyTest>();

			SaveLoadTestHistory.Save(_fileName, _currentTestHistories);
			Console.WriteLine("\nPress any key...");
			Console.Read();

			//Running performance tests...
			//Name                                  Cur Time    Pre Time    Diff
			//WorldCreateEntity:                    512 ms      514 ms      -2
			//WorldDestroyEntity:                   392 ms      413 ms      -21
			//WorldDestroyAllEntities:              362 ms      366 ms      -4
			//WorldGetEntity:                       21 ms       21 ms       0
			//WorldHasEntity:                       17 ms       17 ms       0

			//EntityAddComponent:                   206 ms      196 ms      10
			//EntityGetComponent:                   75 ms       69 ms       6
			//EntityGetComponents:                  655 ms      635 ms      20
			//EntityHasComponent:                   34 ms       34 ms       0
			//EntityRemoveComponent:                142 ms      126 ms      16
			//EntityRemoveComponents:               315 ms      340 ms      -25
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