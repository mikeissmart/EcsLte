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
            //Collector_CreateGet                                       1636 ms   1927 ms
            //Collector_CreateGetBeforeEntities                         806 ms    923 ms
            //Collector_DestroyEntities                                 658 ms    -1 ms
            //Collector_DestroyEntity                                   664 ms    857 ms
            //Collector_GetEntities                                     21 ms     121 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        530 ms    765 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     332 ms    565 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    373 ms    556 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           284 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             604 ms    859 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          325 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            370 ms    526 ms

            //EntityComponent_AddComponent                              405 ms    208 ms
            //EntityComponent_GetAllComponents                          147 ms    118 ms
            //EntityComponent_GetComponent                              116 ms    58 ms
            //EntityComponent_GetRandEntityComponent                    320 ms    108 ms
            //EntityComponent_HasComponent                              53 ms     5 ms
            //EntityComponent_RemoveAllComponents                       241 ms    117 ms
            //EntityComponent_RemoveComponent                           199 ms    62 ms
            //EntityComponent_ReplaceComponent                          147 ms    91 ms

            //EntityLife_CreateEntities                                 161 ms    -1 ms
            //EntityLife_CreateEntity                                   432 ms    764 ms
            //EntityLife_DestroyEntities                                169 ms    -1 ms
            //EntityLife_DestroyEntity                                  179 ms    486 ms
            //EntityLife_GetEntities                                    21 ms     -1 ms
            //EntityLife_HasEntity                                      32 ms     3 ms
            //EntityLife_ReuseEntities                                  28 ms     -1 ms

            //Filter_Equals                                             156 ms    15 ms
            //Filter_Filtered                                           85 ms     7 ms
            //Filter_Filtered_ManyComponents                            85 ms     35 ms

            //Group_CreateGet                                           239 ms    384 ms
            //Group_CreateGetAfterEntities                              115 ms    497 ms
            //Group_CreateGetBeforeEntities                             747 ms    724 ms
            //Group_DestroyEntities                                     325 ms    -1 ms
            //Group_DestroyEntity                                       348 ms    514 ms
            //Group_GetEntities                                         21 ms     129 ms

            //PrimaryKey_CreateGetAfterEntities                         212 ms    453 ms
            //PrimaryKey_CreateGetBeforeEntities                        369 ms    201 ms

            //SharedKey_CreateGetAfterEntities                          155 ms    335 ms
            //SharedKey_CreateGetBeforeEntities                         386 ms    220 ms
            //SharedKey_GetEntities                                     1 ms      194 ms

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