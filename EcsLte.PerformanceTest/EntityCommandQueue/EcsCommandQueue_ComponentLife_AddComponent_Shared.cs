using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EntityCommandQueue_ComponentLife_AddComponent_Shared : BasePerformanceTest
	{
		private Entity[] _entities;

		public override void PreRun()
		{
			base.PreRun();

			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
		}

		public override void Run()
		{
			var component = new TestSharedComponent1();
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				_context.DefaultCommand.AddComponent(_entities[i], component);
			_context.DefaultCommand.RunCommands();
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			var component = new TestSharedComponent1();
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { _context.DefaultCommand.AddComponent(_entities[i], component); });
			_context.DefaultCommand.RunCommands();
		}
	}
}