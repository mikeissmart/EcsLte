namespace EcsLte.PerformanceTest
{
	internal class WorldCreateEntity : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.CreateEntity();
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}