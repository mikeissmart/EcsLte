using System;
using System.Collections.Generic;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    internal interface IPrimaryKey
    {
        Entity GetEntity(IComponentPrimaryKey component);
        void SubscribeToEvents(IComponentPrimaryKey component, EntityEvent added, EntityEvent removed);
        void UnsubscribeToEvents(IComponentPrimaryKey component, EntityEvent added, EntityEvent removed);
        void Initialize(EcsContext context, EntityManager entityManager);
        void Clear();
    }

    internal class PrimaryKey<TComponent> : IPrimaryKey
        where TComponent : IComponentPrimaryKey
    {
        private class KeyData
        {
            public EntityEventHandler EntityAddedEvent { get; set; }
            public EntityEventHandler EntityRemovedEvent { get; set; }
            public Entity Entity { get; set; }

            public void Clear()
            {
                EntityAddedEvent.Clear();
                EntityRemovedEvent.Clear();
                Entity = Entity.Null;
            }
        }

        private Dictionary<TComponent, KeyData> _keyes;
        private EcsContext _context;
        private EntityManager _entityManager;

        public PrimaryKey()
        {
            _keyes = new Dictionary<TComponent, KeyData>();
        }

        #region PrimaryKey

        public Entity GetEntity(IComponentPrimaryKey componentKey)
        {
            if (!(componentKey is TComponent))
                throw new PrimaryKeyWrongTypeException(typeof(TComponent), componentKey.GetType());

            return GetEntity((TComponent)componentKey);
        }

        public Entity GetEntity(TComponent componentKey)
        {
            lock (_keyes)
            {
                return GetKeyData(componentKey).Entity;
            }
        }

        public void SubscribeToEvents(IComponentPrimaryKey componentKey, EntityEvent added, EntityEvent removed)
        {
            var keyData = GetKeyData(componentKey);
            keyData.EntityAddedEvent += added;
            keyData.EntityRemovedEvent += removed;
        }

        public void UnsubscribeToEvents(IComponentPrimaryKey componentKey, EntityEvent added, EntityEvent removed)
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
            _keyes.Clear();

            var componentPoolIndex = ComponentIndex<TComponent>.Index;

            _entityManager.ComponentPoolEntityComponentAddedEvents[componentPoolIndex] -= OnEntityComponentAdded;
            _entityManager.ComponentPoolEntityComponentReplacedEvents[componentPoolIndex] -= OnEntityComponentReplaced;
            _entityManager.ComponentPoolEntityComponentRemovedEvents[componentPoolIndex] -= OnEntityComponentRemoved;
            _entityManager.AnyEntityWillBeDestroyedEvents -= OnEntityWillBeDestroyed;
        }

        private KeyData GetKeyData(IComponentPrimaryKey componentKey)
        {
            KeyData key = null;
            lock (_keyes)
            {
                if (!_keyes.TryGetValue((TComponent)componentKey, out key))
                {
                    key = ObjectCache.Pop<KeyData>();
                    _keyes.Add((TComponent)componentKey, key);
                }
            }

            return key;
        }

        private void CheckRemoveKeyData(TComponent componentKey, KeyData keyData)
        {
            if (!keyData.EntityAddedEvent.HasSubscriptions &&
                !keyData.EntityRemovedEvent.HasSubscriptions &&
                keyData.Entity == Entity.Null)
            {
                lock (_keyes)
                {
                    _keyes.Remove(componentKey);
                }
            }
        }

        #endregion

        #region Events

        private void OnEntityComponentAdded(Entity entity, int componentPoolIndex, IComponent component)
        {
            KeyData key = null;
            lock (_keyes)
            {
                key = GetKeyData((TComponent)component);
                if (key.Entity != Entity.Null)
                    throw new PrimaryKeyAlreadyHasException(_context, (TComponent)component);
                key.Entity = entity;
            }
            key.EntityAddedEvent.Invoke(entity);
        }

        private void OnEntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent oldComponent, IComponent newComponent)
        {
            OnEntityComponentRemoved(entity, componentPoolIndex, oldComponent);
            OnEntityComponentAdded(entity, componentPoolIndex, newComponent);
        }

        private void OnEntityComponentRemoved(Entity entity, int componentPoolIndex, IComponent component)
        {
            KeyData key = null;
            lock (_keyes)
            {
                key = GetKeyData((TComponent)component);
                key.Entity = Entity.Null;
                CheckRemoveKeyData((TComponent)component, key);
            }
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