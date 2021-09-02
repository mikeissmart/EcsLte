using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    public class GroupManager
    {
        private readonly List<Group>[] _groupComponentIndexes;
        private readonly Dictionary<Filter, Group> _groups;

        internal GroupManager(World world)
        {
            _groups = new Dictionary<Filter, Group>();
            _groupComponentIndexes = new List<Group>[ComponentIndexes.Instance.Count];

            CurrentWorld = world;

            for (var i = 0; i < ComponentIndexes.Instance.Count; i++)
                _groupComponentIndexes[i] = new List<Group>();
        }

        public World CurrentWorld { get; }
        public Group[] Groups => _groups.Values.ToArray();

        public Group GetGroup(Filter filter)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            Group group;
            lock (_groups)
            {
                if (!_groups.TryGetValue(filter, out group))
                {
                    group = new Group(this, filter);
                    _groups.Add(filter, group);

                    lock (_groupComponentIndexes)
                    {
                        foreach (var index in group.Filter.Indexes)
                            _groupComponentIndexes[index].Add(group);
                    }
                }
                else
                {
                    return group;
                }
            }

            foreach (var entity in CurrentWorld.EntityManager.GetEntities())
                group.FilterEntity(entity, -1);

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

            lock (_groups)
            {
                _groups.Remove(group.Filter);
            }

            foreach (var componentIndex in group.Filter.Indexes)
            {
                var groups = _groupComponentIndexes[componentIndex];
                lock (groups)
                {
                    groups.Remove(group);
                }
            }
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            foreach (var group in _groups.Values)
                group.OnEntityWillBeDestroyed(entity);
        }

        internal void OnEntityComponentAddedOrRemoved(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.FilterEntity(entity, componentPoolIndex);
        }

        internal void OnEntityComponentReplaced(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.UpdateEntity(entity, componentPoolIndex);
        }

        internal void InternalDestroy()
        {
            foreach (var group in _groups.Values)
                group.InternalDestroy();
            _groups.Clear();
        }

        private void OnComponentAddedEvent(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.FilterEntity(entity, componentPoolIndex);
        }

        private void OnComponentRemovedEvent(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.FilterEntity(entity, componentPoolIndex);
        }
    }
}