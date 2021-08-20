using System.Collections.Generic;
using EcsLte.Events;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class Group
    {
        private readonly HashSet<Entity> _entities;
        private readonly DataCache<Entity[]> _entitiesCache;
        private readonly EntityManager _entityManager;

        internal Group(GroupManager groupManager, Filter filter)
        {
            _entities = new HashSet<Entity>();
            _entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);
            _entityManager = groupManager.CurrentWorld.EntityManager;

            CurrentWorld = groupManager.CurrentWorld;
            Filter = filter;

            EntityAddedEvent = new GroupChangedEvent();
            EntityRemovedEvent = new GroupChangedEvent();
            EntityUpdatedEvent = new GroupUpdatedEvent();
        }

        public World CurrentWorld { get; }
        public Filter Filter { get; }
        public Entity[] Entities => _entitiesCache.Data;
        public bool IsDestroyed { get; internal set; }

        internal GroupChangedEvent EntityAddedEvent { get; }
        internal GroupChangedEvent EntityRemovedEvent { get; }
        internal GroupUpdatedEvent EntityUpdatedEvent { get; }

        public bool ContainsEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);

            return _entities.Contains(entity);
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

        internal void FilterEntitySilent(Entity entity)
        {
            if (_entityManager.EntityIsFiltered(entity, Filter))
                lock (_entities)
                {
                    if (_entities.Add(entity))
                        _entitiesCache.IsDirty = true;
                }
            else if (_entities.Contains(entity))
                lock (_entities)
                {
                    if (_entities.Remove(entity))
                        _entitiesCache.IsDirty = true;
                }
        }

        internal void UpdateEntity(Entity entity, int componentPoolIndex, IComponent prevComponent,
            IComponent newComponent)
        {
            if (_entities.Contains(entity))
                EntityUpdatedEvent.Invoke(entity, componentPoolIndex, prevComponent, newComponent);
        }

        internal void FilterEntity(Entity entity, int componentPoolIndex, IComponent component)
        {
            if (IsDestroyed)
                throw new GroupIsDestroyedException(this);

            if (_entityManager.EntityIsFiltered(entity, Filter))
                lock (_entities)
                {
                    if (_entities.Add(entity))
                    {
                        _entitiesCache.IsDirty = true;
                        EntityAddedEvent.Invoke(entity, componentPoolIndex, component);
                    }
                }
            else if (_entities.Contains(entity))
                lock (_entities)
                {
                    if (_entities.Remove(entity))
                    {
                        _entitiesCache.IsDirty = true;
                        EntityRemovedEvent.Invoke(entity, componentPoolIndex, component);
                    }
                }
        }

        internal void InternalDestroy()
        {
            _entities.Clear();
            _entitiesCache.IsDirty = true;

            IsDestroyed = true;
        }

        private Entity[] UpdateEntitiesCache()
        {
            var entites = new Entity[_entities.Count];
            _entities.CopyTo(entites);

            return entites;
        }
    }
}