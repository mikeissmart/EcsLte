namespace EcsLte.PerformanceTest
{
	internal class WorldDestroyAllEntities : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.CreateEntity();
		}

		public void Run()
		{
			_world.DestroyAllEntities();
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}