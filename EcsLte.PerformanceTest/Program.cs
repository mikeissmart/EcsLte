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

            //Run(typeof(EcsContext_EntityLife_CreateEntitiesBlueprint));

            //Name                                                      Time      ParallelTime
            //EcsContext_ComponentLife_AddComponent                     331 ms    419 ms         1.07 GB
            //EcsContext_ComponentLife_AddComponent2                    559 ms    513 ms         1.66 GB
            //EcsContext_ComponentLife_AddComponent3                    519 ms    512 ms         1.55 GB
            //EcsContext_ComponentLife_RemoveAllComponents              121 ms    337 ms         1.41 GB
            //EcsContext_ComponentLife_RemoveComponent                  499 ms    499 ms         2.16 GB
            //EcsContext_ComponentLife_ReplaceComponent                 158 ms    15 ms          2.14 GB

            //EcsContext_EntityGroup_SharedComponent                    359 ms    318 ms         3.1 GB
            //EcsContext_EntityGroup_SharedComponentX10                 1039 ms   389 ms         3.1 GB
            //EcsContext_EntityGroup_SharedComponentX2                  412 ms    335 ms         3.1 GB

            //EcsContext_EntityLife_CreateEntities                      120 ms    -1 ms          912.74 MB
            //EcsContext_EntityLife_CreateEntitiesBlueprint             207 ms    -1 ms          475.29 MB
            //EcsContext_EntityLife_CreateEntity                        341 ms    821 ms         2.46 GB
            //EcsContext_EntityLife_CreateEntityBlueprint               692 ms    859 ms         2.13 GB
            //EcsContext_EntityLife_CreateEntityReuse                   55 ms     332 ms         1.58 GB
            //EcsContext_EntityLife_DestroyEntities                     78 ms     -1 ms          357.54 MB
            //EcsContext_EntityLife_DestroyEntity                       98 ms     298 ms         1.27 GB

            //EcsContext_FilterByAll                                    893 ms    1039 ms        1.8 GB
            //EcsContext_FilterByOne                                    193 ms    371 ms         1.8 GB

            //EcsContext_GetComponent_GetAllComponents                  105 ms    39 ms          1.79 GB
            //EcsContext_GetComponent_GetComponent                      104 ms    36 ms          1.79 GB
            //EcsContext_GetComponent_GetUniqueComponent                162 ms    36 ms          1.79 GB
            //EcsContext_GetComponent_GetUniqueEntity                   47 ms     26 ms          1.79 GB
            //EcsContext_GetComponent_HasComponent                      67 ms     36 ms          1.79 GB
            //EcsContext_GetComponent_HasUniqueComponent                38 ms     27 ms          1.79 GB

            //EcsContext_GetEntity_GetEntities                          46 ms     -1 ms          663.76 MB
            //EcsContext_GetEntity_HasEntity                            37 ms     28 ms          1.2 GB

            //EntityCommandQueue_ComponentLife_AddComponent             539 ms    813 ms         962.57 MB
            //EntityCommandQueue_ComponentLife_RemoveAllComponents      229 ms    528 ms         1.51 GB
            //EntityCommandQueue_ComponentLife_RemoveComponent          676 ms    973 ms         1.42 GB
            //EntityCommandQueue_ComponentLife_ReplaceComponent         315 ms    619 ms         1.42 GB

            //EntityCommandQueue_EntityLife_CreateEntities              232 ms    -1 ms          833.98 MB
            //EntityCommandQueue_EntityLife_CreateEntitiesBlueprint     454 ms    -1 ms          504.85 MB
            //EntityCommandQueue_EntityLife_CreateEntity                451 ms    768 ms         1.63 GB
            //EntityCommandQueue_EntityLife_CreateEntityBlueprint       765 ms    1047 ms        1.31 GB
            //EntityCommandQueue_EntityLife_DestroyEntities             229 ms    -1 ms          1019.48 MB
            //EntityCommandQueue_EntityLife_DestroyEntity               224 ms    500 ms         1.18 GB

            //EntityFilter_GetEntity_GetEntitiesAfter                   394 ms    692 ms         2.15 GB
            //EntityFilter_GetEntity_GetEntitiesBefore                  683 ms    -1 ms          778.05 MB
            //EntityFilter_GetEntity_HasEntity                          62 ms     36 ms          1.59 GB

            //EntityFilter_GetWatcher_Added                             265 ms    351 ms         4.41 GB
            //EntityFilter_GetWatcher_AddedOrRemoved                    285 ms    361 ms         4.41 GB
            //EntityFilter_GetWatcher_AddedOrUpdated                    290 ms    378 ms         4.41 GB
            //EntityFilter_GetWatcher_Removed                           255 ms    343 ms         4.41 GB
            //EntityFilter_GetWatcher_Updated                           251 ms    347 ms         4.41 GB

            //EntityGroup_GetEntity_GetEntitiesAfter                    848 ms    734 ms         2.17 GB
            //EntityGroup_GetEntity_GetEntitiesBefore                   363 ms    -1 ms          519.23 MB
            //EntityGroup_GetEntity_HasEntity                           62 ms     36 ms          2.04 GB

            //EntityGroup_GetWatcher_Added                              251 ms    344 ms         1.89 GB
            //EntityGroup_GetWatcher_AddedOrRemoved                     284 ms    380 ms         1.89 GB
            //EntityGroup_GetWatcher_AddedOrUpdated                     288 ms    373 ms         1.89 GB
            //EntityGroup_GetWatcher_Removed                            264 ms    354 ms         1.89 GB
            //EntityGroup_GetWatcher_Updated                            264 ms    351 ms         1.89 GB

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