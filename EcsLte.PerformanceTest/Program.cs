using System;
using System.Diagnostics;
using System.Linq;

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

            /*foreach (var testGrouping in tests)
                if (testGrouping.Key != "Misc")
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }*/
            Run(typeof(EntityComponent_RemoveAllComponents));

            //Name                                                      Time      ParallelTime
            //Collector_CreateGet                                       198 ms    325 ms
            //Collector_CreateGetBeforeEntities                         563 ms    1001 ms
            //Collector_GetEntities                                     3 ms      96 ms

            //CollectorTrigger_Create                                   955 ms    1164 ms
            //CollectorTrigger_Equals                                   168 ms    15 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        302 ms    564 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     222 ms    512 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    314 ms    509 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           152 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             225 ms    460 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          193 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            721 ms    929 ms

            //EntityComponent_AddComponent                              145 ms    48 ms
            //EntityComponent_GetAllComponents                          186 ms    66 ms
            //EntityComponent_GetComponent                              62 ms     6 ms
            //EntityComponent_GetRandEntityComponent                    416 ms    133 ms
            //EntityComponent_RemoveAllComponents                       474 ms    100 ms
            //EntityComponent_RemoveComponent                           95 ms     8 ms
            //EntityComponent_ReplaceComponent                          153 ms    66 ms

            //EntityLife_CreateEntities                                 18 ms     -1 ms
            //EntityLife_CreateEntity                                   108 ms    512 ms
            //EntityLife_DestroyEntities                                510 ms    -1 ms
            //EntityLife_DestroyEntity                                  537 ms    571 ms
            //EntityLife_GetEntities                                    22 ms     -1 ms
            //EntityLife_HasEntity                                      32 ms     3 ms
            //EntityLife_ReuseEntities                                  18 ms     -1 ms

            //Filter_Create                                             923 ms    1195 ms
            //Filter_Equals                                             162 ms    15 ms
            //Filter_Filtered                                           58 ms     6 ms

            //Group_CreateGet                                           193 ms    320 ms
            //Group_CreateGetAfterEntities                              200 ms    334 ms
            //Group_CreateGetBeforeEntities                             430 ms    558 ms
            //Group_GetEntities                                         2 ms      88 ms


            /*Console.WriteLine("");
            Console.WriteLine("ToCsv");
            Console.WriteLine(_toCsv);*/

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