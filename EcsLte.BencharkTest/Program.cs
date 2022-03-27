using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using EcsLte.BencharkTest.Data.Unmanaged;
using EcsLte.BencharkTest.EcsContextTests;
using EcsLte.NativeArcheType;

namespace EcsLte.BencharkTest
{
	internal class Program
	{
		static void Main(string[] args)
		{
#if DEBUG
			//BenchmarkRunner.Run(typeof(Program).Assembly, new DebugInProcessConfig());
			//BenchmarkRunner.Run<TestBech>(new DebugInProcessConfig());
#else
			var config = ManualConfig.Create(DefaultConfig.Instance)
					.WithOptions(ConfigOptions.JoinSummary)
					//.WithOptions(ConfigOptions.DisableLogFile)
					//.AddColumn(CategoriesColumn.Default)
					.AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByMethod)
					.WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Alphabetical));
			BenchmarkRunner.Run(typeof(Program).Assembly, config);
			//BenchmarkRunner.Run<EcsContext_EntityComponentGetTest>(config);
#endif
		}
	}
}
