namespace EcsLte.PerformanceTest
{
	internal class SharedKeyAfterEntitiesGetEntities : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();

			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.AddComponent(
					_world.EntityManager.CreateEntity(),
					new TestComponent1 { Prop = 1 });
		}

		public void Run()
		{
			var sharedKey = _world.KeyManager.GetSharedKey<TestComponent1>(
				_world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>()));

			var keyComponent = new TestComponent1 { Prop = 1 };
			for (int i = 0; i < TestConsts.LoopCount; i++)
				sharedKey.GetEntities(keyComponent);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}