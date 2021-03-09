namespace EcsLte.PerformanceTest
{
	internal class WorldDestroyEntity : IPerformanceTest
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
			var entities = _world.GetEntities();
			for (int i = 0; i < entities.Length; i++)
				entities[i].Destroy();
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}