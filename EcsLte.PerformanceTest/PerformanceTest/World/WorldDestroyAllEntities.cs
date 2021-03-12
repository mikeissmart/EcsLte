namespace EcsLte.PerformanceTest
{
	internal class WorldDestroyAllEntities : IPerformanceTest
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
			_world.EntityManager.DestroyAllEntities();
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}