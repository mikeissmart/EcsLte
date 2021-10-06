using EcsLte.Exceptions;

namespace EcsLte
{
    public interface IGetWatcher
    {
        Watcher WatchAdded(Filter filter);
        Watcher WatchRemoved(Filter filter);
        Watcher WatchUpdated(Filter filter);
        Watcher WatchAddedOrRemoved(Filter filter);
        Watcher WatchAddedOrUpdated(Filter filter);
    }

    public enum WatchTriggerEvent
    {
        Added = 1,
        Removed = 2,
        Updated = 4
    }

    public class Watcher : IEcsContext, IGetEntity
    {
        private readonly IWatcherData _data;

        internal Watcher(EcsContext context, IWatcherData data)
        {
            _data = data;
            _data.IncRefCount();

            CurrentContext = context;
        }

        #region EcsContext

        public EcsContext CurrentContext { get; }

        #endregion

        ~Watcher()
        {
            _data.DecRefCount();
        }

        #region Watcher

        public bool IsActive
        {
            get
            {
                if (CurrentContext.IsDestroyed)
                    throw new EcsContextIsDestroyedException(CurrentContext);
                return _data.IsActive;
            }
        }

        public void ClearEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            _data.ClearEntities();
        }

        public void SetActive(bool active)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            _data.SetActive(active);
        }

        public static bool operator !=(Watcher lhs, Watcher rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Watcher lhs, Watcher rhs)
        {
            if (lhs is null || rhs is null)
                return false;
            return lhs._data.Equals(rhs._data);
        }

        public bool Equals(Watcher other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Watcher other && _data.Equals(other._data);
        }

        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }

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
    }
}