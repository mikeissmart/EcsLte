using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
	internal delegate void ComponentArcheTypeDataEvent(ComponentArcheTypeData archeTypeData);

	internal class ComponentArcheTypeData : IGetEntity
	{
		private DataCache<Dictionary<int, Entity>, Entity[]> _entities;
		private readonly object _modifyLock;

		public ComponentArcheTypeData()
		{
			_entities = new DataCache<Dictionary<int, Entity>, Entity[]>(
					new Dictionary<int, Entity>(), UpdateCachedData);
			_modifyLock = new object();
		}

		internal ComponentArcheType ArcheType { get; private set; }
		internal int Count => _entities.UncachedData.Count;

		internal static ComponentArcheTypeData Initialize(ComponentArcheType archeType)
		{
			var data = ObjectCache<ComponentArcheTypeData>.Pop();

			data.ArcheType = archeType;
			data._entities = new DataCache<Dictionary<int, Entity>, Entity[]>(
				new Dictionary<int, Entity>(), UpdateCachedData);

			return data;
		}

		internal static void Uninitialize(ComponentArcheTypeData data)
		{
			if (data.ArcheTypeDataRemoved != null)
				data.ArcheTypeDataRemoved.Invoke(data);

			lock (data._modifyLock)
			{
				data._entities.UncachedData.Clear();
				data._entities.SetDirty();
			}
			data.EntityAdded = null;
			data.EntityRemoved = null;
			data.EntityUpdated = null;
			data.ArcheTypeDataRemoved = null;

			ObjectCache<ComponentArcheTypeData>.Push(data);
		}

		internal event EntityEvent EntityAdded;
		internal event EntityEvent EntityRemoved;
		internal event EntityEvent EntityUpdated;
		internal event ComponentArcheTypeDataEvent ArcheTypeDataRemoved;

		#region ComponentArcheTypeData

		internal void AddEntity(Entity entity)
		{
			lock (_modifyLock)
			{
				_entities.UncachedData.Add(entity.Id, entity);
				_entities.SetDirty();

				if (EntityAdded != null)
					EntityAdded.Invoke(entity);
			}
		}

		internal void RemoveEntity(Entity entity)
		{
			lock (_modifyLock)
			{
				_entities.UncachedData.Remove(entity.Id);
				_entities.SetDirty();

				if (EntityRemoved != null)
					EntityRemoved.Invoke(entity);
			}
		}

		internal void UpdateEntity(Entity entity)
		{
			if (EntityUpdated != null)
				EntityUpdated.Invoke(entity);
		}

		private static Entity[] UpdateCachedData(Dictionary<int, Entity> unchacedData) => unchacedData.Values.ToArray();

		#endregion

		#region GetEntity

		public bool HasEntity(Entity entity) => _entities.UncachedData.ContainsKey(entity.Id);

		public Entity[] GetEntities() => _entities.CachedData;

		#endregion
	}
}