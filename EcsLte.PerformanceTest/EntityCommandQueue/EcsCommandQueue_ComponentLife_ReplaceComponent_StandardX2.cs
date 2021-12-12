using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EntityCommandQueue_ComponentLife_ReplaceComponent_StandardX2 : BasePerformanceTest
	{
		private Entity[] _entities;

		public override void PreRun()
		{
			base.PreRun();

			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
			var component1 = new TestStandardComponent1();
			var component2 = new TestStandardComponent2();
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.AddComponent(_entities[i], component1);
				_context.AddComponent(_entities[i], component2);
			}
		}

		public override void Run()
		{
			var component1 = new TestStandardComponent1 { Prop = 1 };
			var component2 = new TestStandardComponent2 { Prop = 2 };
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.DefaultCommand.ReplaceComponent(_entities[i], component1);
				_context.DefaultCommand.ReplaceComponent(_entities[i], component2);
			}
			_context.DefaultCommand.RunCommands();
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			var component1 = new TestStandardComponent1 { Prop = 1 };
			var component2 = new TestStandardComponent2 { Prop = 2 };
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i =>
				{
					_context.DefaultCommand.ReplaceComponent(_entities[i], component1);
					_context.DefaultCommand.ReplaceComponent(_entities[i], component2);
				});
			_context.DefaultCommand.RunCommands();
		}
	}
}