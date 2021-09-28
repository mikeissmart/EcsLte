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

            ObjectCache.IsCacheEnabled = true;

            foreach (var testGrouping in tests)
                if (testGrouping.Key != "Misc")
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }

            //Run(typeof(EcsContext_ComponentLife_AddComponent));

            //Name                                                      Time      ParallelTime
            //EcsContext_ComponentLife_AddComponent                     333 ms    492 ms         1.07 GB
            //EcsContext_ComponentLife_RemoveAllComponents              124 ms    341 ms         1.81 GB
            //EcsContext_ComponentLife_RemoveComponent                  406 ms    449 ms         2.34 GB
            //EcsContext_ComponentLife_ReplaceComponent                 152 ms    15 ms          2.5 GB

            //EcsContext_EntityGroup_PrimaryComponent                   343 ms    333 ms         1.26 GB
            //EcsContext_EntityGroup_SharedComponent                    342 ms    418 ms         1.26 GB
            //EcsContext_EntityGroup_SharedComponentX10                 1001 ms   486 ms         1.26 GB
            //EcsContext_EntityGroup_SharedComponentX2                  423 ms    382 ms         1.04 GB

            //EcsContext_EntityLife_CreateEntities                      48 ms     -1 ms          517.12 MB
            //EcsContext_EntityLife_CreateEntity                        412 ms    714 ms         1.51 GB
            //EcsContext_EntityLife_CreateEntityReuse                   57 ms     393 ms         1.35 GB
            //EcsContext_EntityLife_DestroyEntities                     81 ms     -1 ms          489.64 MB
            //EcsContext_EntityLife_DestroyEntity                       85 ms     378 ms         2.26 GB

            //EcsContext_FilterByAll                                    932 ms    1132 ms        1.33 GB
            //EcsContext_FilterByOne                                    198 ms    331 ms         1.32 GB

            //EcsContext_GetComponent_GetAllComponents                  94 ms     39 ms          1.32 GB
            //EcsContext_GetComponent_GetComponent                      100 ms    36 ms          1.32 GB
            //EcsContext_GetComponent_GetUniqueComponent                148 ms    36 ms          1.32 GB
            //EcsContext_GetComponent_GetUniqueEntity                   41 ms     27 ms          1.32 GB
            //EcsContext_GetComponent_HasComponent                      61 ms     37 ms          1.32 GB
            //EcsContext_GetComponent_HasUniqueComponent                37 ms     29 ms          1.32 GB

            //EcsContext_GetEntity_GetEntities                          48 ms     -1 ms          359.79 MB
            //EcsContext_GetEntity_HasEntity                            37 ms     29 ms          1.35 GB

            //EntityCommandQueue_ComponentLife_AddComponent             592 ms    756 ms         1 GB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      244 ms    524 ms         1.51 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          588 ms    829 ms         1.55 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         324 ms    546 ms         1.38 GB

            //EntityCommandQueue_EntityLife_CreateEntities              176 ms    -1 ms          1.36 GB
            //EntityCommandQueue_EntityLife_CreateEntity                509 ms    669 ms         1.13 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             228 ms    -1 ms          450.91 MB
            //EntityCommandQueue_EntityLife_DestroyEntity               232 ms    453 ms         1.21 GB

            //EntityFilter_GetEntity_GetEntitiesAfter                   422 ms    682 ms         1.58 GB
            //EntityFilter_GetEntity_GetEntitiesBefore                  678 ms    -1 ms          2.6 GB
            //EntityFilter_GetEntity_HasEntity                          62 ms     37 ms          1.38 GB

            //EntityFilter_GetWatcher_Added                             275 ms    348 ms         2.57 GB
            //EntityFilter_GetWatcher_AddedOrRemoved                    290 ms    374 ms         2.57 GB
            //EntityFilter_GetWatcher_AddedOrUpdated                    295 ms    370 ms         2.57 GB
            //EntityFilter_GetWatcher_Removed                           267 ms    350 ms         2.57 GB
            //EntityFilter_GetWatcher_Updated                           265 ms    404 ms         2.57 GB

            //EntityGroup_GetEntity_GetEntitiesAfter                    1445 ms   813 ms         1.16 GB
            //EntityGroup_GetEntity_GetEntitiesBefore                   344 ms    -1 ms          1020.4 MB
            //EntityGroup_GetEntity_HasEntity                           63 ms     37 ms          1.45 GB

            //EntityGroup_GetWatcher_Added                              265 ms    369 ms         1.93 GB
            //EntityGroup_GetWatcher_AddedOrRemoved                     299 ms    359 ms         1.93 GB
            //EntityGroup_GetWatcher_AddedOrUpdated                     297 ms    372 ms         1.93 GB
            //EntityGroup_GetWatcher_Removed                            266 ms    364 ms         1.93 GB
            //EntityGroup_GetWatcher_Updated                            277 ms    389 ms         1.93 GB

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
            _toCsv += $"{testType.Name}\t{avgTime}\t{avgParallelTime}\t{avgMemory}{Environment.NewLine}";
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