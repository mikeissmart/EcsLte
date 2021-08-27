using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EcsLte.Events;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class GroupManager
    {
        private readonly EntityManager _entityManager;
        private readonly List<Group>[] _groupComponentIndexes;
        private readonly Dictionary<Filter, Group> _groupLookup;

        internal GroupManager(EntityManager entityManager)
        {
            _groupLookup = new Dictionary<Filter, Group>();
            _groupComponentIndexes = new List<Group>[ComponentIndexes.Instance.Count];
            _entityManager = entityManager;

            CurrentWorld = _entityManager.CurrentWorld;

            for (var i = 0; i < ComponentIndexes.Instance.Count; i++)
                _groupComponentIndexes[i] = new List<Group>();

            _entityManager.AnyComponentAddedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
            _entityManager.AnyComponentRemovedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
            _entityManager.AnyComponentReplacedEvent.Subscribe(OnEntityComponentReplaced);
        }

        public World CurrentWorld { get; }

        internal GroupEvent AnyGroupDestroyed { get; private set; }

        public Group GetGroup(Filter filter)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            Group group;
            lock (_groupLookup)
            {
                if (!_groupLookup.TryGetValue(filter, out group))
                {
                    group = new Group(this, filter);
                    _groupLookup.Add(filter, group);

                    lock (_groupComponentIndexes)
                    {
                        foreach (var index in group.Filter.Indexes)
                            _groupComponentIndexes[index].Add(group);
                    }

                    ParallelRunner.RunParallelForEach(_entityManager.GetEntities(),
                        entity => group.FilterEntitySilent(entity));

                    /*foreach (var entity in _entityManager.GetEntities())
                        group.FilterEntitySilent(entity);*/
                }
            }

            return group;
        }

        public void RemoveGroup(Group group)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (group == null)
                throw new ArgumentNullException();
            if (group.IsDestroyed)
                throw new GroupIsDestroyedException(group);
            if (group.CurrentWorld != CurrentWorld)
                throw new GroupDoesNotExistException(CurrentWorld, group);

            group.InternalDestroy();

            lock (_groupLookup)
            {
                _groupLookup.Remove(group.Filter);
            }

            foreach (var componentIndex in group.Filter.Indexes)
            {
                var groups = _groupComponentIndexes[componentIndex];
                lock (groups)
                {
                    groups.Remove(group);
                }
            }

            AnyGroupDestroyed.Invoke(group);
        }

        internal void InternalDestroy()
        {
            _entityManager.AnyComponentAddedEvent.Unsubscribe(OnEntityComponentAddedOrRemoved);
            _entityManager.AnyComponentRemovedEvent.Unsubscribe(OnEntityComponentAddedOrRemoved);
            _entityManager.AnyComponentReplacedEvent.Unsubscribe(OnEntityComponentReplaced);

            foreach (var group in _groupLookup.Values)
                group.InternalDestroy();
            _groupLookup.Clear();
        }

        private void OnEntityComponentAddedOrRemoved(Entity entity, int componentPoolIndex, IComponent component)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.FilterEntity(entity, componentPoolIndex, component);
        }

        private void OnEntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent prevComponent,
            IComponent newComponent)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.UpdateEntity(entity, componentPoolIndex, prevComponent, newComponent);
        }
    }
}