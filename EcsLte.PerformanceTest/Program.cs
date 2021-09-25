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
                .GroupBy(x => x.Name.Split('_').Length >= 3
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
            //EcsContext_ComponentLife_AddComponent                     472 ms    352 ms         1.17 GB
            //EcsContext_ComponentLife_RemoveAllComponents              294 ms    86 ms          1.34 GB
            //EcsContext_ComponentLife_RemoveComponent                  180 ms    29 ms          1.6 GB
            //EcsContext_ComponentLife_ReplaceComponent                 171 ms    38 ms          1.35 GB

            //EcsContext_EntityLife_CreateEntities                      167 ms    -1 ms          1.57 GB
            //EcsContext_EntityLife_CreateEntity                        535 ms    777 ms         1.37 GB
            //EcsContext_EntityLife_CreateEntityReuse                   57 ms     335 ms         1.26 GB
            //EcsContext_EntityLife_DestroyEntities                     1166 ms   -1 ms          758.45 MB
            //EcsContext_EntityLife_DestroyEntity                       1181 ms   526 ms         1.35 GB

            //EcsContext_FilterByAll                                    930 ms    1063 ms        1.5 GB
            //EcsContext_FilterByOne                                    213 ms    377 ms         1.5 GB

            //EcsContext_GetComponent_GetAllComponents                  99 ms     48 ms          1.5 GB
            //EcsContext_GetComponent_GetComponent                      98 ms     37 ms          1.5 GB
            //EcsContext_GetComponent_GetUniqueComponent                146 ms    36 ms          1.5 GB
            //EcsContext_GetComponent_GetUniqueEntity                   41 ms     27 ms          1.5 GB
            //EcsContext_GetComponent_HasComponent                      60 ms     36 ms          1.5 GB
            //EcsContext_GetComponent_HasUniqueComponent                36 ms     27 ms          1.5 GB

            //EcsContext_GetEntity_GetEntities                          47 ms     -1 ms          1.03 GB
            //EcsContext_GetEntity_HasEntity                            37 ms     28 ms          1.5 GB

            //EcsContext_WithKey_PrimaryKey                             214 ms    446 ms         1.58 GB
            //EcsContext_WithKey_SharedKey                              201 ms    428 ms         1.57 GB
            //EcsContext_WithKey_SharedKeyesX10                         1362 ms   769 ms         33.93 MB
            //EcsContext_WithKey_SharedKeyesX2                          569 ms    508 ms         35.4 MB

            //EntityCommandQueue_ComponentLife_AddComponent             731 ms    950 ms         1.22 GB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      473 ms    758 ms         1.28 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          326 ms    674 ms         1.32 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         365 ms    656 ms         1.69 GB

            //EntityCommandQueue_EntityLife_CreateEntities              283 ms    -1 ms          1.6 GB
            //EntityCommandQueue_EntityLife_CreateEntity                694 ms    1003 ms        1.4 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             1370 ms   -1 ms          787.84 MB
            //EntityCommandQueue_EntityLife_DestroyEntity               1361 ms   1571 ms        1.18 GB

            //EntityFilter_GetEntity_GetEntitiesAfter                   1194 ms   942 ms         1.47 GB
            //EntityFilter_GetEntity_GetEntitiesBefore                  46 ms     -1 ms          3.06 GB
            //EntityFilter_GetEntity_HasEntity                          39 ms     28 ms          1.17 GB

            //EntityFilter_GetWatcher_Added                             239 ms    369 ms         1.55 GB
            //EntityFilter_GetWatcher_AddedOrRemoved                    279 ms    400 ms         1.55 GB
            //EntityFilter_GetWatcher_AddedOrUpdated                    268 ms    385 ms         1.55 GB
            //EntityFilter_GetWatcher_Removed                           256 ms    357 ms         1.55 GB
            //EntityFilter_GetWatcher_Updated                           238 ms    386 ms         1.55 GB

            //EntityKey_PrimaryKey                                      230 ms    433 ms         67.27 MB
            //EntityKey_SharedKey                                       234 ms    441 ms         1.33 GB
            //EntityKey_SharedKeyesX10                                  1307 ms   764 ms         1.97 GB
            //EntityKey_SharedKeyesX2                                   630 ms    700 ms         1.45 GB

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