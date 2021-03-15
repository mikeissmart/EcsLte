using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
	public class GroupManager
	{
		private readonly Dictionary<Filter, Group> _groupLookup;
		private readonly List<Group>[] _groupComponentIndexes;
		private readonly EntityManager _entityManager;

		internal GroupManager(World world, EntityManager entityManager)
		{
			_groupLookup = new Dictionary<Filter, Group>();
			_groupComponentIndexes = new List<Group>[ComponentIndexes.Count];
			_entityManager = entityManager;

			World = world;
			AnyGroupDestroyed = new GroupEvent();

			for (int i = 0; i < ComponentIndexes.Count; i++)
				_groupComponentIndexes[i] = new List<Group>();

			_entityManager.AnyEntityCreated.Subscribe(OnEntityCreated);
			_entityManager.AnyEntityWillBeDestroyedEvent.Subscribe(OnEntityWillBeDestroyed);
			_entityManager.AnyComponentAddedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
			_entityManager.AnyComponentRemovedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
			_entityManager.AnyComponentReplacedEvent.Subscribe(OnEntityComponentReplaced);
		}

		public World World { get; private set; }

		internal GroupEvent AnyGroupDestroyed { get; private set; }

		public Group GetGroup(Filter filter)
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);

			if (!_groupLookup.TryGetValue(filter, out Group group))
			{
				group = new Group(this, filter);
				_groupLookup.Add(filter, group);

				foreach (var index in group.Indexes)
					_groupComponentIndexes[index].Add(group);

				foreach (var entity in _entityManager.GetEntities())
					group.FilterEntitySilent(entity);
			}

			return group;
		}

		public void RemoveGroup(Group group)
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);
			if (group == null)
				throw new ArgumentNullException();
			if (group.IsDestroyed)
				throw new GroupIsDestroyedException(group);
			if (group.GroupManager != this)
				throw new WorldDoesNotHaveGroupException(World, group);

			_groupLookup.Remove(group.Filter);
			foreach (var index in group.Indexes)
				_groupComponentIndexes[index].Remove(group);

			AnyGroupDestroyed.Invoke(group);
		}

		private void OnEntityCreated(Entity entity)
		{
		}

		private void OnEntityWillBeDestroyed(Entity entity)
		{
			// TODO: loop through components and remove them from groups?
		}

		private void OnEntityComponentAddedOrRemoved(Entity entity, int componentPoolIndex, IComponent component)
		{
			foreach (var group in _groupComponentIndexes[componentPoolIndex])
				group.FilterEntity(entity, componentPoolIndex, component);
		}

		private void OnEntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent prevComponent, IComponent newComponent)
		{
			foreach (var group in _groupComponentIndexes[componentPoolIndex])
				group.UpdateEntity(entity, componentPoolIndex, prevComponent, newComponent);
		}
	}
}