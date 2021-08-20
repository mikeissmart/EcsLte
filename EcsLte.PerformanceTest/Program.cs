using System;
using System.Diagnostics;
using System.Linq;
using EcsLte.Utilities;

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
                foreach (var test in testGrouping)
                    Run(test);
                Console.WriteLine("");
            }

            // Pre Events
            //Name                                                      Time      ParallelTime
            //EntityComponent_AddComponent:                             201 ms    101 ms    
            //EntityComponent_GetAllComponents:                         144 ms    146 ms    
            //EntityComponent_GetComponent:                             62 ms     17 ms     
            //EntityComponent_RemoveAllComponents:                      96 ms     23 ms     
            //EntityComponent_RemoveComponent:                          72 ms     23 ms     
            //EntityComponent_ReplaceComponent:                         198 ms    150 ms    

            //EntityLife_CreateEntities:                                573 ms    -1 ms     
            //EntityLife_CreateEntity:                                  516 ms    689 ms    
            //EntityLife_DestroyAllEntities:                            198 ms    -1 ms     
            //EntityLife_DestroyEntities:                               150 ms    -1 ms     
            //EntityLife_DestroyEntity:                                 169 ms    404 ms    
            //EntityLife_HasEntity:                                     32 ms     17 ms     

            //Filter_Equals:                                            11 ms     6 ms      
            //Filter_Filtered:                                          47 ms     15 ms

            Console.WriteLine("Press any key to continue...");
#if RELEASE
            Console.ReadKey();
#endif
        }

        private static void Run(Type testType)
        {
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
                test.PreRun();
                var parallelCount = test.ParallelRunCount();
                if (parallelCount > 0)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    ParallelRunner.RunParallel(parallelCount, test.RunParallel);
                    _stopwatch.Stop();

                    paralleltimes[i] = _stopwatch.ElapsedMilliseconds;
                    avgParallelTime += _stopwatch.ElapsedMilliseconds;
                }
                else
                {
                    avgParallelTime = -1;
                }

                test.PostRun();

#if RELEASE
                string timesText = "";
                for (int j = 0; j < loops; j++)
                    timesText += $"({times[j]} / {paralleltimes[j]}) ".PadRight(14);
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(
                    ($"//{testType.Name}: ") + timesText, 0);
#endif
            }

            avgTime /= loops;
            avgParallelTime = avgParallelTime > 0
                ? avgParallelTime /= loops
                : -1;

            // Clear line
#if RELEASE
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("".PadRight(110));

            Console.SetCursorPosition(0, Console.CursorTop);
#endif
            Console.WriteLine(
                $"//{testType.Name}: ".PadRight(60) +
                $"{avgTime} ms".PadRight(10) +
                $"{avgParallelTime} ms".PadRight(10));
        }
    }
}