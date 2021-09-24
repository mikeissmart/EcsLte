using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityKey : IEcsContext, IGetEntity//, IGetWatcher//, IFilterBy
    {
        private EntityManager _entityManager;
        private EntityKeyData _data;

        private EntityKey(EcsContext context, EntityManager entityManager)
        {
            _entityManager = entityManager;
            _data = ObjectCache.Pop<EntityKeyData>();
            _data.Initialize(context);
        }

        internal EntityKey(EcsContext context, EntityManager entityManager,
            IPrimaryKey primaryKey, IComponentPrimaryKey primaryComponent) : this(context, entityManager)
        {
            _data._primaryKey = primaryKey;
            _data._primaryComponent = primaryComponent;

            var entity = primaryKey.GetEntity(primaryComponent);
            if (entity != Entity.Null)
            {
                _data.Entities[entity.Id] = entity;
                _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            }
            primaryKey.SubscribeToEvents(primaryComponent, PrimaryKeyOnEntityAdded, PrimaryKeyOnEntityRemoved);

            CurrentContext = context;
        }

        internal EntityKey(EcsContext context, EntityManager entityManager,
            ISharedKey[] sharedKeyes, IComponentSharedKey[] sharedComponents) : this(context, entityManager)
        {
            _data._sharedKeyes = sharedKeyes;
            _data._sharedComponents = sharedComponents;

            // Initial key
            sharedKeyes[0].SubscribeToEvents(sharedComponents[0], SharedKeyOnEntityAdded, SharedKeyOnEntityRemoved);
            var keyEntities = sharedKeyes[0].GetEntities(sharedComponents[0]);

            // Rest of the keyes
            for (int i = 1; i < sharedKeyes.Length; i++)
            {
                sharedKeyes[i].SubscribeToEvents(sharedComponents[i], SharedKeyOnEntityAdded, SharedKeyOnEntityRemoved);
                var checkKeyEntities = sharedKeyes[i].GetEntities(sharedComponents[i]);
                if (keyEntities.Length > 0)
                    keyEntities = sharedKeyes[i].GetEntities(sharedComponents[i])
                        .Where(x => keyEntities.Any(y => x == y))
                        .ToArray();
            }

            // All keyes have these entities
            for (int i = 0; i < keyEntities.Length; i++)
            {
                var entity = keyEntities[i];
                _data.Entities[entity.Id] = entity;
                _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            }

            CurrentContext = context;
        }

        #region EcsContext

        public EcsContext CurrentContext { get; private set; }

        #endregion

        #region EntityKey

        public Entity GetFirstOrDefault()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Entities.GetEntities()
                .Where(x => x != Entity.Null)
                .FirstOrDefault();
        }

        internal void InternalDestroy()
        {
            if (_data._primaryKey != null)
            {
                _data._primaryKey.SubscribeToEvents(_data._primaryComponent, PrimaryKeyOnEntityAdded, PrimaryKeyOnEntityRemoved);
            }
            if (_data._sharedKeyes != null)
            {
                for (int i = 1; i < _data._sharedKeyes.Length; i++)
                    _data._sharedKeyes[i].UnsubscribeToEvents(_data._sharedComponents[i], SharedKeyOnEntityAdded, SharedKeyOnEntityRemoved);
            }
            var entities = _data.Entities.GetEntities();
            for (int i = 0; i < entities.Length; i++)
                _entityManager.EntityWillBeDestroyedEvents[entities[i].Id] -= OnEntityWillBeDestroyed;
        }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);
            if (entity.Id <= 0 || entity.Id >= _data.Entities.Length)
                return false;

            return _data.Entities[entity.Id] == entity;
        }

        public Entity[] GetEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Entities.GetEntities();
        }

        #endregion

        #region Events

        private void PrimaryKeyOnEntityAdded(Entity entity)
        {
            _data.Entities[entity.Id] = entity;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
        }

        private void PrimaryKeyOnEntityRemoved(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
        }

        private void SharedKeyOnEntityAdded(Entity entity)
        {
            bool hasEntity = true;
            for (int i = 0; i < _data._sharedKeyes.Length; i++)
            {
                if (!_data._sharedKeyes[i].HasEntity(_data._sharedComponents[i], entity))
                {
                    hasEntity = false;
                    break;
                }
            }

            if (hasEntity)
            {
                _data.Entities[entity.Id] = entity;
                _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            }
        }

        private void SharedKeyOnEntityRemoved(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
        }

        private void OnEntityWillBeDestroyed(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
        }

        #endregion
    }
}