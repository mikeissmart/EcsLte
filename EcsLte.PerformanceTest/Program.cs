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
            Console.WriteLine("//Name".PadRight(60) + "Time".PadRight(10) + "ParallelTime".PadRight(15) + "Memory");
            _stopwatch = new Stopwatch();

            var baseTestType = typeof(BasePerformanceTest);
            var tests = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    baseTestType.IsAssignableFrom(x) &&
                    x != baseTestType)
                .OrderBy(x => x.Name)
                .GroupBy(x => x.Name.Split('_').Length == 3
                    ? $"{x.Name.Split('_')[0]}_{x.Name.Split('_')[1]}"
                    : x.Name.Split('_').Length == 2
                        ? x.Name.Split('_')[0]
                        : x.Name)
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
            //EcsContext_ComponentLife_AddComponent                     532 ms    300 ms         1002.77 MB
            //EcsContext_ComponentLife_RemoveAllComponents              284 ms    80 ms          1.5 GB
            //EcsContext_ComponentLife_RemoveComponent                  177 ms    33 ms          1.63 GB
            //EcsContext_ComponentLife_ReplaceComponent                 176 ms    57 ms          1.49 GB

            //EcsContext_EntityLife_CreateEntities                      148 ms    -1 ms          1.26 GB
            //EcsContext_EntityLife_CreateEntity                        447 ms    788 ms         1.6 GB
            //EcsContext_EntityLife_CreateEntityReuse                   55 ms     330 ms         1.31 GB
            //EcsContext_EntityLife_DestroyEntities                     484 ms    -1 ms          772.99 MB
            //EcsContext_EntityLife_DestroyEntity                       527 ms    406 ms         1.06 GB

            //EcsContext_FilterByAll                                    912 ms    1035 ms        1.61 GB
            //EcsContext_FilterByOne                                    213 ms    376 ms         1.61 GB

            //EcsContext_GetComponent_GetAllComponents                  102 ms    49 ms          1.61 GB
            //EcsContext_GetComponent_GetComponent                      99 ms     36 ms          1.61 GB
            //EcsContext_GetComponent_GetUniqueComponent                150 ms    36 ms          1.61 GB
            //EcsContext_GetComponent_GetUniqueEntity                   43 ms     27 ms          1.61 GB
            //EcsContext_GetComponent_HasComponent                      65 ms     34 ms          1.61 GB
            //EcsContext_GetComponent_HasUniqueComponent                37 ms     27 ms          1.61 GB

            //EcsContext_GetEntity_GetEntities                          47 ms     -1 ms          756.11 MB
            //EcsContext_GetEntity_HasEntity                            38 ms     27 ms          1.17 GB

            //EntityCommandQueue_ComponentLife_AddComponent             694 ms    854 ms         1.43 GB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      519 ms    753 ms         1.03 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          342 ms    631 ms         1.53 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         347 ms    625 ms         1.42 GB

            //EntityCommandQueue_EntityLife_CreateEntities              295 ms    -1 ms          934.52 MB
            //EntityCommandQueue_EntityLife_CreateEntity                682 ms    810 ms         1.22 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             664 ms    -1 ms          806.25 MB
            //EntityCommandQueue_EntityLife_DestroyEntity               701 ms    891 ms         1.36 GB

            //EntityFilter_GetEntity_GetEntitiesAfter                   1127 ms   937 ms         1.45 GB
            //EntityFilter_GetEntity_GetEntitiesBefore                  60 ms     -1 ms          1.15 GB
            //EntityFilter_GetEntity_HasEntity                          38 ms     29 ms          1.16 GB

            //EntityFilter_GetWatcher_Added                             223 ms    391 ms         1.64 GB
            //EntityFilter_GetWatcher_AddedOrRemoved                    254 ms    359 ms         1.64 GB
            //EntityFilter_GetWatcher_AddedOrUpdated                    252 ms    374 ms         1.64 GB
            //EntityFilter_GetWatcher_Removed                           237 ms    361 ms         1.64 GB
            //EntityFilter_GetWatcher_Updated                           225 ms    325 ms         1.64 GB

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
            var memories = new long[loops];
            var paralleltimes = new long[loops];
            long avgTime = 0;
            long avgMemory = 0;
            long avgParallelTime = 0;
            for (var i = 0; i < loops; i++)
            {
                var test = (BasePerformanceTest)Activator.CreateInstance(testType);
                test.PreRun();
                _stopwatch.Reset();
                _stopwatch.Start();
                test.Run();
                _stopwatch.Stop();
                memories[i] = GetMemoryUsage();
                test.PostRun();

                times[i] = _stopwatch.ElapsedMilliseconds;
                avgTime += _stopwatch.ElapsedMilliseconds;
                avgMemory += memories[i];

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
            avgMemory /= loops;
            avgParallelTime = avgParallelTime > 0
                ? avgParallelTime /= loops
                : -1;
            Console.WriteLine(
                //"//" + new String('-', 57) + " " +
                $"//{testType.Name}".PadRight(60) +
                $"{avgTime} ms".PadRight(10) +
                $"{avgParallelTime} ms".PadRight(15) +
                $"{GetMemoryUsageReadable(avgMemory)}");
            _toCsv += $"{testType.Name}\t{avgTime}\t{avgParallelTime}{Environment.NewLine}";
        }

        private static string GetMemoryUsageReadable(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private static long GetMemoryUsage()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            return process.WorkingSet64;
        }
    }
}