namespace EcsLte.PerformanceTest
{
	internal class WorldGetEntity : IPerformanceTest
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
			for (int i = 1; i < TestConsts.LoopCount; i++)
				_world.EntityManager.GetEntity(i);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}