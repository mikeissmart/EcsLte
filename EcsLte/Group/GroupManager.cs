using System;
using System.Collections.Generic;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class GroupManager
    {
        private readonly List<Group>[] _groupComponentIndexes;
        private readonly Dictionary<Filter, Group> _groupLookup;

        internal GroupManager(World world)
        {
            _groupLookup = new Dictionary<Filter, Group>();
            _groupComponentIndexes = new List<Group>[ComponentIndexes.Instance.Count];

            CurrentWorld = world;

            for (var i = 0; i < ComponentIndexes.Instance.Count; i++)
                _groupComponentIndexes[i] = new List<Group>();
        }

        public World CurrentWorld { get; }

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

                    /*ParallelRunner.RunParallelForEach(CurrentWorld.EntityManager.GetEntities(),
                        entity => group.FilterEntity(entity));*/

                    foreach (var entity in CurrentWorld.EntityManager.GetEntities())
                        group.FilterEntity(entity);
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
        }

        internal void OnEntityComponentAddedOrRemoved(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.FilterEntity(entity);
        }

        internal void OnEntityComponentReplaced(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _groupComponentIndexes[componentPoolIndex])
                group.UpdateEntity(entity);
        }

        internal void InternalDestroy()
        {
            foreach (var group in _groupLookup.Values)
                group.InternalDestroy();
            _groupLookup.Clear();
        }
    }
}