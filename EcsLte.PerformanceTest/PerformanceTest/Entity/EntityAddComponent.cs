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
				_entities[i] = _world.CreateEntity();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_entities[i].AddComponent<TestComponent1>();
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}