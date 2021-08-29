using System;
using System.Diagnostics;
using System.Linq;

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
                if (testGrouping.Key != "Misc")
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }
            }

            //Name                                                      Time      ParallelTime
            //Collector_CreateGet                                       50 ms     132 ms
            //Collector_CreateGetBeforeEntities                         977 ms    867 ms

            //CollectorTrigger_Equals                                   15 ms     2 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        319 ms    524 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     207 ms    460 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    287 ms    520 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           141 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             201 ms    540 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          152 ms    -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            242 ms    517 ms

            //EntityComponent_AddComponent                              140 ms    45 ms
            //EntityComponent_GetAllComponents                          161 ms    39 ms
            //EntityComponent_GetComponent                              66 ms     6 ms
            //EntityComponent_RemoveAllComponents                       90 ms     8 ms
            //EntityComponent_RemoveComponent                           94 ms     8 ms
            //EntityComponent_ReplaceComponent                          161 ms    54 ms

            //EntityLife_CreateEntities                                 27 ms     -1 ms
            //EntityLife_CreateEntity                                   79 ms     365 ms
            //EntityLife_DestroyEntities                                120 ms    -1 ms
            //EntityLife_DestroyEntity                                  132 ms    379 ms
            //EntityLife_GetEntities                                    20 ms     -1 ms
            //EntityLife_HasEntity                                      32 ms     3 ms
            //EntityLife_ReuseEntities                                  19 ms     -1 ms

            //Filter_Equals                                             15 ms     2 ms
            //Filter_Filtered                                           329 ms    65 ms

            //Group_CreateGet                                           58 ms     239 ms
            //Group_CreateGetAfterEntities                              500 ms    697 ms
            //Group_CreateGetBeforeEntities                             788 ms    719 ms


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