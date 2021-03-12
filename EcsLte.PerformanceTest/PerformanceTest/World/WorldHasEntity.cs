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
				_world.EntityManager.CreateEntity();
			_entity = _world.EntityManager.CreateEntity();
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.HasEntity(_entity);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}