using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityManager
    {
        private readonly DataCache<Entity[], Entity[]> _entities;
        private readonly Queue<Entity> _reuseableEntities;
        private readonly IComponent[][] _componentPools;
        private readonly Dictionary<string, EntityCommandPlayback> _entityCommandPlaybacks;
        private int _nextId;

        internal EntityManager(World world)
        {
            _entities = new DataCache<Entity[], Entity[]>(new Entity[4], UpdateEntitiesCache);
            _reuseableEntities = new Queue<Entity>();
            _componentPools = new IComponent[ComponentIndexes.Instance.Count][];
            for (int i = 0; i < _componentPools.Length; i++)
                _componentPools[i] = new IComponent[4];
            _entityCommandPlaybacks = new Dictionary<string, EntityCommandPlayback>();

            CurrentWorld = world;

            // Create null entity
            CreateEntity();
            _entities.UncachedData[0] = Entity.Null;

            DefaultEntityCommandPlayback = CreateOrGetEntityCommand("Default");
        }

        public World CurrentWorld { get; }
        public EntityCommandPlayback DefaultEntityCommandPlayback { get; }

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
            if (entity.Id <= 0 || entity.Id >= _entities.UncachedData.Length)
                return false;

            return _entities.UncachedData[entity.Id] == entity;
        }

        public bool EntityIsFiltered(Entity entity, Filter filter)
        {
            if (!HasEntity(entity))
                return false;

            return FilteredAllOf(entity, filter) &&
                   FilteredAnyOf(entity, filter) &&
                   FilteredNoneOf(entity, filter);
        }

        public Entity[] GetEntities()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _entities.CachedData;
        }

        public Entity CreateEntity()
        {
            var entity = LocalCreateEntity();

            _entities.UncachedData[entity.Id] = entity;
            _entities.IsDirty = true;

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            var entities = LocalCreateEntities(count);

            foreach (var entity in entities)
                _entities.UncachedData[entity.Id] = entity;
            _entities.IsDirty = true;

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            RemoveAllComponents(entity);

            lock (_reuseableEntities)
            { _reuseableEntities.Enqueue(entity); }
            _entities.UncachedData[entity.Id] = Entity.Null;
            _entities.IsDirty = true;
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            lock (_reuseableEntities)
            {
                foreach (var entity in entities)
                {
                    if (!HasEntity(entity))
                        throw new EntityDoesNotExistException(CurrentWorld, entity);

                    RemoveAllComponents(entity);

                    _reuseableEntities.Enqueue(entity);
                    _entities.UncachedData[entity.Id] = Entity.Null;
                }
            }
            _entities.IsDirty = true;
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

            CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, componentIndex);
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

                CurrentWorld.GroupManager.OnEntityComponentReplaced(entity, componentIndex);
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

            CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, componentIndex);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            for (var i = 0; i < _componentPools.Length; i++)
            {
                var pool = _componentPools[i];
                var component = pool[entity.Id];
                if (component != null)
                {
                    pool[entity.Id] = null;
                    CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, i);
                }
            }
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
            _entities.UncachedData[entity.Id] = entity;
            _entities.IsDirty = true;
        }

        internal void InternalDestroy()
        {
            Array.Clear(_entities.UncachedData, 0, _entities.UncachedData.Length);
            _entities.IsDirty = true;
            _reuseableEntities.Clear();
            foreach (var pool in _componentPools)
                Array.Clear(pool, 0, pool.Length);
            _entityCommandPlaybacks.Clear();
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
                if (_entities.UncachedData.Length == _nextId)
                {
                    Array.Resize(ref _entities.UncachedData, _entities.UncachedData.Length << 1);
                    lock (_componentPools)
                    {
                        for (int i = 0; i < _componentPools.Length; i++)
                            Array.Resize(ref _componentPools[i], _componentPools[i].Length << 1);
                    }
                }
                _entities.UncachedData[entity.Id] = Entity.Null;
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
                    int newSize = 0;
                    if (_reuseableEntities.Count < count)
                    {
                        newSize = count - _reuseableEntities.Count;
                        newSize = (int)Math.Pow(2, (int)Math.Log(_entities.UncachedData.Length + newSize, 2) + 1);
                        Array.Resize(ref _entities.UncachedData, newSize);

                        lock (_componentPools)
                        {
                            for (int i = 0; i < _componentPools.Length; i++)
                                Array.Resize(ref _componentPools[i], newSize);
                        }
                    }
                    else
                        newSize = count;

                    for (int i = 0; i < count; i++)
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
                                Id = _nextId++,
                                Version = 1
                            };
                        }
                        entities[i] = entity;
                    }
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
                return _entities.UncachedData
                    .Where(x => x != Entity.Null)
                    .ToArray();
            }
        }
    }
}