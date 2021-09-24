using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityFilter : IEcsContext, IGetEntity, IGetWatcher
    {
        private EntityFilterData _data;
        private WatcherTable _watcherTable;
        private EntityManager _entityManager;

        internal EntityFilter(EcsContext context, EntityManager entityManager, Filter filter, Entity[] entities)
        {
            _data = ObjectCache.Pop<EntityFilterData>();
            _data.Initialize(context);
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                _data.Entities[entity.Id] = entity;
                entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            }
            _watcherTable = ObjectCache.Pop<WatcherTable>();
            _watcherTable.Initialize(context);
            _entityManager = entityManager;

            CurrentContext = context;
            Filter = filter;

            for (int i = 0; i < filter.Indexes.Length; i++)
            {
                var componentPoolIndex = filter.Indexes[i];

                _entityManager.ComponentPoolEntityComponentAddedEvents[componentPoolIndex] += OnEntityComponentAdded;
                _entityManager.ComponentPoolEntityComponentReplacedEvents[componentPoolIndex] += OnEntityComponentReplaced;
                _entityManager.ComponentPoolEntityComponentRemovedEvents[componentPoolIndex] += OnEntityComponentRemoved;
            }
        }

        #region EcsContext

        public EcsContext CurrentContext { get; private set; }

        #endregion

        #region EntityFilter

        public Filter Filter { get; private set; }

        private void LocalFilterEntity(Entity entity)
        {
            if (CurrentContext.InternalFilteredBy(entity, Filter))
            {
                if (!HasEntity(entity))
                {
                    _data.Entities[entity.Id] = entity;
                    _watcherTable.AddedEntity(entity);
                    _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
                }
            }
            else if (HasEntity(entity))
            {
                _data.Entities[entity.Id] = Entity.Null;
                _watcherTable.RemovedEntity(entity);
                _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
            }
        }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);
            _watcherTable.Reset();
            ObjectCache.Push(_watcherTable);
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

        private void OnEntityComponentAdded(Entity entity, int componentPoolIndex, IComponent component)
        {
            LocalFilterEntity(entity);
        }

        private void OnEntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent oldComponent, IComponent newComponent)
        {
            if (HasEntity(entity))
                _watcherTable.UpdatedEntity(entity);
        }

        private void OnEntityComponentRemoved(Entity entity, int componentPoolIndex, IComponent component)
        {
            LocalFilterEntity(entity);
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