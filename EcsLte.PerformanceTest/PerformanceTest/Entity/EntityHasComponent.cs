namespace EcsLte.PerformanceTest
{
	internal class EntityHasComponent : IPerformanceTest
	{
		private World _world;
		private Entity[] _entities;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_entities = new Entity[TestConsts.LoopCount];
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_entities[i] = _world.CreateEntity();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_entities[i].HasComponent<TestComponent1>();
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}