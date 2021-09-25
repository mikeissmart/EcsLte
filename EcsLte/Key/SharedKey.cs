using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    internal interface ISharedKey
    {
        SharedKeyData this[IComponentSharedKey componentKey] { get; }

        bool HasEntity(IComponentSharedKey component, Entity entity);
        Entity[] GetEntities(IComponentSharedKey component);
        void SubscribeToEvents(IComponentSharedKey component, EntityEvent added, EntityEvent removed);
        void UnsubscribeToEvents(IComponentSharedKey component, EntityEvent added, EntityEvent removed);
        void Initialize(EcsContext context, EntityManager entityManager);
        void Clear();
    }

    internal class SharedKey<TComponent> : ISharedKey
        where TComponent : IComponentSharedKey
    {
        private Dictionary<TComponent, SharedKeyData> _keyes;
        private EcsContext _context;
        private EntityManager _entityManager;

        public SharedKey()
        {
            _keyes = new Dictionary<TComponent, SharedKeyData>();
        }

        #region SharedKey

        public SharedKeyData this[IComponentSharedKey componentKey]
        {
            get => GetKeyData(componentKey);
        }

        public bool HasEntity(IComponentSharedKey componentKey, Entity entity)
        {
            if (!(componentKey is TComponent))
                throw new SharedKeyWrongTypeException(typeof(TComponent), componentKey.GetType());

            return HasEntity((TComponent)componentKey, entity);
        }

        public bool HasEntity(TComponent componentKey, Entity entity)
        {
            return GetEntities(componentKey).Any(x => x == entity);
        }

        public Entity[] GetEntities(IComponentSharedKey componentKey)
        {
            if (!(componentKey is TComponent))
                throw new SharedKeyWrongTypeException(typeof(TComponent), componentKey.GetType());

            return GetEntities((TComponent)componentKey);
        }

        public Entity[] GetEntities(TComponent componentKey)
        {
            return GetKeyData(componentKey).Entities.GetEntities();
        }

        public void SubscribeToEvents(IComponentSharedKey componentKey, EntityEvent added, EntityEvent removed)
        {
            var keyData = GetKeyData(componentKey);
            keyData.EntityAddedEvent += added;
            keyData.EntityRemovedEvent += removed;
        }

        public void UnsubscribeToEvents(IComponentSharedKey componentKey, EntityEvent added, EntityEvent removed)
        {
            var keyData = GetKeyData(componentKey);
            keyData.EntityAddedEvent -= added;
            keyData.EntityRemovedEvent -= removed;
        }

        public void Initialize(EcsContext context, EntityManager entityManager)
        {
            _context = context;
            _entityManager = entityManager;

            var componentPoolIndex = ComponentIndex<TComponent>.Index;

            _entityManager.ComponentPoolEntityComponentAddedEvents[componentPoolIndex] += OnEntityComponentAdded;
            _entityManager.ComponentPoolEntityComponentReplacedEvents[componentPoolIndex] += OnEntityComponentReplaced;
            _entityManager.ComponentPoolEntityComponentRemovedEvents[componentPoolIndex] += OnEntityComponentRemoved;
            _entityManager.AnyEntityWillBeDestroyedEvents += OnEntityWillBeDestroyed;
        }

        public void Clear()
        {
            foreach (var entities in _keyes.Values)
            {
                entities.Reset();
                ObjectCache.Push(entities);
            }
            _keyes.Clear();

            var componentPoolIndex = ComponentIndex<TComponent>.Index;

            _entityManager.ComponentPoolEntityComponentAddedEvents[componentPoolIndex] -= OnEntityComponentAdded;
            _entityManager.ComponentPoolEntityComponentReplacedEvents[componentPoolIndex] -= OnEntityComponentReplaced;
            _entityManager.ComponentPoolEntityComponentRemovedEvents[componentPoolIndex] -= OnEntityComponentRemoved;
            _entityManager.AnyEntityWillBeDestroyedEvents -= OnEntityWillBeDestroyed;
        }

        private SharedKeyData GetKeyData(IComponentSharedKey componentKey)
        {
            SharedKeyData key = null;
            lock (_keyes)
            {
                if (!_keyes.TryGetValue((TComponent)componentKey, out key))
                {
                    key = ObjectCache.Pop<SharedKeyData>();
                    key.Initialize(_entityManager);
                    _keyes.Add((TComponent)componentKey, key);
                }
            }

            return key;
        }

        #endregion

        #region Events

        private void OnEntityComponentAdded(Entity entity, int componentPoolIndex, IComponent component)
        {
            bool wasAdded = false;
            SharedKeyData key = null;
            lock (_keyes)
            {
                key = GetKeyData((TComponent)component);
                if (key.Entities[entity.Id] != entity)
                {
                    key.Entities[entity.Id] = entity;
                    wasAdded = true;
                }
            }
            if (wasAdded)
                key.EntityAddedEvent.Invoke(entity);
        }

        private void OnEntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent oldComponent, IComponent newComponent)
        {
            OnEntityComponentRemoved(entity, componentPoolIndex, oldComponent);
            OnEntityComponentAdded(entity, componentPoolIndex, newComponent);
        }

        private void OnEntityComponentRemoved(Entity entity, int componentPoolIndex, IComponent component)
        {
            bool wasRemoved = false;
            SharedKeyData key = null;
            lock (_keyes)
            {
                key = GetKeyData((TComponent)component);
                if (_keyes.TryGetValue((TComponent)component, out key))
                {
                    if (key.Entities[entity.Id] == entity)
                    {
                        key.Entities[entity.Id] = Entity.Null;
                        wasRemoved = true;
                        if (key.Entities.GetEntities().Length == 0 &&
                            !key.EntityAddedEvent.HasSubscriptions &&
                            !key.EntityRemovedEvent.HasSubscriptions)
                        {
                            key.Reset();
                            ObjectCache.Push(key);
                            _keyes.Remove((TComponent)component);
                        }
                    }
                }
            }
            if (wasRemoved)
                key.EntityRemovedEvent.Invoke(entity);
        }

        private void OnEntityWillBeDestroyed(Entity entity)
        {
            if (_context.HasComponent<TComponent>(entity))
            {
                OnEntityComponentRemoved(entity,
                    ComponentIndex<TComponent>.Index,
                    _context.GetComponent<TComponent>(entity));
            }
        }

        #endregion
    }
}