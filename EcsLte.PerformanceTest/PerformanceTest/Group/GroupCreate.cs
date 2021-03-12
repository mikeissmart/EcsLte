namespace EcsLte.PerformanceTest
{
	internal class GroupCreate : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
		}

		public void Run()
		{
			var filter = Filter.AllOf<TestComponent1>();
			for (int i = 0; i < TestConsts.LoopCount; i++)
				_world.GroupManager.GetGroup(filter);
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}