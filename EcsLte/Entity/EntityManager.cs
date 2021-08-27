using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EcsLte.Events;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityManager
    {
        private readonly List<Entity> _entities;
        private readonly DataCache<Entity[]> _entitiesCache;
        private readonly Queue<Entity> _reuseableEntities;
        private readonly List<IComponent>[] _componentPools;
        private readonly Dictionary<string, EntityCommandPlayback> _entityCommandPlaybacks;
        private int _nextId;

        internal EntityManager(World world)
        {
            _entities = new List<Entity>();
            _entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);
            _reuseableEntities = new Queue<Entity>();
            _componentPools = new List<IComponent>[ComponentIndexes.Instance.Count];
            for (int i = 0; i < _componentPools.Length; i++)
                _componentPools[i] = new List<IComponent>();
            _entityCommandPlaybacks = new Dictionary<string, EntityCommandPlayback>();

            CurrentWorld = world;
            AnyEntityCreated = new EntityEvent();
            AnyEntityWillBeDestroyedEvent = new EntityEvent();
            AnyComponentAddedEvent = new EntityComponentChangedEvent();
            AnyComponentRemovedEvent = new EntityComponentChangedEvent();
            AnyComponentReplacedEvent = new EntityComponentReplacedEvent();

            // Create null entity
            CreateEntity();
            _entities[0] = Entity.Null;

            DefaultEntityCommandPlayback = CreateOrGetEntityCommand("Default");
        }

        public World CurrentWorld { get; }
        public EntityCommandPlayback DefaultEntityCommandPlayback { get; }

        internal EntityEvent AnyEntityCreated { get; }
        internal EntityEvent AnyEntityWillBeDestroyedEvent { get; }
        internal EntityComponentChangedEvent AnyComponentAddedEvent { get; }
        internal EntityComponentChangedEvent AnyComponentRemovedEvent { get; }
        internal EntityComponentReplacedEvent AnyComponentReplacedEvent { get; }

        public EntityCommandPlayback CreateOrGetEntityCommand(string name)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (!ParallelRunner.IsMainThread)
                throw new EntityCommandPlaybackOffThreadException(name);

            if (_entityCommandPlaybacks.TryGetValue(name, out EntityCommandPlayback entityCommand))
                return entityCommand;

            entityCommand = new EntityCommandPlayback(CurrentWorld, name);
            _entityCommandPlaybacks.Add(name, entityCommand);

            return entityCommand;
        }

        public bool HasEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                return false;
            if (entity.Id <= 0 || entity.Id >= _entities.Count)
                return false;

            return _entities[entity.Id] == entity;
        }

        public bool EntityIsFiltered(Entity entity, Filter filter)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            return FilteredAllOf(entity, filter) &&
                   FilteredAnyOf(entity, filter) &&
                   FilteredNoneOf(entity, filter);
        }

        public Entity[] GetEntities()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _entitiesCache.Data;
        }

        public Entity CreateEntity()
        {
            var entity = LocalCreateEntity();

            lock (_entities)
            {
                _entities[entity.Id] = entity;
                _entitiesCache.IsDirty = true;
            }

            AnyEntityCreated.Invoke(entity);

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            var entities = LocalCreateEntities(count);

            lock (_entities)
            {
                foreach (var entity in entities)
                    _entities[entity.Id] = entity;
                _entitiesCache.IsDirty = true;
            }

            foreach (var entity in entities)
                AnyEntityCreated.Invoke(entity);

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            RemoveAllComponents(entity);

            AnyEntityWillBeDestroyedEvent.Invoke(entity);

            lock (_reuseableEntities)
            { _reuseableEntities.Enqueue(entity); }
            lock (_entities)
            {
                _entities[entity.Id] = Entity.Null;
                _entitiesCache.IsDirty = true;
            }
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            foreach (var entity in entities)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(CurrentWorld, entity);

                RemoveAllComponents(entity);

                AnyEntityWillBeDestroyedEvent.Invoke(entity);

                lock (_reuseableEntities)
                {
                    lock (_entities)
                    {
                        lock (_reuseableEntities)
                        { _reuseableEntities.Enqueue(entity); }
                        lock (_entities)
                        {
                            _entities[entity.Id] = Entity.Null;
                        }
                    }
                }
            }
            _entitiesCache.IsDirty = true;
        }

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            return _componentPools[ComponentIndex<TComponent>.Index][entity.Id] != null;
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return (TComponent)_componentPools[ComponentIndex<TComponent>.Index][entity.Id];
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            var components = new List<IComponent>();
            foreach (var pool in _componentPools)
            {
                var component = pool[entity.Id];
                if (component != null)
                    components.Add(component);
            }
            return components.ToArray();
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            var componentIndex = ComponentIndex<TComponent>.Index;
            _componentPools[componentIndex][entity.Id] = component;

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
                var componentPool = _componentPools[componentIndex];
                var prevComponent = componentPool[entity.Id];
                componentPool[entity.Id] = newComponent;

                AnyComponentReplacedEvent.Invoke(entity, componentIndex, prevComponent, newComponent);
            }
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var componentIndex = ComponentIndex<TComponent>.Index;
            var componentPool = _componentPools[componentIndex];
            var component = componentPool[entity.Id];
            componentPool[entity.Id] = null;

            AnyComponentRemovedEvent.Invoke(entity, componentIndex, component);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            var invokeQueue = new List<Tuple<Entity, int, IComponent>>();
            for (var i = 0; i < _componentPools.Length; i++)
            {
                var pool = _componentPools[i];
                var component = pool[entity.Id];
                if (component != null)
                {
                    pool[entity.Id] = null;
                    invokeQueue.Add(Tuple.Create(entity, i, component));
                }
            }

            foreach (var queue in invokeQueue)
                AnyComponentRemovedEvent.Invoke(queue.Item1, queue.Item2, queue.Item3);
        }

        internal Entity EnqueueEntityFromCommand()
        {
            return LocalCreateEntity();
        }

        internal Entity[] EnqueueEntitiesFromCommand(int count)
        {
            return LocalCreateEntities(count);
        }

        internal void DequeueEntityFromCommand(Entity entity)
        {
            _entities[entity.Id] = entity;
            _entitiesCache.IsDirty = true;

            AnyEntityCreated.Invoke(entity);
        }

        internal void InternalDestroy()
        {
            _entities.Clear();
            _entitiesCache.IsDirty = true;
            _reuseableEntities.Clear();
            foreach (var pool in _componentPools)
                pool.Clear();
            _entityCommandPlaybacks.Clear();

            AnyEntityCreated.Clear();
            AnyEntityWillBeDestroyedEvent.Clear();
            AnyComponentAddedEvent.Clear();
            AnyComponentRemovedEvent.Clear();
            AnyComponentReplacedEvent.Clear();
        }

        private Entity LocalCreateEntity()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            Entity entity;
            lock (_reuseableEntities)
            {
                if (_reuseableEntities.Count > 0)
                {
                    entity = _reuseableEntities.Dequeue();
                    entity.Version++;

                    return entity;
                }
            }

            lock (_entities) // use lock since _nextId is updating
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
                _entities.Add(Entity.Null);
            }
            lock (_componentPools)
            {
                foreach (var pool in _componentPools)
                    pool.Add(null);
            }

            return entity;
        }

        private Entity[] LocalCreateEntities(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Count must be greater than 0");
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var entities = new Entity[count];
            lock (_reuseableEntities)
            {
                lock (_entities)
                {
                    int expandBy = 0;
                    if (_reuseableEntities.Count < count)
                    {
                        expandBy = count - _reuseableEntities.Count;
                        _entities.AddRange(
                            Enumerable.Repeat<Entity>(Entity.Null, expandBy));

                        lock (_componentPools)
                        {
                            foreach (var pool in _componentPools)
                            {
                                pool.AddRange(
                                   Enumerable.Repeat<IComponent>(null, expandBy));
                            }
                        }
                    }
                    else
                        expandBy = count;

                    ParallelRunner.RunParallelFor(count,
                        index =>
                        {
                            Entity entity;
                            if (_reuseableEntities.Count > 0)
                            {
                                entity = _reuseableEntities.Dequeue();
                                entity.Version++;
                            }
                            else
                            {
                                entity = new Entity
                                {
                                    Id = _nextId + index,
                                    Version = 1
                                };
                            }
                            entities[index] = entity;
                        });
                    _nextId += expandBy;
                }
            }

            return entities;
        }

        private bool FilteredAllOf(Entity entity, Filter filter)
        {
            if (filter.AllOfIndexes == null || filter.AllOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.Indexes)
            {
                if (_componentPools[index][entity.Id] == null)
                    return false;
            }

            return true;
        }

        private bool FilteredAnyOf(Entity entity, Filter filter)
        {
            if (filter.AnyOfIndexes == null || filter.AnyOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.Indexes)
            {
                if (_componentPools[index][entity.Id] != null)
                    return true;
            }

            return false;
        }

        private bool FilteredNoneOf(Entity entity, Filter filter)
        {
            if (filter.NoneOfIndexes == null || filter.NoneOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.Indexes)
            {
                if (_componentPools[index][entity.Id] != null)
                    return false;
            }

            return true;
        }

        private Entity[] UpdateEntitiesCache()
        {
            lock (_entities)
            {
                return _entities
                    .Where(x => x != Entity.Null)
                    .ToArray();
            }
        }
    }
}