using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public class SharedKey<TComponent> : ISharedKey
		where TComponent : IComponent
	{
		private static readonly List<Entity> _entityBuffer = new List<Entity>();

		private readonly EntityManager _entityManager;
		private readonly Dictionary<TComponent, HashSet<Entity>> _keyLookup;
		private readonly int _componentPoolIndex;

		internal SharedKey(EntityManager entityManager, EntityKeyInfo keyInfo, Group group)
		{
			_entityManager = entityManager;
			_keyLookup = new Dictionary<TComponent, HashSet<Entity>>(
				new EntityInfoComparer<TComponent> { KeyInfo = keyInfo });
			_componentPoolIndex = ComponentIndex<TComponent>.Index;

			ComponentType = typeof(TComponent);
			Group = group;

			group.EntityAddedEvent.Subscribe(OnGroupEntityAdded);
			group.EntityRemovedEvent.Subscribe(OnGroupEntityRemoved);
			group.EntityUpdatedEvent.Subscribe(OnGroupEntityUpdated);

			int componentIndex = ComponentIndex<TComponent>.Index;
			if (!(group.Filter.AllOfIndexes.Any(x => x == componentIndex) ||
				group.Filter.AnyOfIndexes.All(x => x == componentIndex)))
				// TODO: throw proper exception
				throw new Exception("Group does not have component in filter all or any.");

			foreach (var entity in group.Entities)
			{
				var component = _entityManager.GetComponent(entity, _componentPoolIndex);
				if (component != null)
					OnGroupEntityAdded(entity, _componentPoolIndex, component);
			}
		}

		public Type ComponentType { get; private set; }
		public Group Group { get; private set; }
		public bool IsDestroyed { get; set; }

		public HashSet<Entity> GetEntities(IComponent component)
		{
			if (IsDestroyed)
				throw new SharedKeyIsDestroyedException(this);
			if (!(component is TComponent))
				// throw proper exception
				throw new Exception("Component is not the same type as TComponent.");

			if (_keyLookup.TryGetValue((TComponent)component, out HashSet<Entity> entities))
				return entities;
			return new HashSet<Entity>();
		}

		public Entity GetFirstOrSingleEntity(IComponent component)
		{
			var entities = GetEntities(component);
			if (entities.Count > 0)
				return entities.ElementAt(0);
			return Entity.Null;
		}

		private void OnGroupEntityAdded(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (componentPoolIndex == _componentPoolIndex)
			{
				if (!_keyLookup.TryGetValue((TComponent)component, out HashSet<Entity> entities))
				{
					entities = new HashSet<Entity>();
					_keyLookup.Add((TComponent)component, entities);
				}

				entities.Add(entity);
			}
		}

		private void OnGroupEntityRemoved(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (componentPoolIndex == _componentPoolIndex)
			{
				if (_keyLookup.TryGetValue((TComponent)component, out HashSet<Entity> entities))
					entities.Remove(entity);
			}
		}

		private void OnGroupEntityUpdated(Entity entity, int componentPoolIndex, IComponent prevComponent, IComponent newComponent)
		{
			OnGroupEntityRemoved(entity, componentPoolIndex, prevComponent);
			OnGroupEntityAdded(entity, componentPoolIndex, newComponent);
		}
	}
}