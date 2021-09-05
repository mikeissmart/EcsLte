using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityManager
    {
        private readonly IComponent[][] _componentPools;
        private readonly DataCache<Entity[], Entity[]> _entities;
        private readonly Dictionary<string, EntityCommandPlayback> _entityCommandPlaybacks;
        private readonly Queue<Entity> _reuseableEntities;
        private readonly Entity[] _uniqueEntities;
        private List<int>[] _entityComponentIndexes;
        private int _nextId;

        internal EntityManager(World world)
        {
            _entities = new DataCache<Entity[], Entity[]>(new Entity[4], UpdateEntitiesCache);
            _entityComponentIndexes = new List<int>[4];
            for (var i = 0; i < _entityComponentIndexes.Length; i++)
                _entityComponentIndexes[i] = new List<int>();
            _reuseableEntities = new Queue<Entity>();
            _uniqueEntities = new Entity[ComponentIndexes.Instance.Count];
            _componentPools = new IComponent[ComponentIndexes.Instance.Count][];
            for (var i = 0; i < _componentPools.Length; i++)
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

            if (_entityCommandPlaybacks.TryGetValue(name, out var entityCommand))
                return entityCommand;

            entityCommand = new EntityCommandPlayback(CurrentWorld, name);
            _entityCommandPlaybacks.Add(name, entityCommand);

            return entityCommand;
        }

        public bool EntityIsFiltered(Entity entity, Filter filter)
        {
            if (!HasEntity(entity))
                return false;

            return FilteredAllOf(entity, filter) &&
                   FilteredAnyOf(entity, filter) &&
                   FilteredNoneOf(entity, filter);
        }

        internal void InternalDestroy()
        {
            Array.Clear(_entities.UncachedData, 0, _entities.UncachedData.Length);
            _entities.IsDirty = true;
            _reuseableEntities.Clear();
            foreach (var pool in _componentPools)
                Array.Clear(pool, 0, pool.Length);
            Array.Clear(_uniqueEntities, 0, _uniqueEntities.Length);
            _entityCommandPlaybacks.Clear();
        }

        #region UpdateCache

        private Entity[] UpdateEntitiesCache()
        {
            lock (_entities)
            {
                return _entities.UncachedData
                    .Where(x => x != Entity.Null)
                    .ToArray();
            }
        }

        #endregion

        #region EntityLife

        public bool HasEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (entity.Id <= 0 || entity.Id >= _entities.UncachedData.Length)
                return false;

            return _entities.UncachedData[entity.Id] == entity;
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

            foreach (var entity in entities) _entities.UncachedData[entity.Id] = entity;
            _entities.IsDirty = true;

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            CurrentWorld.GroupManager.OnEntityWillBeDestroyed(entity);
            RemoveAllComponents(entity);

            lock (_reuseableEntities)
            {
                _reuseableEntities.Enqueue(entity);
            }

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

                    CurrentWorld.GroupManager.OnEntityWillBeDestroyed(entity);
                    RemoveAllComponents(entity);

                    _reuseableEntities.Enqueue(entity);
                    _entities.UncachedData[entity.Id] = Entity.Null;
                }
            }

            _entities.IsDirty = true;
        }

        #endregion

        #region UniqueComponent

        public bool HasUniqueComponent<TComponentUnique>()
            where TComponentUnique : IComponentUnique
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _uniqueEntities[ComponentIndex<TComponentUnique>.Index] != Entity.Null;
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>()
            where TComponentUnique : IComponentUnique
        {
            var entity = GetUniqueEntity<TComponentUnique>();
            return GetComponent<TComponentUnique>(entity);
        }

        public Entity GetUniqueEntity<TComponentUnique>()
            where TComponentUnique : IComponentUnique
        {
            if (!HasUniqueComponent<TComponentUnique>())
                throw new EntityNotHaveComponentUniqueException(typeof(TComponentUnique));

            return _uniqueEntities[ComponentIndex<TComponentUnique>.Index];
        }

        public Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique)
            where TComponentUnique : IComponentUnique
        {
            if (HasUniqueComponent<TComponentUnique>())
                throw new EntityAlreadyHasComponentUniqueException(typeof(TComponentUnique));

            var entity = CreateEntity();
            AddComponent(entity, componentUnique);

            return entity;
        }

        public Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique)
            where TComponentUnique : IComponentUnique
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(newComponentUnique);

            var entity = GetUniqueEntity<TComponentUnique>();
            ReplaceComponent(entity, newComponentUnique);

            return entity;
        }

        public Entity RemoveUniqueComponent<TComponentUnique>()
            where TComponentUnique : IComponentUnique
        {
            if (!HasUniqueComponent<TComponentUnique>())
                throw new EntityNotHaveComponentUniqueException(typeof(TComponentUnique));

            var entity = GetUniqueEntity<TComponentUnique>();
            RemoveComponent<TComponentUnique>(entity);
            if (GetAllComponents(entity).Length == 0)
                DestroyEntity(entity);

            return entity;
        }

        #endregion

        #region ComponentLife

        public void AddAllComponents(Entity entity)
        {
            for (var i = 0; i < ComponentIndexes.Instance.Count; i++)
            {
                var type = ComponentIndexes.Instance.AllComponentTypes[i];
                var com = (IComponent)Activator.CreateInstance(type);

                _componentPools[i][entity.Id] = com;
            }
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

            var componentIndexes = _entityComponentIndexes[entity.Id].ToArray();
            var components = new IComponent[componentIndexes.Length];
            for (var i = 0; i < components.Length; i++)
            {
                var index = componentIndexes[i];
                components[i] = _componentPools[index][entity.Id];
            }

            return components;
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            if (ComponentIndex<TComponent>.IsUnique)
            {
                if (_uniqueEntities[componentPoolIndex] != Entity.Null)
                    throw new EntityAlreadyHasComponentUniqueException(typeof(TComponent));
                _uniqueEntities[componentPoolIndex] = entity;
            }

            var entityIndexes = _entityComponentIndexes[entity.Id];
            lock (entityIndexes)
            {
                entityIndexes.Add(componentPoolIndex);
            }

            _componentPools[componentPoolIndex][entity.Id] = component;
            CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, componentPoolIndex);
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
                var componentPoolIndex = ComponentIndex<TComponent>.Index;
                _componentPools[componentPoolIndex][entity.Id] = newComponent;
                CurrentWorld.GroupManager.OnEntityComponentReplaced(entity, componentPoolIndex);
            }
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            if (ComponentIndex<TComponent>.IsUnique && _uniqueEntities[componentPoolIndex] == entity)
                _uniqueEntities[componentPoolIndex] = Entity.Null;

            var entityIndexes = _entityComponentIndexes[entity.Id];
            lock (entityIndexes)
            {
                entityIndexes.Remove(componentPoolIndex);
            }

            _componentPools[componentPoolIndex][entity.Id] = null;
            CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, componentPoolIndex);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            var entityIndexes = _entityComponentIndexes[entity.Id];
            int[] componentIndexes;
            lock (entityIndexes)
            {
                componentIndexes = entityIndexes.ToArray();
                entityIndexes.Clear();
            }

            var components = new IComponent[componentIndexes.Length];
            for (var i = 0; i < components.Length; i++)
            {
                var index = componentIndexes[i];

                if (ComponentIndexes.Instance.UniqueComponentIndexes.Any(x => x == index) &&
                    _uniqueEntities[index] == entity)
                    _uniqueEntities[index] = Entity.Null;

                _componentPools[index][entity.Id] = null;
                CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, index);
            }
        }

        #endregion

        #region EntityPlaybackLife

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

        #endregion

        #region LocalEntityLife

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

                    var oldSize = _entityComponentIndexes.Length;
                    Array.Resize(ref _entityComponentIndexes, _entityComponentIndexes.Length << 1);
                    for (var i = oldSize; i < _entityComponentIndexes.Length; i++)
                        _entityComponentIndexes[i] = new List<int>();

                    lock (_componentPools)
                    {
                        for (var i = 0; i < _componentPools.Length; i++)
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
                    var newSize = 0;
                    if (_reuseableEntities.Count < count)
                    {
                        newSize = count - _reuseableEntities.Count;
                        newSize = (int)Math.Pow(2, (int)Math.Log(_entities.UncachedData.Length + newSize, 2) + 1);
                        Array.Resize(ref _entities.UncachedData, newSize);

                        var oldSize = _entityComponentIndexes.Length;
                        Array.Resize(ref _entityComponentIndexes, newSize);
                        for (var i = oldSize; i < _entityComponentIndexes.Length; i++)
                            _entityComponentIndexes[i] = new List<int>();

                        lock (_componentPools)
                        {
                            for (var i = 0; i < _componentPools.Length; i++)
                                Array.Resize(ref _componentPools[i], newSize);
                        }
                    }
                    else
                    {
                        newSize = count;
                    }

                    for (var i = 0; i < count; i++)
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

        #endregion

        #region FilterEntity

        private bool FilteredAllOf(Entity entity, Filter filter)
        {
            if (filter.AllOfIndexes == null || filter.AllOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.AllOfIndexes)
                if (_componentPools[index][entity.Id] == null)
                    return false;

            return true;
        }

        private bool FilteredAnyOf(Entity entity, Filter filter)
        {
            if (filter.AnyOfIndexes == null || filter.AnyOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.AnyOfIndexes)
                if (_componentPools[index][entity.Id] != null)
                    return true;

            return false;
        }

        private bool FilteredNoneOf(Entity entity, Filter filter)
        {
            if (filter.NoneOfIndexes == null || filter.NoneOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.NoneOfIndexes)
                if (_componentPools[index][entity.Id] != null)
                    return false;

            return true;
        }

        #endregion
    }
}