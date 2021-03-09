using System.Collections.Generic;

namespace EcsLte
{
	public class EntityInfo
	{
		public EntityInfo()
		{
			GetComponents = new DataCache<IComponent[]>(UpdateGetComponents);
		}

		public int Id { get; set; }
		public int Generation { get; set; }
		public bool IsAlive { get; set; }
		public int[] ComponentIndexes { get; set; }
		public World WorldOwner { get; set; }
		public DataCache<IComponent[]> GetComponents { get; set; }

		private IComponent[] UpdateGetComponents()
		{
			var components = new List<IComponent>();
			for (int i = 0; i < ComponentIndexes.Length; i++)
			{
				if (ComponentIndexes[i] != 0)
					components.Add(WorldOwner.ComponentPools[i].GetComponent(ComponentIndexes[i]));
			}

			return components.ToArray();
		}
	}
}