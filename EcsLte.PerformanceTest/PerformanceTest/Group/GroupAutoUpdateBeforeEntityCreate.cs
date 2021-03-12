namespace EcsLte.PerformanceTest
{
	internal class GroupAutoUpdateBeforeEntityCreate : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
		}

		public void Run()
		{
			for (int i = 0; i < TestConsts.LoopCount; i++)
			{
				var entity = _world.EntityManager.CreateEntity();
				_world.EntityManager.AddComponent<TestComponent1>(entity);
			}
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}