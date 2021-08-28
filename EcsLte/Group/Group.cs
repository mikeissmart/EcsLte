using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class Group
    {
        private readonly Dictionary<int, Entity> _entities;
        /*private readonly Dictionary<int, Entity> _newlyAddedEntities;
        private readonly Dictionary<int, Entity> _newlyRemoveEntities;
        private readonly Dictionary<int, Entity> _newlyUpdatedEntities;*/
        private readonly DataCache<Entity[]> _entitiesCache;
        /*private readonly DataCache<Entity[]> _newlyAddedEntitiesCache;
        private readonly DataCache<Entity[]> _newlyRemovedEntitiesCache;
        private readonly DataCache<Entity[]> _newlyUpdatedEntitiesCache;*/

        internal Group(GroupManager groupManager, Filter filter)
        {
            _entities = new Dictionary<int, Entity>();
            /*_newlyAddedEntities = new Dictionary<int, Entity>();
            _newlyRemoveEntities = new Dictionary<int, Entity>();
            _newlyUpdatedEntities = new Dictionary<int, Entity>();*/
            _entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);
            /*_newlyAddedEntitiesCache = new DataCache<Entity[]>(UpdateAddedEntitiesCache);
            _newlyRemovedEntitiesCache = new DataCache<Entity[]>(UpdateRemovedEntitiesCache);
            _newlyUpdatedEntitiesCache = new DataCache<Entity[]>(UpdateUpdatedEntitiesCache);*/

            CurrentWorld = groupManager.CurrentWorld;
            Filter = filter;
        }

        public World CurrentWorld { get; }
        public Filter Filter { get; }
        public Entity[] Entities => _entitiesCache.CachedData;
        /*public Entity[] NewlyAddedEntities => _newlyAddedEntitiesCache.CachedData;
        public Entity[] NewlyRemovedEntities => _newlyRemovedEntitiesCache.CachedData;
        public Entity[] NewlyUpdatedEntities => _newlyUpdatedEntitiesCache.CachedData;*/
        public bool IsDestroyed { get; internal set; }

        public bool ContainsEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);

            lock (_entities)
            { return _entities.ContainsKey(entity.Id); }
        }

        public bool Equals(Group other)
        {
            if (other == null)
                return false;
            return Filter == other.Filter && CurrentWorld == other.CurrentWorld;
        }

        public override string ToString()
        {
            return Filter.ToString();
        }

        internal void FilterEntity(Entity entity)
        {
            if (CurrentWorld.EntityManager.EntityIsFiltered(entity, Filter))
            {
                lock (_entities)
                {
                    if (!_entities.ContainsKey(entity.Id))
                    {
                        _entities.Add(entity.Id, entity);
                        _entitiesCache.IsDirty = true;
                    }
                }
                /*lock (_newlyAddedEntities)
                {
                    if (!_newlyAddedEntities.ContainsKey(entity.Id))
                    {
                        _newlyAddedEntities.Add(entity.Id, entity);
                        _newlyAddedEntitiesCache.IsDirty = true;
                    }
                }*/
            }
            else if (ContainsEntity(entity))
            {
                lock (_entities)
                {
                    _entities.Remove(entity.Id);
                    _entitiesCache.IsDirty = true;
                }
                /*lock (_newlyRemoveEntities)
                {
                    if (!_newlyRemoveEntities.ContainsKey(entity.Id))
                    {
                        _newlyRemoveEntities.Add(entity.Id, entity);
                        _newlyRemovedEntitiesCache.IsDirty = true;
                    }
                }*/
            }
        }

        internal void UpdateEntity(Entity entity)
        {
            if (!ContainsEntity(entity))
            {
                FilterEntity(entity);
                return;
            }
            /*lock (_newlyUpdatedEntities)
            {
                if (_newlyUpdatedEntities.ContainsKey(entity.Id))
                {
                    _newlyUpdatedEntities.Add(entity.Id, entity);
                    _newlyUpdatedEntitiesCache.IsDirty = true;
                }
            }*/
        }

        internal void InternalDestroy()
        {
            _entities.Clear();
            /*_newlyAddedEntities.Clear();
            _newlyRemoveEntities.Clear();
            _newlyUpdatedEntities.Clear();*/
            _entitiesCache.IsDirty = true;
            /*_newlyAddedEntitiesCache.IsDirty = true;
            _newlyRemovedEntitiesCache.IsDirty = true;
            _newlyUpdatedEntitiesCache.IsDirty = true;*/

            IsDestroyed = true;
        }

        private Entity[] UpdateEntitiesCache()
        {
            lock (_entities)
            { return _entities.Values.ToArray(); }
        }

        /*private Entity[] UpdateAddedEntitiesCache()
        {
            lock (_newlyAddedEntities)
            { return _newlyAddedEntities.Values.ToArray(); }
        }

        private Entity[] UpdateRemovedEntitiesCache()
        {
            lock (_newlyRemoveEntities)
            { return _newlyRemoveEntities.Values.ToArray(); }
        }

        private Entity[] UpdateUpdatedEntitiesCache()
        {
            lock (_newlyUpdatedEntities)
            { return _newlyUpdatedEntities.Values.ToArray(); }
        }*/
    }
}