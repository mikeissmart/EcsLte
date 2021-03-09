using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace EcsLte.PerformanceTest
{
	internal static class SaveLoadTestHistory
	{
		public static void Save(string fileName, List<TestHistory> testHistories)
		{
			var serializeSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.All
			};
			File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName), JsonConvert.SerializeObject(
				testHistories.ToArray(),
				serializeSettings));
		}

		public static List<TestHistory> Load(string fileName)
		{
			var serializeSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.All
			};

			if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
				return new List<TestHistory>();
			return new List<TestHistory>(JsonConvert.DeserializeObject<TestHistory[]>(
				File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName)),
				serializeSettings));
		}
	}
}