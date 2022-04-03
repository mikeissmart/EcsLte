using System;
using System.Collections.Generic;
using System.Text;
using EcsLte.Exceptions;

namespace EcsLte
{
	public class EcsContext : IEntityGet, IEntityLife, IEntityComponentGet, IEntityComponentLife
	{
		private IComponentEntityFactory _componentEntityFactory;

		internal EcsContext(string name, IComponentEntityFactory componentEntityFactory)
		{
			_componentEntityFactory = componentEntityFactory;
			Name = name;
		}

		#region EcsContext

		public string Name { get; }
		public bool IsDestroyed { get; private set; }
		public int EntityCount { get => _componentEntityFactory?.Count ?? 0; }
		public int EntityCapacity { get => _componentEntityFactory?.Capacity ?? 0; }

		public void InternalDestroy()
		{
			_componentEntityFactory.Dispose();
			_componentEntityFactory = null;
			IsDestroyed = true;
		}

		#endregion

		#region EntityGet

		public bool HasEntity(Entity entity)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.HasEntity(entity);
		}

		public Entity[] GetEntities()
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.GetEntities();
		}

		#endregion

		#region EntityLife

		public Entity CreateEntity(IEntityBlueprint blueprint = null)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.CreateEntity(blueprint);
		}

		public Entity[] CreateEntities(int count, IEntityBlueprint blueprint = null)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.CreateEntities(count, blueprint);
		}

		public void DestroyEntity(Entity entity)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.DestroyEntity(entity);
		}

		public void DestroyEntities(IEnumerable<Entity> entities)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.DestroyEntities(entities);
		}

		#endregion

		#region ComponentGet

		public bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.HasComponent<TComponent>(entity);
		}

		public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.GetComponent<TComponent>(entity);
		}

		public IComponent[] GetAllComponents(Entity entity)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.GetAllComponents(entity);
		}
		public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.HasUniqueComponent<TComponentUnique>();
		}

		public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.GetUniqueComponent<TComponentUnique>();
		}

		public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.GetUniqueEntity<TComponentUnique>();
		}

		#endregion

		#region ComponentLife

		public void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.AddComponent(entity, component);
		}

		public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent) where TComponent : unmanaged, IComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.ReplaceComponent(entity, newComponent);
		}

		public void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.RemoveComponent<TComponent>(entity);
		}

		public void RemoveAllComponents(Entity entity)
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.RemoveAllComponents(entity);
		}

		public Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.AddUniqueComponent(componentUnique);
		}

		public Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			return _componentEntityFactory.ReplaceUniqueComponent<TComponentUnique>(newComponentUnique);
		}

		public void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
		{
			if (IsDestroyed)
				throw new EcsContextIsDestroyedException(this);

			_componentEntityFactory.RemoveUniqueComponent<TComponentUnique>();
		}

		#endregion
	}
}
