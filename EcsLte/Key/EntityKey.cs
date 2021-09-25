using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityKey : IEcsContext, IGetEntity, IGetWatcher
    {
        private EntityKeyData _data;
        private WatcherTable _watcherTable;
        private EntityManager _entityManager;

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
            var initialEntities = sharedKeyes[0].GetEntities(sharedComponents[0]);

            if (sharedKeyes.Length > 1)
            {
                var cachedKeyEntities = new EntityCollection[sharedKeyes.Length - 1];
                for (int i = 1; i < sharedKeyes.Length; i++)
                {
                    var sharedKey = sharedKeyes[i];
                    sharedKey.SubscribeToEvents(sharedComponents[i], SharedKeyOnEntityAdded, SharedKeyOnEntityRemoved);
                    cachedKeyEntities[i - 1] = sharedKey[sharedComponents[i]].Entities;
                }
                ParallelRunner.RunParallelFor(initialEntities.Length,
                   entityId =>
                   {
                       bool allHave = true;
                       var entity = initialEntities[entityId];
                       for (int i = 0; i < cachedKeyEntities.Length; i++)
                       {
                           if (cachedKeyEntities[i][entityId] != entity)
                           {
                               allHave = false;
                               break;
                           }
                       }

                       if (allHave)
                       {
                           _entityManager.EntityWillBeDestroyedEvents[entityId] += OnEntityWillBeDestroyed;
                           _data.Entities[entityId] = entity;
                       }
                   });
            }
            else
            {
                for (int i = 0; i < initialEntities.Length; i++)
                {
                    var entity = initialEntities[i];
                    _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
                    _data.Entities[entity.Id] = entity;
                }
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

        #region GetWatcher

        public Watcher Added(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Added(filter);
        }

        public Watcher Updated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Updated(filter);
        }

        public Watcher Removed(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Removed(filter);
        }

        public Watcher AddedOrUpdated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.AddedOrUpdated(filter);
        }

        public Watcher AddedOrRemoved(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.AddedOrRemoved(filter);
        }

        #endregion

        #region Events

        private void PrimaryKeyOnEntityAdded(Entity entity)
        {
            _data.Entities[entity.Id] = entity;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            _watcherTable.AddedEntity(entity);
        }

        private void PrimaryKeyOnEntityRemoved(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
            _watcherTable.RemovedEntity(entity);
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
                _watcherTable.AddedEntity(entity);
            }
        }

        private void SharedKeyOnEntityRemoved(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
            _watcherTable.RemovedEntity(entity);
        }

        private void OnEntityWillBeDestroyed(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
            _watcherTable.EntityWillBeDestroyed(entity);
        }

        #endregion
    }
}