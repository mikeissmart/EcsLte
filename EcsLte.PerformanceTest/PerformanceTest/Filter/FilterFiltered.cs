namespace EcsLte.PerformanceTest
{
	internal class FilterFiltered : IPerformanceTest
	{
		private World _world;
		private Entity _entity;
		private Filter _filter;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_entity = _world.EntityManager.CreateEntity();
			_world.EntityManager.AddComponent<TestComponent1>(_entity);

			_filter = Filter.AllOf<TestComponent1, TestComponent2, TestrecordableComponent1>();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_filter.Filtered(_entity);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}