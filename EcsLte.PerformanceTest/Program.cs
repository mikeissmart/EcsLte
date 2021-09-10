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
            //Collector_CreateGet                                       211 ms    363 ms
            //Collector_CreateGetBeforeEntities                         781 ms    800 ms
            //Collector_DestroyEntities                                 661 ms    -1 ms
            //Collector_DestroyEntity                                   656 ms    954 ms
            //Collector_GetEntities                                     20 ms     151 ms

            //CollectorTrigger_Create                                   1116 ms   1190 ms
            //CollectorTrigger_Equals                                   187 ms    16 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        541 ms    785 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     309 ms    506 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    326 ms    527 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           292 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             535 ms    775 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          360 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            375 ms    543 ms

            //EntityComponent_AddComponent                              356 ms    202 ms
            //EntityComponent_GetAllComponents                          159 ms    52 ms
            //EntityComponent_GetComponent                              120 ms    62 ms
            //EntityComponent_GetRandEntityComponent                    316 ms    123 ms
            //EntityComponent_HasComponent                              56 ms     5 ms
            //EntityComponent_RemoveAllComponents                       289 ms    60 ms
            //EntityComponent_RemoveComponent                           133 ms    10 ms
            //EntityComponent_ReplaceComponent                          154 ms    61 ms

            //EntityLife_CreateEntities                                 155 ms    -1 ms
            //EntityLife_CreateEntity                                   408 ms    761 ms
            //EntityLife_DestroyEntities                                185 ms    -1 ms
            //EntityLife_DestroyEntity                                  182 ms    346 ms
            //EntityLife_GetEntities                                    22 ms     -1 ms
            //EntityLife_HasEntity                                      32 ms     4 ms
            //EntityLife_ReuseEntities                                  25 ms     -1 ms

            //Filter_Equals                                             202 ms    17 ms
            //Filter_Filtered                                           87 ms     8 ms
            //Filter_Filtered_ManyComponents                            85 ms     35 ms

            //Group_CreateGet                                           257 ms    403 ms
            //Group_CreateGetAfterEntities                              118 ms    489 ms
            //Group_CreateGetBeforeEntities                             691 ms    662 ms
            //Group_DestroyEntities                                     355 ms    -1 ms
            //Group_DestroyEntity                                       336 ms    450 ms
            //Group_GetEntities                                         20 ms     121 ms


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