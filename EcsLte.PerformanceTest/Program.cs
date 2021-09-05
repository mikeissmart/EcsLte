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

            foreach (var testGrouping in tests)
                if (testGrouping.Key != "Misc")
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }

            //Name                                                      Time      ParallelTime
            //Collector_CreateGet                                       151 ms    220 ms
            //Collector_CreateGetBeforeEntities                         862 ms    1050 ms
            //Collector_GetEntities                                     2 ms      96 ms

            //CollectorTrigger_Create                                   950 ms    1108 ms
            //CollectorTrigger_Equals                                   129 ms    13 ms

            //EntityCommandPlayback_EntityComponent_AddComponent        706 ms    954 ms
            //EntityCommandPlayback_EntityComponent_RemoveComponent     313 ms    465 ms
            //EntityCommandPlayback_EntityComponent_ReplaceComponent    456 ms    496 ms
            //EntityCommandPlayback_EntityLife_CreateEntities           452 ms    -1 ms
            //EntityCommandPlayback_EntityLife_CreateEntity             873 ms    1080 ms
            //EntityCommandPlayback_EntityLife_DestroyEntities          1098 ms   -1 ms
            //EntityCommandPlayback_EntityLife_DestroyEntity            398 ms    670 ms

            //EntityComponent_AddComponent                              573 ms    403 ms
            //EntityComponent_GetAllComponents                          235 ms    36 ms
            //EntityComponent_GetComponent                              58 ms     6 ms
            //EntityComponent_GetRandEntityComponent                    428 ms    130 ms
            //EntityComponent_HasComponent                              47 ms     4 ms
            //EntityComponent_RemoveAllComponents                       419 ms    95 ms
            //EntityComponent_RemoveComponent                           109 ms    9 ms
            //EntityComponent_ReplaceComponent                          216 ms    63 ms

            //EntityLife_CreateEntities                                 193 ms    -1 ms
            //EntityLife_CreateEntity                                   756 ms    1112 ms
            //EntityLife_DestroyEntities                                294 ms    -1 ms
            //EntityLife_DestroyEntity                                  245 ms    478 ms
            //EntityLife_GetEntities                                    20 ms     -1 ms
            //EntityLife_HasEntity                                      28 ms     16 ms
            //EntityLife_ReuseEntities                                  201 ms    -1 ms

            //Filter_Create                                             894 ms    1109 ms
            //Filter_Equals                                             131 ms    13 ms
            //Filter_Filtered                                           57 ms     6 ms
            //Filter_Filtered_ManyComponents                            629 ms    64 ms

            //Group_CreateGet                                           168 ms    231 ms
            //Group_CreateGetAfterEntities                              184 ms    262 ms
            //Group_CreateGetBeforeEntities                             572 ms    854 ms
            //Group_GetEntities                                         3 ms      89 ms

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