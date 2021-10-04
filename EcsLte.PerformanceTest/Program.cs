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
            //EcsContext_ComponentLife_AddComponent                     346 ms    436 ms         1.19 GB
            //EcsContext_ComponentLife_AddComponentX2                   531 ms    554 ms         1.54 GB
            //EcsContext_ComponentLife_AddComponentX3                   534 ms    566 ms         1.15 GB
            //EcsContext_ComponentLife_RemoveAllComponents              124 ms    295 ms         1.27 GB
            //EcsContext_ComponentLife_RemoveComponent                  506 ms    534 ms         1.48 GB
            //EcsContext_ComponentLife_ReplaceComponent                 160 ms    16 ms          1.57 GB

            //EcsContext_EntityLife_CreateEntities                      52 ms     -1 ms          1.03 GB
            //EcsContext_EntityLife_CreateEntitiesBlueprint             162 ms    -1 ms          652.14 MB
            //EcsContext_EntityLife_CreateEntity                        385 ms    650 ms         1.82 GB
            //EcsContext_EntityLife_CreateEntityBlueprint               693 ms    793 ms         1.97 GB
            //EcsContext_EntityLife_CreateEntityReuse                   58 ms     332 ms         1.08 GB
            //EcsContext_EntityLife_DestroyEntities                     81 ms     -1 ms          753.12 MB
            //EcsContext_EntityLife_DestroyEntity                       107 ms    340 ms         1.15 GB

            //EcsContext_FilterAll_                                     180 ms    320 ms         545.5 MB
            //EcsContext_FilterAll_GroupWith                            770 ms    1002 ms        181.49 MB
            //EcsContext_FilterAll_GroupWithX10                         1279 ms   1647 ms        181.4 MB
            //EcsContext_FilterAll_GroupWithX2                          811 ms    1036 ms        181.73 MB

            //EcsContext_FilterOne_                                     159 ms    320 ms         194.79 MB
            //EcsContext_FilterOne_GroupWith                            803 ms    1016 ms        181.85 MB
            //EcsContext_FilterOne_GroupWithX10                         1337 ms   1609 ms        181.94 MB
            //EcsContext_FilterOne_GroupWithX2                          711 ms    1066 ms        182.04 MB

            //EcsContext_GetComponent_GetAllComponents                  95 ms     38 ms          181.39 MB
            //EcsContext_GetComponent_GetComponent                      105 ms    36 ms          181.29 MB
            //EcsContext_GetComponent_GetUniqueComponent                162 ms    35 ms          181.3 MB
            //EcsContext_GetComponent_GetUniqueEntity                   45 ms     26 ms          181.32 MB
            //EcsContext_GetComponent_HasComponent                      64 ms     34 ms          181.32 MB
            //EcsContext_GetComponent_HasUniqueComponent                38 ms     27 ms          181.63 MB

            //EcsContext_GetEntity_GetEntities                          52 ms     -1 ms          554.43 MB
            //EcsContext_GetEntity_HasEntity                            35 ms     27 ms          1.43 GB

            //EcsContext_GetWatcher_WatchAdded                          248 ms    421 ms         532.2 MB
            //EcsContext_GetWatcher_WatchAddedOrRemoved                 262 ms    430 ms         195.37 MB
            //EcsContext_GetWatcher_WatchAddedOrUpdated                 274 ms    441 ms         199.89 MB
            //EcsContext_GetWatcher_WatchRemoved                        232 ms    412 ms         196 MB
            //EcsContext_GetWatcher_WatchUpdated                        226 ms    407 ms         195.39 MB

            //EcsContext_GroupWith                                      596 ms    1005 ms        182.61 MB
            //EcsContext_GroupWithX10                                   1239 ms   1658 ms        183.41 MB
            //EcsContext_GroupWithX2                                    827 ms    1059 ms        183.41 MB

            //EntityCommandQueue_ComponentLife_AddComponent             492 ms    711 ms         975.91 MB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      231 ms    481 ms         1.43 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          672 ms    909 ms         1.56 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         326 ms    564 ms         1.38 GB

            //EntityCommandQueue_EntityLife_CreateEntities              226 ms    -1 ms          1.15 GB
            //EntityCommandQueue_EntityLife_CreateEntitiesBlueprint     440 ms    -1 ms          1.07 GB
            //EntityCommandQueue_EntityLife_CreateEntity                515 ms    661 ms         1.17 GB
            //EntityCommandQueue_EntityLife_CreateEntityBlueprint       742 ms    977 ms         1.1 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             194 ms    -1 ms          1.05 GB
            //EntityCommandQueue_EntityLife_DestroyEntity               248 ms    418 ms         931.65 MB

            //EntityFilter_GetEntity_GetEntitiesAfter                   392 ms    626 ms         1.04 GB
            //EntityFilter_GetEntity_GetEntitiesBefore                  181 ms    -1 ms          997.32 MB
            //EntityFilter_GetEntity_HasEntity                          38 ms     27 ms          2.21 GB

            //EntityFilter_GetWatcher_WatchAdded                        227 ms    399 ms         1.2 GB
            //EntityFilter_GetWatcher_WatchAddedOrRemoved               248 ms    412 ms         200.54 MB
            //EntityFilter_GetWatcher_WatchAddedOrUpdated               243 ms    501 ms         197.33 MB
            //EntityFilter_GetWatcher_WatchRemoved                      224 ms    393 ms         197.8 MB
            //EntityFilter_GetWatcher_WatchUpdated                      224 ms    429 ms         197.42 MB

            //EntityFilterGroup_GetEntity_GetEntitiesAfter              806 ms    710 ms         928.64 MB
            //EntityFilterGroup_GetEntity_GetEntitiesBefore             611 ms    -1 ms          718.82 MB
            //EntityFilterGroup_GetEntity_HasEntities                   37 ms     27 ms          2.06 GB

            //EntityGroup_GetEntity_GetEntitiesAfter                    789 ms    731 ms         2.3 GB
            //EntityGroup_GetEntity_GetEntitiesBefore                   604 ms    -1 ms          1.12 GB
            //EntityGroup_GetEntity_HasEntity                           38 ms     27 ms          2.08 GB

            //EntityGroup_GetWatcher_WatchAdded                         233 ms    360 ms         1.11 GB
            //EntityGroup_GetWatcher_WatchAddedOrRemoved                253 ms    391 ms         197.56 MB
            //EntityGroup_GetWatcher_WatchAddedOrUpdated                255 ms    436 ms         197.7 MB
            //EntityGroup_GetWatcher_WatchRemoved                       239 ms    417 ms         198.19 MB
            //EntityGroup_GetWatcher_WatchUpdated                       246 ms    423 ms         197.72 MB

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

                GC.Collect();
                GC.WaitForPendingFinalizers();

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