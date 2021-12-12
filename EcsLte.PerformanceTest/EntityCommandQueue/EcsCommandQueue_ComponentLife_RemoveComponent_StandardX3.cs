using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EntityCommandQueue_ComponentLife_RemoveComponent_StandardX3 : BasePerformanceTest
	{
		private Entity[] _entities;

		public override void PreRun()
		{
			base.PreRun();

			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
			var component1 = new TestStandardComponent1();
			var component2 = new TestStandardComponent2();
			var component3 = new TestStandardComponent3();
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.AddComponent(_entities[i], component1);
				_context.AddComponent(_entities[i], component2);
				_context.AddComponent(_entities[i], component3);
			}
		}

		public override void Run()
		{
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.DefaultCommand.RemoveComponent<TestStandardComponent1>(_entities[i]);
				_context.DefaultCommand.RemoveComponent<TestStandardComponent2>(_entities[i]);
				_context.DefaultCommand.RemoveComponent<TestStandardComponent3>(_entities[i]);
			}
			_context.DefaultCommand.RunCommands();
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i =>
				{
					_context.DefaultCommand.RemoveComponent<TestStandardComponent1>(_entities[i]);
					_context.DefaultCommand.RemoveComponent<TestStandardComponent2>(_entities[i]);
					_context.DefaultCommand.RemoveComponent<TestStandardComponent3>(_entities[i]);
				});
			_context.DefaultCommand.RunCommands();
		}
	}
}