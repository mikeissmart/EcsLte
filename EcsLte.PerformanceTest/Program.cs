using System;
using System.Diagnostics;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Program
    {
        private static Stopwatch _stopwatch;

        private static void Main(string[] args)
        {
            Console.WriteLine("//Running performance tests...");
            Console.WriteLine();
            Console.WriteLine("//Name".PadRight(60) + "Time".PadRight(10) + "ParallelTime");
            _stopwatch = new Stopwatch();

            var baseTestType = typeof(BasePerformanceTest);
            var tests = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    baseTestType.IsAssignableFrom(x) &&
                    x != baseTestType)
                .OrderBy(x => x.Name)
                .GroupBy(x => x.Name.Split('_')[0])
                .ToList();

            foreach (var testGrouping in tests)
            {
                if (true)//(testGrouping.Key == "EntityCommandPlayback")
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }
            }

            //Name                                                      Time      ParallelTime
            //EntityCommandPlayback_EntityComponent_AddComponent        381 ms    622 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     287 ms    490 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    382 ms    619 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           171 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             235 ms    555 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          189 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            325 ms    572 ms

            //EntityComponent_AddComponent                              245 ms    181 ms
            //EntityComponent_GetAllComponents                          196 ms    81 ms
            //EntityComponent_GetComponent                              68 ms     6 ms
            //EntityComponent_RemoveAllComponents                       258 ms    131 ms
            //EntityComponent_RemoveComponent                           120 ms    52 ms
            //EntityComponent_ReplaceComponent                          270 ms    190 ms

            //EntityLife_CreateEntities                                 76 ms     -1 ms
            //EntityLife_CreateEntity                                   119 ms    801 ms
            //EntityLife_DestroyEntities                                191 ms    -1 ms
            //EntityLife_DestroyEntity                                  194 ms    400 ms
            //EntityLife_HasEntity                                      34 ms     4 ms

            //Filter_Equals                                             15 ms     2 ms
            //Filter_Filtered                                           220 ms    393 ms

            //Group_ContainsEntity                                      20 ms     3 ms
            //Group_CreateGet                                           71 ms     255 ms
            //Group_CreateGetAfterEntities                              329 ms    622 ms
            //Group_CreateGetBeforeEntities                             566 ms    770 ms

            //Misc_ConcurrentDic_TryAdd                                 0 ms      175 ms
            //Misc_ConcurrentDic_TryGetValue_DoesHave                   0 ms      2 ms
            //Misc_ConcurrentDic_TryGetValue_DoesntHave                 0 ms      1 ms
            //Misc_ListLock                                             0 ms      153 ms

            Console.WriteLine("Press any key to continue...");
#if RELEASE
            Console.ReadKey();
#endif
        }

        private static void Run(Type testType)
        {
            //Console.WriteLine($"//{testType.Name}");
            var loops = 5;
            var times = new long[loops];
            var paralleltimes = new long[loops];
            long avgTime = 0;
            long avgParallelTime = 0;
            for (var i = 0; i < loops; i++)
            {
                var test = (BasePerformanceTest)Activator.CreateInstance(testType);
                test.PreRun();
                _stopwatch.Reset();
                _stopwatch.Start();
                test.Run();
                _stopwatch.Stop();
                test.PostRun();

                times[i] = _stopwatch.ElapsedMilliseconds;
                avgTime += _stopwatch.ElapsedMilliseconds;

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
            avgParallelTime = avgParallelTime > 0
                ? avgParallelTime /= loops
                : -1;
            Console.WriteLine(
                //"//" + new String('-', 57) + " " +
                $"//{testType.Name}".PadRight(60) +
                $"{avgTime} ms".PadRight(10) +
                $"{avgParallelTime} ms");
        }
    }
}