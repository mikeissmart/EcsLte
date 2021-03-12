namespace EcsLte.PerformanceTest
{
	internal class WorldDestroyEntity : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.CreateEntity();
		}

		public void Run()
		{
			var entities = _world.EntityManager.GetEntities();
			for (int i = 0; i < entities.Length; i++)
				_world.EntityManager.DestroyEntity(entities[i]);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}