namespace EcsLte.PerformanceTest
{
	internal class PrimaryKeyAfterEntitiesGetEntity : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();

			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.AddComponent(
					_world.EntityManager.CreateEntity(),
					new TestSharedKeyComponent1 { Prop = 1 });
		}

		public void Run()
		{
			var primaryKey = _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(
				_world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>()));

			var keyComponent = new TestPrimaryKeyComponent1 { Prop = 1 };
			for (int i = 0; i < TestConsts.LoopCount; i++)
				primaryKey.GetEntity(keyComponent);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}