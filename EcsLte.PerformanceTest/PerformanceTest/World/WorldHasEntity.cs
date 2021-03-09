namespace EcsLte.PerformanceTest
{
	internal class WorldHasEntity : IPerformanceTest
	{
		private World _world;
		private Entity _entity;

		public void PreRun()
		{
			_world = World.CreateWorld();
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.CreateEntity();
			_entity = _world.CreateEntity();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.HasEntity(_entity);
		}

		public void PostRun()
		{
			_world.DestroyWorld();
		}
	}
}