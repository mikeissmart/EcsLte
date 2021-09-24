using System;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public interface IGetWatcher
    {
        Watcher Added(Filter filter);
        Watcher Removed(Filter filter);
        Watcher Updated(Filter filter);
        Watcher AddedOrRemoved(Filter filter);
        Watcher AddedOrUpdated(Filter filter);
    }

    public enum WatchTriggerEvent
    {
        Added = 1,
        Removed = 2,
        Updated = 4
    }

    public class Watcher : IEcsContext, IGetEntity
    {
        private WatcherData _data;

        internal Watcher(EcsContext context)
        {
            _data = ObjectCache.Pop<WatcherData>();
            _data.Initialize(context);

            CurrentContext = context;
            IsActive = true;
        }

        #region EcsContext

        public EcsContext CurrentContext { get; private set; }

        #endregion

        #region Watcher

        public bool IsActive { get; set; }

        public void ClearEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            _data.Entities.Reset();
        }

        public void Activate()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            IsActive = true;
        }

        public void Deactivate()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            ClearEntities();
            IsActive = false;
        }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);

            IsActive = false;
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

        #region InternalCallback

        internal void AddedEntity(Entity entity)
        {
            if (IsActive)
                _data.Entities[entity.Id] = entity;
        }

        internal void UpdatedEntity(Entity entity)
        {
            if (IsActive)
                _data.Entities[entity.Id] = entity;
        }

        internal void RemovedEntity(Entity entity)
        {
            if (IsActive)
                _data.Entities[entity.Id] = entity;
        }

        internal void EntityWillBeDestroyed(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
        }

        #endregion

    }
}