using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public partial class EcsContext :
        IFilterBy, IGetEntity, IEntityLife, IGetComponent, IComponentLife, IWithKey
    {
        private EntityManager _entityManager;

        internal EcsContext(string name)
        {
            _entityManager = ObjectCache.Pop<EntityManager>();
            _entityManager.Initialize(this);

            DefaultCommand = CommandQueue("Default");

            Name = name;
        }

        #region EcsContext

        public string Name { get; set; }
        public bool IsDestroyed { get; set; }
        public EntityCommandQueue DefaultCommand { get; set; }

        public Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique)
            where TComponentUnique : IComponentUnique
        {
            if (HasUniqueComponent<TComponentUnique>())
                throw new EntityAlreadyHasComponentUniqueException(this, typeof(TComponentUnique));

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
                throw new EntityNotHaveComponentUniqueException(this, typeof(TComponentUnique));

            var entity = GetUniqueEntity<TComponentUnique>();
            RemoveComponent<TComponentUnique>(entity);
            if (GetAllComponents(entity).Length == 0)
                DestroyEntity(entity);

            return entity;
        }

        public EntityCommandQueue CommandQueue(string name)
        {
            if (this.IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            if (!_entityManager.EntityCommandQueue.TryGetValue(name, out var commandQueue))
            {
                commandQueue = new EntityCommandQueue(this, name);
                _entityManager.EntityCommandQueue.Add(name, commandQueue);
            }

            return commandQueue;
        }

        internal EntityCollection CreateEntityCollection()
        {
            return _entityManager.CreateEntityCollection();
        }

        internal void RemoveEntityCollection(EntityCollection collection)
        {
            _entityManager.RemoveEntityCollection(collection);
        }

        internal void InternalDestroy()
        {
            _entityManager.Reset();
            ObjectCache.Push(_entityManager);

            IsDestroyed = true;
        }

        #endregion

        #region FilterEntity

        public EntityFilter FilterBy(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            EntityFilter entityFilter;
            lock (_entityManager.Filters)
            {
                if (!_entityManager.Filters.TryGetValue(filter, out entityFilter))
                {
                    var entities = GetEntities()
                        .AsParallel()
                        .Where(x => InternalFilteredBy(x, filter))
                        .ToArray();

                    entityFilter = new EntityFilter(this, _entityManager, filter, entities);
                    _entityManager.Filters.Add(filter, entityFilter);

                    lock (_entityManager.FilterComponentIndexes)
                    {
                        for (int i = 0; i < filter.Indexes.Length; i++)
                            _entityManager.FilterComponentIndexes[filter.Indexes[i]].Add(entityFilter);
                    }
                }
            }

            return entityFilter;
        }

        internal bool InternalFilteredBy(Entity entity, Filter filter)
        {
            if (!HasEntity(entity))
                return false;

            var componentIndexes = _entityManager.EntityComponentIndexes[entity.Id]
                .ToArray();
            return _entityManager.FilteredAllOf(componentIndexes, filter) &&
                   _entityManager.FilteredAnyOf(componentIndexes, filter) &&
                   _entityManager.FilteredNoneOf(componentIndexes, filter);
        }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entity.Id <= 0 || entity.Id >= _entityManager.Entities.Length)
                return false;

            return _entityManager.Entities[entity.Id] == entity;
        }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _entityManager.Entities.GetEntities();
        }

        #endregion

        #region EntityLife

        public Entity CreateEntity()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var entity = _entityManager.PrepCreateEntity();

            _entityManager.Entities[entity.Id] = entity;

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var entities = _entityManager.PrepCreateEntities(count);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                _entityManager.Entities[entity.Id] = entity;
            }

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            _entityManager.AnyEntityWillBeDestroyedEvents.Invoke(entity);
            _entityManager.EntityWillBeDestroyedEvents[entity.Id].Invoke(entity);
            _entityManager.SilentRemoveAllComponents(entity);

            lock (_entityManager.ReuseableEntities)
            {
                _entityManager.ReuseableEntities.Enqueue(entity);
            }

            _entityManager.Entities[entity.Id] = Entity.Null;
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (_entityManager.ReuseableEntities)
            {
                foreach (var entity in entities)
                {
                    if (!HasEntity(entity))
                        throw new EntityDoesNotExistException(this, entity);

                    _entityManager.AnyEntityWillBeDestroyedEvents.Invoke(entity);
                    _entityManager.EntityWillBeDestroyedEvents[entity.Id].Invoke(entity);
                    _entityManager.SilentRemoveAllComponents(entity);

                    _entityManager.ReuseableEntities.Enqueue(entity);
                    _entityManager.Entities[entity.Id] = Entity.Null;
                }
            }
        }

        internal Entity EnqueueEntityFromCommand()
        {
            return _entityManager.PrepCreateEntity();
        }

        internal Entity[] EnqueueEntitiesFromCommand(int count)
        {
            return _entityManager.PrepCreateEntities(count);
        }

        internal void DequeueEntityFromCommand(Entity entity)
        {
            _entityManager.Entities[entity.Id] = entity;
        }

        #endregion

        #region GetComponent

        public bool HasUniqueComponent<TComponentUnique>()
            where TComponentUnique : IComponentUnique
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _entityManager.UniqueEntities[ComponentIndex<TComponentUnique>.Index] != Entity.Null;
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
                throw new EntityNotHaveComponentUniqueException(this, typeof(TComponentUnique));

            return _entityManager.UniqueEntities[ComponentIndex<TComponentUnique>.Index];
        }

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            return _entityManager.ComponentPools[ComponentIndex<TComponent>.Index]
                .HasComponent(entity.Id);
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(this, entity, typeof(TComponent));

            return (TComponent)_entityManager.ComponentPools[ComponentIndex<TComponent>.Index]
                .GetComponent(entity.Id);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            var componentIndexes = _entityManager.EntityComponentIndexes[entity.Id].ToArray();
            var components = new IComponent[componentIndexes.Length];
            for (var i = 0; i < components.Length; i++)
            {
                var index = componentIndexes[i];
                components[i] = _entityManager.ComponentPools[index]
                    .GetComponent(entity.Id);
            }

            return components;
        }

        #endregion

        #region ComponentLife

        public void AddUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique componentUnique)
            where TComponentUnique : IComponentUnique
        {
            AddComponent(entity, componentUnique);
        }

        public void ReplaceUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique newComponentUnique)
            where TComponentUnique : IComponentUnique
        {
            ReplaceComponent(entity, newComponentUnique);
        }

        public void RemoveUniqueComponent<TComponentUnique>(Entity entity)
            where TComponentUnique : IComponentUnique
        {
            RemoveComponent<TComponentUnique>(entity);
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(this, entity, typeof(TComponent));

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            if (ComponentIndex<TComponent>.IsUnique)
            {
                if (_entityManager.UniqueEntities[componentPoolIndex] != Entity.Null)
                    throw new EntityAlreadyHasComponentUniqueException(this, typeof(TComponent));
                _entityManager.UniqueEntities[componentPoolIndex] = entity;
            }
            if (ComponentIndex<TComponent>.IsPrimaryKey)
            {
                if (_entityManager.PrimaryKeyes[componentPoolIndex]
                    .GetEntity((IComponentPrimaryKey)component) != Entity.Null)
                    throw new PrimaryKeyAlreadyHasException(this, (IComponentPrimaryKey)component);
            }

            var entityIndexes = _entityManager.EntityComponentIndexes[entity.Id];
            lock (entityIndexes)
            {
                entityIndexes.Add(componentPoolIndex);
            }

            _entityManager.ComponentPools[componentPoolIndex].AddComponent(entity.Id, component);

            _entityManager.AnyEntityComponentAddedEvents.Invoke(entity, componentPoolIndex, component);
            _entityManager.EntityComponentAddedEvents[entity.Id].Invoke(entity, componentPoolIndex, component);
            _entityManager.ComponentPoolEntityComponentAddedEvents[componentPoolIndex]
                .Invoke(entity, componentPoolIndex, component);
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
                var oldComponent = _entityManager.ComponentPools[componentPoolIndex].GetComponent(entity.Id);
                _entityManager.ComponentPools[componentPoolIndex].ReplaceComponent(entity.Id, newComponent);

                _entityManager.AnyEntityComponentReplacedEvents.Invoke(entity, componentPoolIndex, oldComponent, newComponent);
                _entityManager.EntityComponentReplacedEvents[entity.Id].Invoke(entity, componentPoolIndex, oldComponent, newComponent);
                _entityManager.ComponentPoolEntityComponentReplacedEvents[componentPoolIndex]
                    .Invoke(entity, componentPoolIndex, oldComponent, newComponent);
            }
        }

        public void RemoveComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(this, entity, typeof(TComponent));

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            if (ComponentIndex<TComponent>.IsUnique && _entityManager.UniqueEntities[componentPoolIndex] == entity)
                _entityManager.UniqueEntities[componentPoolIndex] = Entity.Null;

            var entityIndexes = _entityManager.EntityComponentIndexes[entity.Id];
            lock (entityIndexes)
            {
                entityIndexes.Remove(componentPoolIndex);
            }

            var oldComponent = _entityManager.ComponentPools[componentPoolIndex].GetComponent(entity.Id);
            _entityManager.ComponentPools[componentPoolIndex].RemoveComponent(entity.Id);

            _entityManager.AnyEntityComponentRemovedEvents.Invoke(entity, componentPoolIndex, oldComponent);
            _entityManager.EntityComponentRemovedEvents[entity.Id].Invoke(entity, componentPoolIndex, oldComponent);
            _entityManager.ComponentPoolEntityComponentRemovedEvents[componentPoolIndex]
                .Invoke(entity, componentPoolIndex, oldComponent);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            var componentIndexes = _entityManager.SilentRemoveAllComponents(entity);

            var anyEvent = _entityManager.AnyEntityComponentRemovedEvents;
            var entityEvent = _entityManager.EntityComponentRemovedEvents[entity.Id];

            for (int i = 0; i < componentIndexes.Item1.Length; i++)
            {
                var componentPoolIndex = componentIndexes.Item1[i];
                var oldComponent = componentIndexes.Item2[i];

                anyEvent.Invoke(entity, componentPoolIndex, oldComponent);
                entityEvent.Invoke(entity, componentPoolIndex, oldComponent);
                _entityManager.ComponentPoolEntityComponentRemovedEvents[componentPoolIndex]
                    .Invoke(entity, componentPoolIndex, oldComponent);
            }
        }

        #endregion

        #region  WithKey

        public EntityKey WithKey(IComponentPrimaryKey primaryKey)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (primaryKey == null)
                throw new ArgumentNullException();

            var keyCollection = new KeyCollection(new[] { primaryKey });
            lock (_entityManager.EntityKeyes)
            {
                if (!_entityManager.EntityKeyes.TryGetValue(keyCollection, out var entityKey))
                {
                    var key = _entityManager.PrimaryKeyes[ComponentIndexes.Instance.GetComponentIndex(primaryKey.GetType())];
                    entityKey = new EntityKey(this, _entityManager, key, primaryKey);
                    _entityManager.EntityKeyes.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
        }

        public EntityKey WithKey(IComponentSharedKey sharedKey)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (sharedKey == null)
                throw new ArgumentNullException();

            var keyCollection = new KeyCollection(new[] { sharedKey });
            lock (_entityManager.EntityKeyes)
            {
                if (!_entityManager.EntityKeyes.TryGetValue(keyCollection, out var entityKey))
                {
                    var key = _entityManager.SharedKeyes[ComponentIndexes.Instance.GetComponentIndex(sharedKey.GetType())];
                    entityKey = new EntityKey(this, _entityManager, new[] { key }, new[] { sharedKey });
                    _entityManager.EntityKeyes.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
        }

        public EntityKey WithKey(params IComponentSharedKey[] sharedKeys)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            foreach (var sharedKey in sharedKeys)
                if (sharedKey == null)
                    throw new ArgumentNullException();

            sharedKeys = sharedKeys.OrderBy(x => x.GetHashCode()).ToArray();
            var keyCollection = new KeyCollection(sharedKeys);
            lock (_entityManager.EntityKeyes)
            {
                if (!_entityManager.EntityKeyes.TryGetValue(keyCollection, out var entityKey))
                {
                    var keyes = new ISharedKey[sharedKeys.Length];
                    for (int i = 0; i < sharedKeys.Length; i++)
                    {
                        keyes[i] = _entityManager.SharedKeyes[ComponentIndexes.Instance.GetComponentIndex(sharedKeys[i]
                            .GetType())];
                    }
                    entityKey = new EntityKey(this, _entityManager, keyes, sharedKeys);
                    _entityManager.EntityKeyes.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
        }

        #endregion
    }
}