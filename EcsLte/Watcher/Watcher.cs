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
            _data.Initialize();

            CurrentContext = context;
            IsActive = true;
        }

        #region Watcher

        public bool IsActive { get; set; }

        public void ClearEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            _data.ClearEntities();
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

            IsActive = false;
            _data.ClearEntities();
        }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);

            IsActive = false;
        }

        #endregion

        #region EcsContext

        public EcsContext CurrentContext { get; private set; }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (!CurrentContext.HasEntity(entity))
                return false;

            return _data.HasEntity(entity);
        }

        public Entity[] GetEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.GetEntities();
        }

        #endregion

        #region WatcherCallback

        internal void AddedEntity(Entity entity)
        {
            if (IsActive)
                _data.AddEntity(entity);
        }

        internal void UpdatedEntity(Entity entity)
        {
            if (IsActive)
                _data.AddEntity(entity);
        }

        internal void RemovedEntity(Entity entity)
        {
            if (IsActive)
                _data.AddEntity(entity);
        }

        #endregion

    }
}