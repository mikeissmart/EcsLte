using System;

namespace EcsLte
{
	[Serializable]
	public class EntityConfig
	{
		public int EntityId { get; set; }
		public ComponentConfig[] ComponentConfigs { get; set; }
	}
}