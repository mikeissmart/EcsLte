using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EcsContext_EntityLife_CreateEntity : BasePerformanceTest
	{
		public override void Run()
		{
			Entity entity;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				entity = _context.CreateEntity();
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			Entity entity;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { entity = _context.CreateEntity(); });
		}
	}
}