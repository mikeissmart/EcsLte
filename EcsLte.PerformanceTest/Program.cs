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
            //EcsContext_ComponentLife_AddComponent                     331 ms    495 ms         1.07 GB
            //EcsContext_ComponentLife_AddComponentX2                   522 ms    545 ms         2.51 GB????????????????????????
            //EcsContext_ComponentLife_AddComponentX3                   541 ms    528 ms         2.23 GB
            //EcsContext_ComponentLife_RemoveAllComponents              123 ms    453 ms         1.23 GB
            //EcsContext_ComponentLife_RemoveComponent                  486 ms    459 ms         1.43 GB
            //EcsContext_ComponentLife_ReplaceComponent                 158 ms    14 ms          1.5 GB

            //EcsContext_EntityGroup_SharedComponent                    355 ms    282 ms         1.29 GB????????????????????????
            //EcsContext_EntityGroup_SharedComponentX10                 1034 ms   413 ms         172.05 MB
            //EcsContext_EntityGroup_SharedComponentX2                  395 ms    298 ms         172.46 MB

            //EcsContext_EntityLife_CreateEntities                      53 ms     -1 ms          597.3 MB
            //EcsContext_EntityLife_CreateEntitiesBlueprint             194 ms    -1 ms          433.99 MB
            //EcsContext_EntityLife_CreateEntity                        383 ms    686 ms         1.69 GB
            //EcsContext_EntityLife_CreateEntityBlueprint               723 ms    847 ms         1.89 GB
            //EcsContext_EntityLife_CreateEntityReuse                   57 ms     383 ms         1.88 GB
            //EcsContext_EntityLife_DestroyEntities                     80 ms     -1 ms          1.09 GB
            //EcsContext_EntityLife_DestroyEntity                       89 ms     297 ms         1.78 GB

            //EcsContext_FilterByAll                                    52 ms     232 ms         1.39 GB????????????????????????
            //EcsContext_FilterByOne                                    54 ms     217 ms         174.53 MB

            //EcsContext_GetComponent_GetAllComponents                  89 ms     39 ms          173.83 MB
            //EcsContext_GetComponent_GetComponent                      96 ms     36 ms          174.13 MB
            //EcsContext_GetComponent_GetUniqueComponent                169 ms    36 ms          174.16 MB
            //EcsContext_GetComponent_GetUniqueEntity                   45 ms     27 ms          174.16 MB
            //EcsContext_GetComponent_HasComponent                      62 ms     36 ms          174.16 MB
            //EcsContext_GetComponent_HasUniqueComponent                39 ms     27 ms          174.16 MB

            //EcsContext_GetEntity_GetEntities                          47 ms     -1 ms          557.47 MB
            //EcsContext_GetEntity_HasEntity                            36 ms     27 ms          1.89 GB????????????????????????

            //EcsContext_GetWatcher_Added                               99 ms     308 ms         1.38 GB????????????????????????
            //EcsContext_GetWatcher_AddedOrRemoved                      121 ms    341 ms         174.33 MB
            //EcsContext_GetWatcher_AddedOrUpdated                      122 ms    445 ms         174.15 MB
            //EcsContext_GetWatcher_Removed                             92 ms     423 ms         174.16 MB
            //EcsContext_GetWatcher_Updated                             92 ms     365 ms         174.14 MB

            //EntityCommandQueue_ComponentLife_AddComponent             515 ms    736 ms         956.52 MB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      244 ms    463 ms         1.45 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          674 ms    884 ms         1.53 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         321 ms    527 ms         1.44 GB

            //EntityCommandQueue_EntityLife_CreateEntities              213 ms    -1 ms          834.95 MB
            //EntityCommandQueue_EntityLife_CreateEntitiesBlueprint     434 ms    -1 ms          873 MB
            //EntityCommandQueue_EntityLife_CreateEntity                485 ms    650 ms         1.24 GB
            //EntityCommandQueue_EntityLife_CreateEntityBlueprint       732 ms    944 ms         1.15 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             226 ms    -1 ms          1.45 GB
            //EntityCommandQueue_EntityLife_DestroyEntity               215 ms    415 ms         1.21 GB

            //EntityFilter_EntityGroup_SharedComponent                  346 ms    285 ms         1.38 GB????????????????????????
            //EntityFilter_EntityGroup_SharedComponentX10               1038 ms   387 ms         180.46 MB
            //EntityFilter_EntityGroup_SharedComponentX2                410 ms    301 ms         180.3 MB

            //EntityFilter_GetEntity_GetEntitiesAfter                   394 ms    557 ms         1.19 GB
            //EntityFilter_GetEntity_GetEntitiesBefore                  500 ms    -1 ms          2.56 GB
            //EntityFilter_GetEntity_HasEntity                          62 ms     35 ms          1.17 GB

            //EntityFilter_GetWatcher_Added                             98 ms     363 ms         1.32 GB????????????????????????
            //EntityFilter_GetWatcher_AddedOrRemoved                    124 ms    356 ms         179.81 MB
            //EntityFilter_GetWatcher_AddedOrUpdated                    124 ms    314 ms         179.79 MB
            //EntityFilter_GetWatcher_Removed                           102 ms    430 ms         179.76 MB
            //EntityFilter_GetWatcher_Updated                           98 ms     410 ms         179.73 MB

            //EntityGroup_GetEntity_GetEntitiesAfter                    777 ms    797 ms         1.21 GB
            //EntityGroup_GetEntity_GetEntitiesBefore                   351 ms    -1 ms          2.56 GB????????????????????????
            //EntityGroup_GetEntity_HasEntity                           62 ms     36 ms          1.13 GB

            //EntityGroup_GetWatcher_Added                              97 ms     397 ms         1.48 GB????????????????????????
            //EntityGroup_GetWatcher_AddedOrRemoved                     126 ms    328 ms         175.85 MB
            //EntityGroup_GetWatcher_AddedOrUpdated                     124 ms    411 ms         175.86 MB
            //EntityGroup_GetWatcher_Removed                            94 ms     349 ms         175.87 MB
            //EntityGroup_GetWatcher_Updated                            97 ms     346 ms         175.87 MB


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

        private static string GetMemoryUsageReadable(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
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