using EcsLte.Exceptions;

namespace EcsLte
{
	public class EntityGroup : IEcsContext, IGetEntity, IGetWatcher
	{
		private readonly EntityGroupData _data;

		internal EntityGroup(EcsContext context, EntityGroupData data)
		{
			_data = data;

			CurrentContext = context;
		}

		#region EcsContext

		public EcsContext CurrentContext { get; }

		#endregion

		~EntityGroup()
		{
			_data.DecRefCount();
		}

		#region EntityGroup

		public ISharedComponent[] SharedComponents
		{
			get
			{
				if (CurrentContext.IsDestroyed)
					throw new EcsContextIsDestroyedException(CurrentContext);

				return _data.SharedComponents;
			}
		}

		public static bool operator !=(EntityGroup lhs, EntityGroup rhs) => !(lhs == rhs);

		public static bool operator ==(EntityGroup lhs, EntityGroup rhs)
		{
			if (lhs is null || rhs is null)
				return false;
			return lhs._data.HashCode == rhs._data.HashCode;
		}

		public bool Equals(EntityGroup other) => this == other;

		public override bool Equals(object obj) => obj is EntityGroup other && this == other;

		public override int GetHashCode() => _data.GetHashCode();

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