namespace EcsLte.PerformanceTest
{
	internal class EntityAddComponent : IPerformanceTest
	{
		private World _world;
		private Entity[] _entities;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_entities = new Entity[TestConsts.LoopCount];
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_entities[i] = _world.EntityManager.CreateEntity();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.AddComponent<TestComponent1>(_entities[i]);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}