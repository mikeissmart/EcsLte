namespace EcsLte.PerformanceTest
{
	public class WorldJsonSerialize : IPerformanceTest
	{
		private World _world;

		public void PreRun()
		{
			_world = World.CreateWorld();
			var entity = _world.EntityManager.CreateEntity();
			_world.EntityManager.AddComponent<TestComponent1>(entity);
		}

		public void Run()
		{
			/*var serializeSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.All
			};

			var worldRecordableData = _world.GetRecordableData();
			var json = JsonConvert.SerializeObject(worldRecordableData, serializeSettings);
			json = json;

			var loadData = JsonConvert.DeserializeObject<WorldRecordableData>(json, serializeSettings);
			loadData = loadData;*/
		}

		public void PostRun()
		{
			World.DestroyWorld(_world);
		}
	}
}