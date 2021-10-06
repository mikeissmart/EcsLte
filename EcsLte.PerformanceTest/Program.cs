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
                if (!testGrouping.Key.Contains("Misc"))
                {
                    foreach (var test in testGrouping)
                        Run(test);
                    Console.WriteLine("");
                }

            //Name                                                      Time      ParallelTime
            //EcsContext_ComponentLife_AddComponent                     312 ms    481 ms         1.19 GB
            //EcsContext_ComponentLife_AddComponentX2                   525 ms    500 ms         1.54 GB
            //EcsContext_ComponentLife_AddComponentX3                   537 ms    562 ms         1.01 GB
            //EcsContext_ComponentLife_AddSharedComponent               383 ms    428 ms         1.1 GB

            //EcsContext_ComponentLife_AddSharedComponentX2             1134 ms   596 ms         1.43 GB
            //EcsContext_ComponentLife_AddSharedComponentX3             1302 ms   652 ms         1.33 GB
            //----------
            //EcsContext_ComponentLife_AddSharedComponentX2             731 ms    577 ms         1.31 GB
            //EcsContext_ComponentLife_AddSharedComponentX3             865 ms    551 ms         1.55 GB


            //EcsContext_ComponentLife_RemoveAllComponents              147 ms    335 ms         1.15 GB
            //EcsContext_ComponentLife_RemoveComponent                  499 ms    558 ms         1.36 GB
            //EcsContext_ComponentLife_ReplaceComponent                 131 ms    14 ms          1.58 GB

            //EcsContext_EntityLife_CreateEntities                      77 ms     -1 ms          917.33 MB
            //EcsContext_EntityLife_CreateEntitiesBlueprint             154 ms    -1 ms          1.09 GB
            //EcsContext_EntityLife_CreateEntity                        322 ms    599 ms         1.95 GB
            //EcsContext_EntityLife_CreateEntityBlueprint               699 ms    712 ms         1.54 GB
            //EcsContext_EntityLife_CreateEntityReuse                   59 ms     419 ms         1.52 GB
            //EcsContext_EntityLife_DestroyEntities                     78 ms     -1 ms          1.04 GB
            //EcsContext_EntityLife_DestroyEntity                       104 ms    259 ms         1.19 GB

            //EcsContext_FilterAll_                                     179 ms    294 ms         526.78 MB
            //EcsContext_FilterAll_GroupWith                            669 ms    1135 ms        179.84 MB

            //EcsContext_FilterAll_GroupWithX10                         932 ms    1771 ms        179.7 MB
            //EcsContext_FilterAll_GroupWithX2                          686 ms    1161 ms        179.68 MB

            //EcsContext_FilterOne_                                     167 ms    295 ms         194.02 MB
            //EcsContext_FilterOne_GroupWith                            650 ms    1145 ms        180.56 MB
            //EcsContext_FilterOne_GroupWithX10                         1223 ms   1825 ms        179.82 MB
            //EcsContext_FilterOne_GroupWithX2                          679 ms    1247 ms        179.81 MB

            //EcsContext_GetComponent_GetAllComponents                  88 ms     39 ms          179.02 MB
            //EcsContext_GetComponent_GetComponent                      102 ms    36 ms          178.98 MB
            //EcsContext_GetComponent_GetUniqueComponent                168 ms    36 ms          179.5 MB
            //EcsContext_GetComponent_GetUniqueEntity                   46 ms     28 ms          179.5 MB
            //EcsContext_GetComponent_HasComponent                      59 ms     36 ms          179.5 MB
            //EcsContext_GetComponent_HasUniqueComponent                38 ms     27 ms          179.5 MB

            //EcsContext_GetEntity_GetEntities                          48 ms     -1 ms          552.15 MB
            //EcsContext_GetEntity_HasEntity                            34 ms     28 ms          1.21 GB

            //EcsContext_GetWatcher_WatchAdded                          225 ms    388 ms         527.7 MB
            //EcsContext_GetWatcher_WatchAddedOrRemoved                 248 ms    416 ms         193.55 MB
            //EcsContext_GetWatcher_WatchAddedOrUpdated                 252 ms    375 ms         196.81 MB
            //EcsContext_GetWatcher_WatchRemoved                        230 ms    355 ms         199.84 MB
            //EcsContext_GetWatcher_WatchUpdated                        218 ms    457 ms         193.99 MB

            //EcsContext_GroupWith                                      650 ms    1120 ms        181.33 MB
            //EcsContext_GroupWithX10                                   1233 ms   1817 ms        182.17 MB
            //EcsContext_GroupWithX2                                    709 ms    1187 ms        181.79 MB

            //EntityCommandQueue_ComponentLife_AddComponent             461 ms    730 ms         974.47 MB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      267 ms    477 ms         1.44 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          658 ms    906 ms         1.56 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         304 ms    519 ms         1.45 GB

            //EntityCommandQueue_EntityLife_CreateEntities              244 ms    -1 ms          1.1 GB
            //EntityCommandQueue_EntityLife_CreateEntitiesBlueprint     501 ms    -1 ms          1.15 GB
            //EntityCommandQueue_EntityLife_CreateEntity                500 ms    738 ms         1.26 GB
            //EntityCommandQueue_EntityLife_CreateEntityBlueprint       755 ms    920 ms         1.15 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             184 ms    -1 ms          1.11 GB
            //EntityCommandQueue_EntityLife_DestroyEntity               246 ms    397 ms         927.82 MB

            //EntityFilter_Create_AfterEntities                         199 ms    -1 ms          649.57 MB
            //EntityFilter_Create_BeforeEntities                        368 ms    599 ms         1.5 GB

            //EntityFilter_GetEntity_GetEntities                        49 ms     -1 ms          1.71 GB
            //EntityFilter_GetEntity_HasEntity                          37 ms     27 ms          2.43 GB

            //EntityFilter_GetWatcher_WatchAdded                        236 ms    390 ms         1.24 GB
            //EntityFilter_GetWatcher_WatchAddedOrRemoved               256 ms    400 ms         195.73 MB
            //EntityFilter_GetWatcher_WatchAddedOrUpdated               263 ms    414 ms         198.49 MB
            //EntityFilter_GetWatcher_WatchRemoved                      235 ms    436 ms         198.94 MB
            //EntityFilter_GetWatcher_WatchUpdated                      243 ms    352 ms         196.1 MB

            //EntityFilterGroup_Create_AfterEntities                    658 ms    -1 ms          751.7 MB
            //EntityFilterGroup_Create_BeforeEntities                   416 ms    598 ms         1.39 GB

            //EntityFilterGroup_GetEntity_GetEntities                   51 ms     -1 ms          1.55 GB
            //EntityFilterGroup_GetEntity_HasEntities                   37 ms     27 ms          2.43 GB

            //EntityGroup_Create_AfterEntities                          686 ms    -1 ms          2 GB
            //EntityGroup_Create_BeforeEntities                         446 ms    539 ms         1.29 GB

            //EntityGroup_GetEntity_GetEntities                         51 ms     -1 ms          1.49 GB
            //EntityGroup_GetEntity_HasEntity                           37 ms     27 ms          2.5 GB

            //EntityGroup_GetWatcher_WatchAdded                         225 ms    442 ms         1.2 GB
            //EntityGroup_GetWatcher_WatchAddedOrRemoved                260 ms    470 ms         196.1 MB
            //EntityGroup_GetWatcher_WatchAddedOrUpdated                251 ms    417 ms         196.1 MB
            //EntityGroup_GetWatcher_WatchRemoved                       234 ms    414 ms         195.95 MB
            //EntityGroup_GetWatcher_WatchUpdated                       227 ms    441 ms         196.43 MB

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
                var test = (BasePerformanceTest) Activator.CreateInstance(testType);
                test.PreRun();
                _stopwatch.Reset();
                _stopwatch.Start();
                test.Run();
                _stopwatch.Stop();
                memories[i] = GetMemoryUsage();
                test.PostRun();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                times[i] = _stopwatch.ElapsedMilliseconds;
                avgTime += _stopwatch.ElapsedMilliseconds;
                avgMemory += memories[i];

                test = (BasePerformanceTest) Activator.CreateInstance(testType);
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
            string[] Suffix = {"B", "KB", "MB", "GB", "TB"};
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
                dblSByte = bytes / 1024.0;

            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private static long GetMemoryUsage()
        {
            var process = Process.GetCurrentProcess();
            return process.WorkingSet64;
        }
    }
}