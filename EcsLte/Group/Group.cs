using System;
using System.Collections.Generic;

namespace EcsLte
{
	public class Group : IEquatable<Group>
	{
		private readonly HashSet<Entity> _entities;
		private readonly DataCache<Entity[]> _entitiesCache;

		internal Group(GroupManager groupManager, Filter filter)
		{
			_entities = new HashSet<Entity>();
			_entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);

			GroupManager = groupManager;
			Filter = filter;

			EntityAddedEvent = new GroupChangedEvent();
			EntityRemovedEvent = new GroupChangedEvent();
			EntityUpdatedEvent = new GroupUpdatedEvent();
		}

		public GroupManager GroupManager { get; private set; }
		public int[] Indexes { get => Filter.Indexes; }
		public Filter Filter { get; private set; }
		public Entity[] Entities { get => _entitiesCache.Data; }

		internal GroupChangedEvent EntityAddedEvent { get; private set; }
		internal GroupChangedEvent EntityRemovedEvent { get; private set; }
		internal GroupUpdatedEvent EntityUpdatedEvent { get; private set; }

		public bool ContainsEntity(Entity entity)
			=> _entities.Contains(entity);

		public bool Equals(Group other)
		{
			if (other == null)
				return false;
			return Filter == other.Filter && GroupManager == other.GroupManager;
		}

		internal void FilterEntitySilent(Entity entity)
		{
			if (Filter.Filtered(entity))
			{
				if (_entities.Add(entity))
					_entitiesCache.IsDirty = true;
			}
			else
			{
				if (_entities.Remove(entity))
					_entitiesCache.IsDirty = true;
			}
		}

		internal void FilterEntity(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (Filter.Filtered(entity))
			{
				if (_entities.Add(entity))
				{
					_entitiesCache.IsDirty = true;
					EntityAddedEvent.Invoke(entity, componentPoolIndex, component);
				}
			}
			else
			{
				if (_entities.Remove(entity))
				{
					_entitiesCache.IsDirty = true;
					EntityRemovedEvent.Invoke(entity, componentPoolIndex, component);
				}
			}
		}

		internal void UpdateEntity(Entity entity, int componentPoolIndex, IComponent prevComponent, IComponent newComponent)
		{
			if (_entities.Contains(entity))
			{
				EntityRemovedEvent.Invoke(entity, componentPoolIndex, prevComponent);
				EntityAddedEvent.Invoke(entity, componentPoolIndex, newComponent);
				EntityUpdatedEvent.Invoke(entity, componentPoolIndex, prevComponent, newComponent);
			}
		}

		private Entity[] UpdateEntitiesCache()
		{
			var entites = new Entity[_entities.Count];
			_entities.CopyTo(entites);

			return entites;
		}
	}
}