using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Program
    {
        private static Stopwatch _stopwatch;
        private static string _toCsv = "";

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

            ObjectCache.IsCacheEnabled = false;

            foreach (var testGrouping in tests)
                if (testGrouping.Key != "Misc")
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }

            //Name                                                      Time      ParallelTime
            //Collector_CreateGet                                       205 ms    337 ms
            //Collector_CreateGetBeforeEntities                         769 ms    616 ms
            //Collector_DestroyEntities                                 801 ms    -1 ms
            //Collector_DestroyEntity                                   813 ms    1234 ms
            //Collector_GetEntities                                     20 ms     152 ms

            //CollectorTrigger_Create                                   1158 ms   1151 ms
            //CollectorTrigger_Equals                                   185 ms    16 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        550 ms    760 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     268 ms    458 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    282 ms    538 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           291 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             553 ms    695 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          350 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            350 ms    539 ms

            //EntityComponent_AddComponent                              304 ms    211 ms
            //EntityComponent_GetAllComponents                          159 ms    52 ms
            //EntityComponent_GetComponent                              114 ms    46 ms
            //EntityComponent_GetRandEntityComponent                    349 ms    111 ms
            //EntityComponent_HasComponent                              56 ms     5 ms
            //EntityComponent_RemoveAllComponents                       232 ms    84 ms
            //EntityComponent_RemoveComponent                           125 ms    10 ms
            //EntityComponent_ReplaceComponent                          132 ms    49 ms

            //EntityLife_CreateEntities                                 148 ms    -1 ms
            //EntityLife_CreateEntity                                   429 ms    709 ms
            //EntityLife_DestroyEntities                                197 ms    -1 ms
            //EntityLife_DestroyEntity                                  195 ms    346 ms
            //EntityLife_GetEntities                                    27 ms     -1 ms
            //EntityLife_HasEntity                                      32 ms     3 ms
            //EntityLife_ReuseEntities                                  28 ms     -1 ms

            //Filter_Equals                                             179 ms    16 ms
            //Filter_Filtered                                           80 ms     8 ms
            //Filter_Filtered_ManyComponents                            82 ms     41 ms

            //Group_CreateGet                                           240 ms    320 ms
            //Group_CreateGetAfterEntities                              105 ms    411 ms
            //Group_CreateGetBeforeEntities                             643 ms    680 ms
            //Group_DestroyEntities                                     340 ms    -1 ms
            //Group_DestroyEntity                                       344 ms    410 ms
            //Group_GetEntities                                         19 ms     125 ms

            Console.WriteLine("");
            Console.WriteLine("ToCsv");
            Console.WriteLine(_toCsv);

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
            _toCsv += $"{testType.Name}\t{avgTime}\t{avgParallelTime}{Environment.NewLine}";
        }
    }
}