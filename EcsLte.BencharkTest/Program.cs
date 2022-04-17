using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using EcsLte.BencharkTest.Exporters;
using System;
using System.Threading;

namespace EcsLte.BencharkTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            var config = new DebugInProcessConfig()
                //.WithOptions(ConfigOptions.DisableLogFile)
                .AddExporter(EcsContextCsvExcelExporter.Default);
            //.AddExporter(CsvExporter.Default);

            //BenchmarkRunner.Run(typeof(Program).Assembly, new DebugInProcessConfig());
#else
			var config = ManualConfig.Create(DefaultConfig.Instance);
			config.ArtifactsPath = "../../Artifacts";
#endif
            config.WithOptions(ConfigOptions.JoinSummary)
                    //.WithOptions(ConfigOptions.DisableLogFile)
                    //.AddColumn(CategoriesColumn.Default)
                    .AddExporter(EcsContextCsvExcelExporter.Default)
                    .AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByMethod)
                    .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Alphabetical));
#if DEBUG
            BenchmarkRunner.Run<EcsContextTests.EcsContext_HybridTests_ChangeComponent>(config);
            //BenchmarkRunner.Run(typeof(Program).Assembly, config);
            Console.WriteLine("done");
            Console.ReadLine();
#else
			//BenchmarkRunner.Run(typeof(Program).Assembly, config);
            BenchmarkRunner.Run<EcsContextTests.EcsContext_HybridTests_CreateEntityAddComponent_Reuse>(config);
#endif
        }
    }

#if DEBUG
    public class TestBech
    {
        private Random _random;

        /*[ParamsAllValues]
		public EntityComponentArrangement ComponentArrangement { get; set; }*/

        [ParamsAllValues]
        public EcsContextType ContextType { get; set; }

        [GlobalSetup]
        public void GlobalSetup() => _random = new Random();

        [GlobalCleanup]
        public void GlobalCleanup()
        {
        }

        [IterationSetup]
        public void IterationSetup()
        {
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
        }

        [Benchmark]
        public void Bechmark1() => Thread.Sleep(_random.Next(0, 1));

        [Benchmark]
        public void Bechmark2() => Thread.Sleep(_random.Next(0, 1));
    }
#endif
}
