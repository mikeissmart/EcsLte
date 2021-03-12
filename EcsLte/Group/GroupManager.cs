using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
	public class GroupManager
	{
		private readonly Dictionary<Filter, Group> _groupLookup;
		private readonly List<Group>[] _groupComponentIndexes;
		private readonly World _world;
		private readonly EntityManager _entityManager;

		internal GroupManager(World world, EntityManager entityManager)
		{
			_groupLookup = new Dictionary<Filter, Group>();
			_groupComponentIndexes = new List<Group>[ComponentIndexes.Count];
			_world = world;
			_entityManager = entityManager;

			for (int i = 0; i < ComponentIndexes.Count; i++)
				_groupComponentIndexes[i] = new List<Group>();

			_entityManager.AnyEntityCreated.Subscribe(OnEntityCreated);
			_entityManager.AnyEntityWillBeDestroyedEvent.Subscribe(OnEntityWillBeDestroyed);
			_entityManager.AnyComponentAddedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
			_entityManager.AnyComponentRemovedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
			_entityManager.AnyComponentReplacedEvent.Subscribe(OnEntityComponentReplaced);
		}

		public Group GetGroup(Filter filter)
		{
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);

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
			if (group == null)
				throw new ArgumentNullException();
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);
			if (!_groupLookup.ContainsKey(group.Filter))
				// TODO: throw proper exception
				throw new Exception();

			_groupLookup.Remove(group.Filter);
			foreach (var index in group.Indexes)
				_groupComponentIndexes[index].Remove(group);
		}

		private void OnEntityCreated(Entity entity)
		{
		}

		private void OnEntityWillBeDestroyed(Entity entity)
		{
			// TODO: loop through components and remove them from groups
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