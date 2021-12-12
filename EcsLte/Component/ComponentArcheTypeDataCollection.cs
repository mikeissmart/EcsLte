using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte
{
	internal class ComponentArcheTypeDataCollection
	{
		private readonly DataCache<List<ComponentArcheTypeData>, ComponentArcheTypeData[]> _archeTypeDatas;

		public ComponentArcheTypeDataCollection() => _archeTypeDatas = new DataCache<List<ComponentArcheTypeData>, ComponentArcheTypeData[]>(
				new List<ComponentArcheTypeData>(),
				UpdateCachedData);

		internal ComponentArcheTypeData[] ArcheTypeDatas => _archeTypeDatas.CachedData;

		internal static ComponentArcheTypeDataCollection Initialize(ComponentArcheTypeData[] initialArcheTypeDatas)
		{
			var data = ObjectCache<ComponentArcheTypeDataCollection>.Pop();

			data._archeTypeDatas.UncachedData.AddRange(initialArcheTypeDatas);
			data._archeTypeDatas.SetDirty();

			return data;
		}

		internal static void Uninitialize(ComponentArcheTypeDataCollection data)
		{
			data._archeTypeDatas.UncachedData.Clear();
			data._archeTypeDatas.SetDirty();

			ObjectCache<ComponentArcheTypeDataCollection>.Push(data);
		}

		internal void AddComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
		{
			lock (_archeTypeDatas)
			{
				_archeTypeDatas.UncachedData.Add(archeTypeData);
				_archeTypeDatas.SetDirty();
			}
		}

		internal bool RemoveComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
		{
			lock (_archeTypeDatas)
			{
				if (_archeTypeDatas.UncachedData.Remove(archeTypeData))
				{
					_archeTypeDatas.SetDirty();
					return true;
				}
			}

			return false;
		}

		private static ComponentArcheTypeData[] UpdateCachedData(List<ComponentArcheTypeData> uncachedData) => uncachedData.ToArray();
	}
}