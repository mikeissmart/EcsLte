using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Events;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityManager
    {
        private readonly DataCache<Entity[]> _entitiesCache;
        private readonly List<EntityInfo> _entityInfos;
        private readonly Queue<EntityInfo> _reuseableEntityInfos;
        private int _nextId;

        internal EntityManager(World world)
        {
            _entityInfos = new List<EntityInfo>();
            _reuseableEntityInfos = new Queue<EntityInfo>();
            _entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);
            _nextId = 1;

            CurrentWorld = world;
            AnyEntityCreated = new EntityEvent();
            AnyEntityWillBeDestroyedEvent = new EntityEvent();
            AnyComponentAddedEvent = new EntityComponentChangedEvent();
            AnyComponentRemovedEvent = new EntityComponentChangedEvent();
            AnyComponentReplacedEvent = new EntityComponentReplacedEvent();

            _entityInfos.Add(null);
        }

        public World CurrentWorld { get; }

        internal EntityEvent AnyEntityCreated { get; }
        internal EntityEvent AnyEntityWillBeDestroyedEvent { get; }
        internal EntityComponentChangedEvent AnyComponentAddedEvent { get; }
        internal EntityComponentChangedEvent AnyComponentRemovedEvent { get; }
        internal EntityComponentReplacedEvent AnyComponentReplacedEvent { get; }

        public bool HasEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (entity.Id <= 0 && entity.Id >= _entityInfos.Count)
                throw new ArgumentOutOfRangeException();

            var entityInfo = _entityInfos[entity.Id];
            return entityInfo != null && entityInfo.Version == entity.Version;
        }

        public bool EntityIsFiltered(Entity entity, Filter filter)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            return filter.Filtered(_entityInfos[entity.Id]);
        }

        public Entity[] GetEntities()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _entitiesCache.Data;
        }

        public Entity CreateEntity()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            EntityInfo entityInfo = null;
            lock (_entityInfos)
            {
                if (_reuseableEntityInfos.Count > 0)
                {
                    entityInfo = _reuseableEntityInfos.Dequeue();
                    entityInfo.Version++;
                    _entityInfos[entityInfo.Id] = entityInfo;
                }
                else
                {
                    entityInfo = new EntityInfo
                    {
                        Id = _nextId++,
                        Version = 1
                    };
                    _entityInfos.Add(entityInfo);
                }

                _entitiesCache.IsDirty = true;
            }

            var entity = Entity.CreateFromInfo(entityInfo);
            AnyEntityCreated.Invoke(entity);

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Count must be greater than 0");

            var entities = new Entity[count];
            lock (_entityInfos)
            {
                for (var i = 0; i < count; i++)
                {
                    EntityInfo entityInfo = null;
                    if (_reuseableEntityInfos.Count > 0)
                    {
                        entityInfo = _reuseableEntityInfos.Dequeue();
                        entityInfo.Version++;
                        _entityInfos[entityInfo.Id] = entityInfo;
                    }
                    else
                    {
                        entityInfo = new EntityInfo
                        {
                            Id = _nextId++,
                            Version = 1
                        };
                        _entityInfos.Add(entityInfo);
                    }

                    var entity = Entity.CreateFromInfo(entityInfo);
                    AnyEntityCreated.Invoke(entity);
                    entities[i] = entity;
                }

                _entitiesCache.IsDirty = true;
            }

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            RemoveAllComponents(entity);

            AnyEntityWillBeDestroyedEvent.Invoke(entity);
            var entityInfo = _entityInfos[entity.Id];
            lock (entityInfo)
            {
                entityInfo.Reset();
            }

            lock (_entityInfos)
            {
                _reuseableEntityInfos.Enqueue(entityInfo);
                _entityInfos[entity.Id] = null;
                _entitiesCache.IsDirty = true;
            }
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            lock (_entityInfos)
            {
                foreach (var entity in entities)
                {
                    if (!HasEntity(entity))
                        throw new EntityDoesNotExistException(CurrentWorld, entity);

                    RemoveAllComponents(entity);

                    AnyEntityWillBeDestroyedEvent.Invoke(entity);
                    var entityInfo = _entityInfos[entity.Id];
                    lock (entityInfo)
                    {
                        entityInfo.Reset();
                    }

                    _reuseableEntityInfos.Enqueue(entityInfo);
                    _entityInfos[entity.Id] = null;
                }

                _entitiesCache.IsDirty = true;
            }
        }

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            return _entityInfos[entity.Id][ComponentIndex<TComponent>.Index] != null;
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return (TComponent)_entityInfos[entity.Id][ComponentIndex<TComponent>.Index];
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            return _entityInfos[entity.Id].GetComponents();
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            var componentIndex = ComponentIndex<TComponent>.Index;
            var entityInfo = _entityInfos[entity.Id];
            lock (entityInfo)
            {
                entityInfo[componentIndex] = component;
            }

            AnyComponentAddedEvent.Invoke(entity, componentIndex, component);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
            {
                AddComponent(entity, newComponent);
            }
            else
            {
                var componentIndex = ComponentIndex<TComponent>.Index;
                var entityInfo = _entityInfos[entity.Id];
                var prevComponent = entityInfo[componentIndex];
                lock (entityInfo)
                {
                    entityInfo[componentIndex] = newComponent;
                }

                AnyComponentReplacedEvent.Invoke(entity, componentIndex, prevComponent, newComponent);
            }
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var componentIndex = ComponentIndex<TComponent>.Index;
            var entityInfo = _entityInfos[entity.Id];
            var component = entityInfo[componentIndex];
            lock (entityInfo)
            {
                entityInfo[componentIndex] = null;
            }

            AnyComponentRemovedEvent.Invoke(entity, componentIndex, component);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            var entityInfo = _entityInfos[entity.Id];
            lock (entityInfo)
            {
                for (var i = 0; i < ComponentIndexes.Instance.Count; i++)
                {
                    var component = entityInfo[i];
                    entityInfo[i] = null;
                    if (component != null)
                        AnyComponentRemovedEvent.Invoke(entity, i, component);
                }
            }
        }

        public void DestroyAllEntities()
        {
            foreach (var entity in GetEntities())
                DestroyEntity(entity);
        }

        internal void InternalDestroy()
        {
            _entityInfos.Clear();

            AnyEntityCreated.Clear();
            AnyEntityWillBeDestroyedEvent.Clear();
            AnyComponentAddedEvent.Clear();
            AnyComponentRemovedEvent.Clear();
            AnyComponentReplacedEvent.Clear();
        }

        private Entity[] UpdateEntitiesCache()
        {
            return _entityInfos
                .Where(x => x != null)
                .Select(x => Entity.CreateFromInfo(x))
                .ToArray();
        }
    }
}