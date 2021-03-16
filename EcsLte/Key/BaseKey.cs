using EcsLte.Exceptions;
using System;
using System.Linq;

namespace EcsLte
{
	public abstract class BaseKey
	{
		protected readonly EntityManager _entityManager;
		protected readonly int _componentPoolIndex;

		protected BaseKey(EntityManager entityManager, Group group, int componentPoolIndex, Type componentType)
		{
			_entityManager = entityManager;
			_componentPoolIndex = componentPoolIndex;

			Group = group;
			ComponentType = componentType;

			group.EntityAddedEvent.Subscribe(OnGroupEntityAdded);
			group.EntityRemovedEvent.Subscribe(OnGroupEntityRemoved);
			group.EntityUpdatedEvent.Subscribe(OnGroupEntityUpdated);

			if (!group.Filter.AllOfIndexes.Any(x => x == componentPoolIndex) &&
				!group.Filter.AnyOfIndexes.Any(x => x == componentPoolIndex))
				throw new KeyGroupComponentNotInAllOrAnyException(this, componentType);
		}

		public Type ComponentType { get; private set; }
		public Group Group { get; private set; }
		public bool IsDestroyed { get; internal set; }

		protected abstract void GroupEntityAddedEvent(Entity entity, int componentPoolIndex, IComponent component);

		protected abstract void GroupEntityRemovedEvent(Entity entity, int componentPoolIndex, IComponent component);

		private void OnGroupEntityAdded(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (componentPoolIndex == _componentPoolIndex)
				GroupEntityAddedEvent(entity, componentPoolIndex, component);
		}

		private void OnGroupEntityRemoved(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (componentPoolIndex == _componentPoolIndex)
				GroupEntityRemovedEvent(entity, componentPoolIndex, component);
		}

		private void OnGroupEntityUpdated(Entity entity, int componentPoolIndex, IComponent prevComponent, IComponent newComponent)
		{
			if (componentPoolIndex == _componentPoolIndex)
			{
				GroupEntityRemovedEvent(entity, componentPoolIndex, prevComponent);
				GroupEntityAddedEvent(entity, componentPoolIndex, newComponent);
			}
		}
	}
}