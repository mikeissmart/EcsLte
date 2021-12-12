using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
	internal class EntityGroup_GetEntity_HasEntity : BasePerformanceTest
	{
		private Entity[] _entities;
		private EntityGroup _entityGroup;

		public override void PreRun()
		{
			base.PreRun();

			var component = new TestSharedComponent1 { Prop = 1 };
			_entityGroup = _context.GroupWith(component);
			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				_context.AddComponent(_entities[i], component);
		}

		public override void Run()
		{
			bool hasEntity;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				hasEntity = _entityGroup.HasEntity(_entities[i]);
		}

		public override bool CanRunParallel() => true;

		public override void RunParallel()
		{
			bool hasEntity;
			ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
				i => { hasEntity = _entityGroup.HasEntity(_entities[i]); });
		}
	}
}