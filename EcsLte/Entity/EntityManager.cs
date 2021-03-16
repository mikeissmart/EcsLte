using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public class EntityManager
	{
		private readonly List<EntityInfo> _entityInfos;
		private readonly Queue<EntityInfo> _reuseableEntityInfos;
		private readonly DataCache<Entity[]> _entitiesCache;

		internal EntityManager(World world)
		{
			_entityInfos = new List<EntityInfo>();
			_reuseableEntityInfos = new Queue<EntityInfo>();
			_entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);
			World = world;

			AnyEntityCreated = new EntityEvent();
			AnyEntityWillBeDestroyedEvent = new EntityEvent();
			AnyComponentAddedEvent = new EntityComponentChangedEvent();
			AnyComponentRemovedEvent = new EntityComponentChangedEvent();
			AnyComponentReplacedEvent = new EntityComponentReplacedEvent();

			// Create place holder for null entity
			var entityInfo = new EntityInfo(0, 0, world);
			entityInfo.Reset();
			_entityInfos.Add(entityInfo);
		}

		public World World { get; private set; }

		internal EntityEvent AnyEntityCreated { get; private set; }
		internal EntityEvent AnyEntityWillBeDestroyedEvent { get; private set; }
		internal EntityComponentChangedEvent AnyComponentAddedEvent { get; private set; }
		internal EntityComponentChangedEvent AnyComponentRemovedEvent { get; private set; }
		internal EntityComponentReplacedEvent AnyComponentReplacedEvent { get; private set; }

		public Entity CreateEntity()
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);

			EntityInfo entityInfo;
			if (_reuseableEntityInfos.Count > 0)
			{
				entityInfo = _reuseableEntityInfos.Dequeue();
				entityInfo.Generation++;
				entityInfo.IsAlive = true;
			}
			else
			{
				entityInfo = new EntityInfo(_entityInfos.Count, 1, World);
				_entityInfos.Add(entityInfo);
			}
			var entity = new Entity { Info = entityInfo };

			_entitiesCache.IsDirty = true;
			AnyEntityCreated.Invoke(entity);

			return entity;
		}

		public void DestroyEntity(Entity entity)
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(World, entity);

			var entityInfo = entity.Info;

			AnyEntityWillBeDestroyedEvent.Invoke(entity);
			RemoveAllComponents(entity);
			entityInfo.Reset();

			_reuseableEntityInfos.Enqueue(entityInfo);
			_entitiesCache.IsDirty = true;
		}

		public void DestroyAllEntities()
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);

			var entities = GetEntities();
			for (int i = 0; i < entities.Length; i++)
				DestroyEntity(entities[i]);
		}

		public Entity GetEntity(int id)
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);
			if (id <= 0 && id >= _entityInfos.Count)
				throw new ArgumentOutOfRangeException();

			var entity = new Entity { Info = _entityInfos[id] };
			if (!entity.Info.IsAlive)
				throw new WorldDoesNotHaveEntityException(World, entity);

			return entity;
		}

		public Entity[] GetEntities()
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);

			return _entitiesCache.Data;
		}

		public bool HasEntity(Entity entity)
		{
			if (World.IsDestroyed)
				throw new WorldIsDestroyedException(World);
			if (entity.Id <= 0 && entity.Id >= _entityInfos.Count)
				throw new ArgumentOutOfRangeException();
			return entity.WorldId == World.WorldId && entity.Info.IsAlive;
		}

		public bool HasComponent<TComponent>(Entity entity)
			where TComponent : IComponent
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(World, entity);

			return entity.Info[ComponentIndex<TComponent>.Index] != null;
		}

		public TComponent AddComponent<TComponent>(Entity entity, TComponent component = default)
			where TComponent : IComponent
		{
			if (HasComponent<TComponent>(entity))
				throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

			var componentIndex = ComponentIndex<TComponent>.Index;

			entity.Info[componentIndex] = component;
			AnyComponentAddedEvent.Invoke(entity, componentIndex, component);

			return component;
		}

		public TComponent GetComponent<TComponent>(Entity entity)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>(entity))
				throw new EntityNotHaveComponentException(entity, typeof(TComponent));

			return (TComponent)entity.Info[ComponentIndex<TComponent>.Index];
		}

		public void RemoveComponent<TComponent>(Entity entity)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>(entity))
				throw new EntityNotHaveComponentException(entity, typeof(TComponent));

			var componentIndex = ComponentIndex<TComponent>.Index;
			var component = entity.Info[componentIndex];

			entity.Info[componentIndex] = null;
			AnyComponentRemovedEvent.Invoke(entity, componentIndex, component);
		}

		public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>(entity))
				AddComponent(entity, newComponent);
			{
				var componentIndex = ComponentIndex<TComponent>.Index;
				var prevComponent = entity.Info[componentIndex];

				entity.Info[componentIndex] = newComponent;
				AnyComponentReplacedEvent.Invoke(entity, componentIndex, prevComponent, newComponent);
			}
		}

		public IComponent[] GetAllComponents(Entity entity)
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(World, entity);
			return entity.Info.GetComponents();
		}

		public void RemoveAllComponents(Entity entity)
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(World, entity);

			for (int i = 0; i < ComponentIndexes.Instance.Count; i++)
			{
				var component = entity.Info[i];
				if (component != null)
				{
					entity.Info[i] = null;
					AnyComponentRemovedEvent.Invoke(entity, i, component);
				}
			}
		}

		internal IComponent GetComponent(Entity entity, int componentPoolIndex)
			=> entity.Info[componentPoolIndex];

		private Entity[] UpdateEntitiesCache()
			=> _entityInfos
					.Where(x => x.IsAlive)
					.Select(x => new Entity { Info = x })
					.ToArray();
	}
}