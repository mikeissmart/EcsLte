using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EcsContext_ComponentLife_RemoveComponent_Standard : BasePerformanceTest
	{
		private Entity[] _entities;

		public override void PreRun()
		{
			base.PreRun();

			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
			var component = new TestStandardComponent1();
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				_context.AddComponent(_entities[i], component);
		}

		public override void Run()
		{
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				_context.RemoveComponent<TestStandardComponent1>(_entities[i]);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel() => ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { _context.RemoveComponent<TestStandardComponent1>(_entities[i]); });
	}
}