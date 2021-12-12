namespace EcsLte.PerformanceTest
{
	internal class EntityFilterGroup_GetEntity_GetEntities : BasePerformanceTest
	{
		private EntityFilterGroup _entityFilterGroup;

		public override void PreRun()
		{
			base.PreRun();

			var sharedComponent = new TestSharedComponent1 { Prop = 1 };
			var standardComponent = new TestStandardComponent1 { Prop = 1 };
			_context.CreateEntities(TestConsts.EntityLoopCount, new EntityBlueprint()
				.AddComponent(sharedComponent)
				.AddComponent(standardComponent));
			_entityFilterGroup = _context.FilterByGroupWith(
				Filter.AllOf<TestSharedComponent1>(), sharedComponent);
		}

		public override void Run()
		{
			Entity[] entities;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				entities = _entityFilterGroup.GetEntities();
		}
	}
}