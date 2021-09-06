using System;
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
            //Run(typeof(EntityComponent_AddComponent));

            //Name                                                      Time      ParallelTime
            //Collector_CreateGet                                       175 ms    303 ms
            //Collector_CreateGetBeforeEntities                         797 ms    534 ms
            //Collector_GetEntities                                     38 ms     123 ms

            //CollectorTrigger_Create                                   1168 ms   1193 ms
            //CollectorTrigger_Equals                                   149 ms    15 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        930 ms    1056 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     366 ms    569 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    432 ms    586 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           513 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             792 ms    1150 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          643 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            489 ms    729 ms

            //EntityComponent_AddComponent                              556 ms    252 ms
            //EntityComponent_GetAllComponents                          171 ms    64 ms
            //EntityComponent_GetComponent                              70 ms     6 ms
            //EntityComponent_GetRandEntityComponent                    503 ms    133 ms
            //EntityComponent_HasComponent                              53 ms     5 ms
            //EntityComponent_RemoveAllComponents                       275 ms    223 ms
            //EntityComponent_RemoveComponent                           132 ms    12 ms
            //EntityComponent_ReplaceComponent                          310 ms    61 ms

            //EntityLife_CreateEntities                                 349 ms    -1 ms
            //EntityLife_CreateEntity                                   918 ms    978 ms
            //EntityLife_DestroyEntities                                292 ms    -1 ms
            //EntityLife_DestroyEntity                                  282 ms    674 ms
            //EntityLife_GetEntities                                    22 ms     -1 ms
            //EntityLife_HasEntity                                      32 ms     4 ms
            //EntityLife_ReuseEntities                                  75 ms     -1 ms

            //Filter_Create                                             1135 ms   1168 ms
            //Filter_Equals                                             144 ms    15 ms
            //Filter_Filtered                                           62 ms     6 ms
            //Filter_Filtered_ManyComponents                            705 ms    69 ms

            //Group_CreateGet                                           220 ms    361 ms
            //Group_CreateGetAfterEntities                              60 ms     428 ms
            //Group_CreateGetBeforeEntities                             850 ms    761 ms
            //Group_GetEntities                                         42 ms     129 ms

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