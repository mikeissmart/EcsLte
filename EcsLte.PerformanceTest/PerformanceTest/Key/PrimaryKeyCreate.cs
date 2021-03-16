namespace EcsLte.PerformanceTest
{
	internal class PrimaryKeyCreate : IPerformanceTest
	{
		private World _world;
		private IPrimaryKey _primaryKey;

		public void PreRun()
		{
			_world = World.CreateWorld();
			_primaryKey = _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>(
				_world.GroupManager.GetGroup(Filter.AllOf<TestPrimaryKeyComponent1>()));

			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.EntityManager.AddComponent(
					_world.EntityManager.CreateEntity(),
					new TestPrimaryKeyComponent1 { Prop = i });
		}

		public void Run()
		{
			var keyComponent = new TestPrimaryKeyComponent1 { Prop = 1 };
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_primaryKey.GetEntity(keyComponent);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}