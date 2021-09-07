using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityManager
    {
        private EntityManagerData _data;

        internal EntityManager(World world)
        {
            _data = ObjectCache.Pop<EntityManagerData>();

            CurrentWorld = world;

            // Create null entity
            CreateEntity();
            _data.Entities.UncachedData[0] = Entity.Null;

            DefaultEntityCommandPlayback = CreateOrGetEntityCommand("Default");
        }

        public World CurrentWorld { get; }
        public EntityCommandPlayback DefaultEntityCommandPlayback { get; }

        internal int EntityArrayLength { get => _data.Entities.UncachedData.Length; }

        public EntityCommandPlayback CreateOrGetEntityCommand(string name)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);
            if (!ParallelRunner.IsMainThread)
                throw new EntityCommandPlaybackOffThreadException(name);

            if (_data.EntityCommandPlaybacks.TryGetValue(name, out var entityCommand))
                return entityCommand;

            entityCommand = new EntityCommandPlayback(CurrentWorld, name);
            _data.EntityCommandPlaybacks.Add(name, entityCommand);

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
            _data.Reset();
            ObjectCache.Push(_data);
        }

        #region UpdateCache

        private Entity[] UpdateEntitiesCache()
        {
            lock (_data.Entities)
            {
                return _data.Entities.UncachedData
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
            if (entity.Id <= 0 || entity.Id >= _data.Entities.UncachedData.Length)
                return false;

            return _data.Entities.UncachedData[entity.Id] == entity;
        }

        public Entity[] GetEntities()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _data.Entities.CachedData;
        }

        public Entity CreateEntity()
        {
            var entity = LocalCreateEntity();

            _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            var entities = LocalCreateEntities(count);

            foreach (var entity in entities) _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;

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

            lock (_data.ReuseableEntities)
            {
                _data.ReuseableEntities.Enqueue(entity);
            }

            _data.Entities.UncachedData[entity.Id] = Entity.Null;
            _data.Entities.IsDirty = true;
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            lock (_data.ReuseableEntities)
            {
                foreach (var entity in entities)
                {
                    if (!HasEntity(entity))
                        throw new EntityDoesNotExistException(CurrentWorld, entity);

                    CurrentWorld.GroupManager.OnEntityWillBeDestroyed(entity);
                    RemoveAllComponents(entity);

                    _data.ReuseableEntities.Enqueue(entity);
                    _data.Entities.UncachedData[entity.Id] = Entity.Null;
                }
            }

            _data.Entities.IsDirty = true;
        }

        #endregion

        #region UniqueComponent

        public bool HasUniqueComponent<TComponentUnique>()
            where TComponentUnique : IComponentUnique
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _data.UniqueEntities[ComponentIndex<TComponentUnique>.Index] != Entity.Null;
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

            return _data.UniqueEntities[ComponentIndex<TComponentUnique>.Index];
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

                _data.ComponentPools[i].AddComponent(entity.Id, com);
            }
        }

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            return _data.ComponentPools[ComponentIndex<TComponent>.Index].HasComponent(entity.Id);
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return (TComponent)_data.ComponentPools[ComponentIndex<TComponent>.Index].GetComponent(entity.Id);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            var componentIndexes = _data.EntityComponentIndexes[entity.Id].ToArray();
            var components = new IComponent[componentIndexes.Length];
            for (var i = 0; i < components.Length; i++)
            {
                var index = componentIndexes[i];
                components[i] = _data.ComponentPools[index].GetComponent(entity.Id);
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
                if (_data.UniqueEntities[componentPoolIndex] != Entity.Null)
                    throw new EntityAlreadyHasComponentUniqueException(typeof(TComponent));
                _data.UniqueEntities[componentPoolIndex] = entity;
            }

            var entityIndexes = _data.EntityComponentIndexes[entity.Id];
            lock (entityIndexes)
            {
                entityIndexes.Add(componentPoolIndex);
            }

            _data.ComponentPools[componentPoolIndex].AddComponent(entity.Id, component);
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
                _data.ComponentPools[componentPoolIndex].ReplaceComponent(entity.Id, newComponent);
                CurrentWorld.GroupManager.OnEntityComponentReplaced(entity, componentPoolIndex);
            }
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            if (ComponentIndex<TComponent>.IsUnique && _data.UniqueEntities[componentPoolIndex] == entity)
                _data.UniqueEntities[componentPoolIndex] = Entity.Null;

            var entityIndexes = _data.EntityComponentIndexes[entity.Id];
            lock (entityIndexes)
            {
                entityIndexes.Remove(componentPoolIndex);
            }

            _data.ComponentPools[componentPoolIndex].RemoveComponent(entity.Id);
            CurrentWorld.GroupManager.OnEntityComponentAddedOrRemoved(entity, componentPoolIndex);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(CurrentWorld, entity);

            var entityIndexes = _data.EntityComponentIndexes[entity.Id];
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
                    _data.UniqueEntities[index] == entity)
                    _data.UniqueEntities[index] = Entity.Null;

                _data.ComponentPools[index].RemoveComponent(entity.Id);
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
            _data.Entities.UncachedData[entity.Id] = entity;
            _data.Entities.IsDirty = true;
        }

        #endregion

        #region LocalEntityLife

        private Entity LocalCreateEntity()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            Entity entity;
            lock (_data.ReuseableEntities)
            {
                if (_data.ReuseableEntities.Count > 0)
                {
                    entity = _data.ReuseableEntities.Dequeue();
                    entity.Version++;

                    return entity;
                }
            }

            lock (_data.Entities)
            {
                entity = new Entity
                {
                    Id = _data.NextId++,
                    Version = 1
                };
                if (_data.Entities.UncachedData.Length == _data.NextId)
                {
                    int newSize = _data.Entities.UncachedData.Length * 2;
                    Array.Resize(ref _data.Entities.UncachedData, newSize);

                    var oldSize = _data.EntityComponentIndexes.Length;
                    Array.Resize(ref _data.EntityComponentIndexes, newSize);
                    for (var i = oldSize; i < _data.EntityComponentIndexes.Length; i++)
                        _data.EntityComponentIndexes[i] = new List<int>();

                    for (var i = 0; i < _data.ComponentPools.Length; i++)
                        _data.ComponentPools[i].Resize(newSize);

                    CurrentWorld.GroupManager.OnEntityArrayResize(newSize);
                }
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
            lock (_data.ReuseableEntities)
            {
                lock (_data.Entities)
                {
                    int newSize = 0;
                    var activeEntityCount = _data.Entities.UncachedData.Length - (_data.ReuseableEntities.Count - _data.NextId);
                    if (activeEntityCount < count)
                    {
                        newSize = count - activeEntityCount;
                        newSize = (int)Math.Pow(2, (int)Math.Log(_data.Entities.UncachedData.Length + newSize, 2) + 1);
                        Array.Resize(ref _data.Entities.UncachedData, newSize);

                        var oldSize = _data.EntityComponentIndexes.Length;
                        Array.Resize(ref _data.EntityComponentIndexes, newSize);
                        for (var i = oldSize; i < _data.EntityComponentIndexes.Length; i++)
                            _data.EntityComponentIndexes[i] = new List<int>();

                        for (var i = 0; i < _data.ComponentPools.Length; i++)
                            _data.ComponentPools[i].Resize(newSize);

                        CurrentWorld.GroupManager.OnEntityArrayResize(newSize);
                    }
                    else
                    {
                        newSize = count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        Entity entity;
                        if (_data.ReuseableEntities.Count > 0)
                        {
                            entity = _data.ReuseableEntities.Dequeue();
                            entity.Version++;
                        }
                        else
                        {
                            entity = new Entity
                            {
                                Id = _data.NextId++,
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
                if (!_data.ComponentPools[index].HasComponent(entity.Id))
                    return false;

            return true;
        }

        private bool FilteredAnyOf(Entity entity, Filter filter)
        {
            if (filter.AnyOfIndexes == null || filter.AnyOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.AnyOfIndexes)
                if (_data.ComponentPools[index].HasComponent(entity.Id))
                    return true;

            return false;
        }

        private bool FilteredNoneOf(Entity entity, Filter filter)
        {
            if (filter.NoneOfIndexes == null || filter.NoneOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.NoneOfIndexes)
                if (_data.ComponentPools[index].HasComponent(entity.Id))
                    return false;

            return true;
        }

        #endregion
    }
}