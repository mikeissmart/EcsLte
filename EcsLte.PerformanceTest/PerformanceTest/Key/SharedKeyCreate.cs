namespace EcsLte.PerformanceTest
{
	internal class SharedKeyCreate : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
		}

		public void Run()
		{
			var group = _world.GroupManager.GetGroup(Filter.AllOf<TestSharedKeyComponent1>());
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.KeyManager.GetSharedKey<TestSharedKeyComponent1>(group);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}