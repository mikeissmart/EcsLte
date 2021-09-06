using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class GroupManager
    {
        private readonly GroupManagerData _data;

        internal GroupManager(World world)
        {
            _data = ObjectCache.Pop<GroupManagerData>();

            CurrentWorld = world;
        }

        public World CurrentWorld { get; }
        public Group[] Groups => _data.Groups.Values.ToArray();

        public Group GetGroup(Filter filter)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            Group group;
            lock (_data.Groups)
            {
                if (!_data.Groups.TryGetValue(filter, out group))
                {
                    var entities = CurrentWorld.EntityManager.GetEntities()
                        .AsParallel()
                        .Where(x => CurrentWorld.EntityManager.EntityIsFiltered(x, filter))
                        .ToArray();

                    group = new Group();
                    group.Initialize(this, filter, entities);
                    _data.Groups.Add(filter, group);

                    lock (_data.GroupComponentIndexes)
                    {
                        foreach (var index in group.Filter.Indexes)
                            _data.GroupComponentIndexes[index].Add(group);
                    }
                }
                else
                {
                    return group;
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

            lock (_data.Groups)
            {
                _data.Groups.Remove(group.Filter);
            }

            foreach (var componentIndex in group.Filter.Indexes)
            {
                var groups = _data.GroupComponentIndexes[componentIndex];
                lock (groups)
                {
                    groups.Remove(group);
                }
            }
        }

        internal void OnEntityArrayResize(int newSize)
        {
            lock (_data.Groups)
            {
                foreach (var group in _data.Groups.Values)
                    group.OnEntityArrayResize(newSize);
            }
        }

        internal void OnEntityWillBeDestroyed(Entity entity)
        {
            foreach (var group in _data.Groups.Values)
                group.OnEntityWillBeDestroyed(entity);
        }

        internal void OnEntityComponentAddedOrRemoved(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _data.GroupComponentIndexes[componentPoolIndex])
                group.FilterEntity(entity, componentPoolIndex);
        }

        internal void OnEntityComponentReplaced(Entity entity, int componentPoolIndex)
        {
            foreach (var group in _data.GroupComponentIndexes[componentPoolIndex])
                group.UpdateEntity(entity, componentPoolIndex);
        }

        internal void InternalDestroy()
        {
            foreach (var group in _data.Groups.Values)
                group.InternalDestroy();
            _data.Reset();
            ObjectCache.Push(_data);
        }
    }
}