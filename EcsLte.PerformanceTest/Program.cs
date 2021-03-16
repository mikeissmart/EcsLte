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

			Run<FilterEquals>();
			Run<FilterFiltered>();
			Run<FilterGetHasCode>();
			Run<EmptyTest>();

			Run<GroupCreate>();
			Run<GroupAutoUpdateAfterEntityCreate>();
			Run<GroupAutoUpdateBeforeEntityCreate>();
			Run<EmptyTest>();

			Run<PrimaryKeyCreate>();
			Run<PrimaryKeyAfterEntitiesGetEntity>();
			Run<PrimaryKeyeBeforeEntitiesGetEntity>();
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
			//EntityAddComponent:                   623 ms      583 ms      40
			//EntityGetComponent:                   106 ms      104 ms      2
			//EntityGetComponents:                  515 ms      536 ms      -21
			//EntityHasComponent:                   80 ms       78 ms       2
			//EntityRemoveComponent:                218 ms      232 ms      -14
			//EntityRemoveComponents:               345 ms      568 ms      -223

			//FilterEquals:                         12 ms       12 ms       0
			//FilterFiltered:                       35 ms       36 ms       -1
			//FilterGetHasCode:                     2 ms        2 ms        0

			//GroupCreate:                          53 ms       54 ms       -1
			//GroupAutoUpdateAfterEntityCreate:     251 ms      92 ms       159
			//GroupAutoUpdateBeforeEntityCreate:    2066 ms     2235 ms     -169

			//PrimaryKeyCreate:                     198 ms      0 ms        0
			//PrimaryKeyAfterEntitiesGetEntity:     109 ms      0 ms        0
			//PrimaryKeyeBeforeEntitiesGetEntity:   202 ms      0 ms        0

			//SharedKeyCreate:                      119 ms      118 ms      1
			//SharedKeyAfterEntitiesGetEntities:    636 ms      718 ms      -82
			//SharedKeyeBeforeEntitiesGetEntities:  190 ms      190 ms      0

			//WorldCreateEntity:                    1376 ms     1566 ms     -190
			//WorldDestroyEntity:                   525 ms      766 ms      -241
			//WorldDestroyAllEntities:              505 ms      746 ms      -241
			//WorldGetEntity:                       46 ms       34 ms       12
			//WorldHasEntity:                       46 ms       46 ms       0
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