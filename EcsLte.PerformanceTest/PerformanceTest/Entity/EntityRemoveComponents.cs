namespace EcsLte.PerformanceTest
{
	internal class EntityRemoveComponents : IPerformanceTest
	{
		private World _world;
		private Entity[] _entities;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_entities = new Entity[TestConsts.LoopCount];
			for (int i = 0; i < TestConsts.LoopCount; i++)
			{
				var entity = _world.CreateEntity();
				entity.AddComponent<TestComponent1>();
				_entities[i] = entity;
			}
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_entities[i].RemoveComponents();
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}