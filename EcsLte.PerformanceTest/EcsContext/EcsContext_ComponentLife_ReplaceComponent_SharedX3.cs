using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EcsContext_ComponentLife_ReplaceComponent_SharedX3 : BasePerformanceTest
	{
		private Entity[] _entities;

		public override void PreRun()
		{
			base.PreRun();

			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
			var component1 = new TestSharedComponent1();
			var component2 = new TestSharedComponent2();
			var component3 = new TestSharedComponent3();
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.AddComponent(_entities[i], component1);
				_context.AddComponent(_entities[i], component2);
				_context.AddComponent(_entities[i], component3);
			}
		}

		public override void Run()
		{
			var component1 = new TestSharedComponent1 { Prop = 1 };
			var component2 = new TestSharedComponent2 { Prop = 2 };
			var component3 = new TestSharedComponent2 { Prop = 3 };
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.ReplaceComponent(_entities[i], component1);
				_context.ReplaceComponent(_entities[i], component2);
				_context.ReplaceComponent(_entities[i], component3);
			}
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			var component1 = new TestSharedComponent1 { Prop = 1 };
			var component2 = new TestSharedComponent2 { Prop = 2 };
			var component3 = new TestSharedComponent2 { Prop = 3 };
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i =>
				{
					_context.ReplaceComponent(_entities[i], component1);
					_context.ReplaceComponent(_entities[i], component2);
					_context.ReplaceComponent(_entities[i], component3);
				});
		}
	}
}