using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System;
using System.Threading;

namespace EcsLte.BenchmarkTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var config = new DebugInProcessConfig();
#else
			var config = ManualConfig.Create(DefaultConfig.Instance);
			config.ArtifactsPath = "../../TestResults";
#endif
            config = config.WithOptions(ConfigOptions.JoinSummary)
                .StopOnFirstError(true)
                //.WithOptions(ConfigOptions.DisableLogFile)
                //.AddColumn(CategoriesColumn.Default)
                .AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByMethod)
                .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Alphabetical));
            ;
#if DEBUG
            BenchmarkRunner.Run(typeof(Program).Assembly, config);
            Console.WriteLine("done");
            Console.ReadLine();
#else
            BenchmarkRunner.Run(typeof(Program).Assembly, config);
            //BenchmarkRunner.Run<EcsContextTests.EcsContext_EntityComponent>(config);
#endif
        }
    }

    /*public class TestBech
    {
        [Benchmark]
        public void Bechmark()
        {
            Thread.Sleep(50);
        }
    }*/
}
