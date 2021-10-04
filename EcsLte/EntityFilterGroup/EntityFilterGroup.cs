using EcsLte.Exceptions;

namespace EcsLte
{
    public class EntityFilterGroup : IEcsContext, IGetEntity, IGetWatcher
    {
        private readonly EntityFilterGroupData _data;

        internal EntityFilterGroup(EcsContext context, EntityFilterGroupData data)
        {
            _data = data;

            CurrentContext = context;
        }

        ~EntityFilterGroup()
        {
            _data.DecRefCount();
        }

        #region EntityGroup

        public Filter Filter
        {
            get
            {
                if (CurrentContext.IsDestroyed)
                    throw new EcsContextIsDestroyedException(CurrentContext);

                return _data.Filter;
            }
        }
        public ISharedComponent[] SharedKeys
        {
            get
            {
                if (CurrentContext.IsDestroyed)
                    throw new EcsContextIsDestroyedException(CurrentContext);

                return _data.SharedComponents;
            }
        }

        public static bool operator !=(EntityFilterGroup lhs, EntityFilterGroup rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(EntityFilterGroup lhs, EntityFilterGroup rhs)
        {
            if (lhs is null || rhs is null)
                return false;
            return lhs._data.HashCode == rhs._data.HashCode;
        }

        public bool Equals(EntityFilterGroup other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityFilterGroup other && this == other;
        }

        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }

        #endregion

        #region EcsContext

        public EcsContext CurrentContext { get; }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Entities.HasEntity(entity);
        }

        public Entity[] GetEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Entities.GetEntities();
        }

        #endregion

        #region GetWatcher

        public Watcher WatchAdded(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Watchers.Added(CurrentContext, filter);
        }

        public Watcher WatchUpdated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Watchers.Updated(CurrentContext, filter);
        }

        public Watcher WatchRemoved(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Watchers.Removed(CurrentContext, filter);
        }

        public Watcher WatchAddedOrUpdated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Watchers.AddedOrUpdated(CurrentContext, filter);
        }

        public Watcher WatchAddedOrRemoved(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Watchers.AddedOrRemoved(CurrentContext, filter);
        }

        #endregion

    }
}