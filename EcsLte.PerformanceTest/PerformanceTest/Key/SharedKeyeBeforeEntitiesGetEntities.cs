namespace EcsLte.PerformanceTest
{
	internal class SharedKeyeBeforeEntitiesGetEntities : IPerformanceTest
	{
		private World _world;
		private ISharedKey _sharedKey;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_sharedKey = _world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(
				_world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>()));

			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.AddComponent(
					_world.EntityManager.CreateEntity(),
					new TestSharedKeyComponent1 { Prop = 1 });
		}

		public void Run()
		{
			var keyComponent = new TestSharedKeyComponent1 { Prop = 1 };
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_sharedKey.GetEntities(keyComponent);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}