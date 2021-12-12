using EcsLte.Exceptions;

namespace EcsLte
{
	public class EntityFilter : IEcsContext, IGetEntity, IGetWatcher
	{
		private readonly EntityFilterData _data;

		internal EntityFilter(EcsContext context, EntityFilterData data)
		{
			_data = data;

			CurrentContext = context;
		}

		#region EcsContext

		public EcsContext CurrentContext { get; }

		#endregion

		~EntityFilter()
		{
			_data.DecRefCount();
		}

		#region EntityFilter

		public Filter Filter
		{
			get
			{
				if (CurrentContext.IsDestroyed)
					throw new EcsContextIsDestroyedException(CurrentContext);

				return _data.Filter;
			}
		}

		public static bool operator !=(EntityFilter lhs, EntityFilter rhs) => !(lhs == rhs);

		public static bool operator ==(EntityFilter lhs, EntityFilter rhs)
		{
			if (lhs is null || rhs is null)
				return false;
			return lhs._data.Equals(rhs._data);
		}

		public bool Equals(EntityFilter other) => this == other;

		public override bool Equals(object obj) => obj is EntityFilter other && _data.Equals(other._data);

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