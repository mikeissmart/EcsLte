namespace EcsLte.PerformanceTest
{
	internal class GroupAutoUpdateAfterEntityCreate : IPerformanceTest
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
			_world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}