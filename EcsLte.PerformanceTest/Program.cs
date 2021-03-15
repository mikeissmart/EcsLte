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

			Run<SharedKeyCreate>();
			Run<SharedKeyAfterEntitiesGetEntities>();
			Run<SharedKeyeBeforeEntitiesGetEntities>();
			Run<EmptyTest>();

			Run<WorldCreateEntity>();
			Run<WorldDestroyEntity>();
			Run<WorldDestroyAllEntities>();
			Run<WorldGetEntity>();
			Run<WorldHasEntity>();
			//Run<WorldJsonSerialize>();
			Run<EmptyTest>();

			Console.WriteLine("\nPress 1 to save time...");
			if (Console.ReadKey().KeyChar == '1')
				SaveLoadTestHistory.Save(_fileName, _currentTestHistories);

			//Running performance tests...
			//Name                                  Cur Time    Pre Time    Diff
			//EntityAddComponent:                   576 ms      336 ms      240
			//EntityGetComponent:                   104 ms      139 ms      -35
			//EntityGetComponents:                  581 ms      989 ms      -408
			//EntityHasComponent:                   74 ms       65 ms       9
			//EntityRemoveComponent:                213 ms      286 ms      -73
			//EntityRemoveComponents:               359 ms      290 ms      69

			//GroupCreate:                          513 ms      515 ms      -2
			//GroupAutoUpdateAfterEntityCreate:     86 ms       206 ms      -120
			//GroupAutoUpdateBeforeEntityCreate:    2147 ms     2020 ms     127

			//SharedKeyCreate:                      458 ms      0 ms        0
			//SharedKeyAfterEntitiesGetEntities:    1535 ms     0 ms        0
			//SharedKeyeBeforeEntitiesGetEntities:  614 ms      0 ms        0

			//WorldCreateEntity:                    1540 ms     1433 ms     107
			//WorldDestroyEntity:                   539 ms      531 ms      8
			//WorldDestroyAllEntities:              547 ms      607 ms      -60
			//WorldGetEntity:                       33 ms       64 ms       -31
			//WorldHasEntity:                       46 ms       32 ms       14
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

			int loops = 5;
			long[] times = new long[loops];
			long avgTime = 0;
			for (int i = 0; i < loops; i++)
			{
				test = new TPerformanceTest();
				test.PreRun();
				_stopwatch.Reset();
				_stopwatch.Start();
				test.Run();
				_stopwatch.Stop();
				test.PostRun();

				times[i] = _stopwatch.ElapsedMilliseconds;
				avgTime += _stopwatch.ElapsedMilliseconds;

				string timesText = "";
				foreach (var time in times)
					timesText += $"{time}".PadRight(5);

				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write(
					($"//{typeof(TPerformanceTest).Name}: ") + timesText, 0);
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

			Console.SetCursorPosition(0, Console.CursorTop);
			Console.WriteLine(
				($"//{typeof(TPerformanceTest).Name}: ").PadRight(40) +
				($"{avgTime} ms ").PadRight(12) +
				($"{prevTestHistory?.TimeMs ?? 0} ms ").PadRight(12) +
				$"{timeDiff}");
		}
	}
}