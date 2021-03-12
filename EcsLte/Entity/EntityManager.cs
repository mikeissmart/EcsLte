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
		private readonly IComponentPool[] _componentPools;
		private readonly World _world;

		internal EntityManager(World world)
		{
			_entityInfos = new List<EntityInfo>();
			_reuseableEntityInfos = new Queue<EntityInfo>();
			_entitiesCache = new DataCache<Entity[]>(UpdateEntitiesCache);
			_componentPools = new IComponentPool[ComponentIndexes.Count];
			_world = world;

			AnyEntityCreated = new EntityEvent();
			AnyEntityWillBeDestroyedEvent = new EntityEvent();
			AnyComponentAddedEvent = new EntityComponentChangedEvent();
			AnyComponentRemovedEvent = new EntityComponentChangedEvent();
			AnyComponentReplacedEvent = new EntityComponentReplacedEvent();

			// Create place holder for null entity
			var entityInfo = new EntityInfo(0, 0, world, null);
			entityInfo.Reset();
			_entityInfos.Add(entityInfo);

			// Create data dependent on component count
			for (int i = 0; i < ComponentIndexes.Count; i++)
				_componentPools[i] = (IComponentPool)Activator
					.CreateInstance(typeof(ComponentPool<>)
					.MakeGenericType(ComponentIndexes.AllComponentTypes[i]));
		}

		internal EntityEvent AnyEntityCreated { get; private set; }
		internal EntityEvent AnyEntityWillBeDestroyedEvent { get; private set; }
		internal EntityComponentChangedEvent AnyComponentAddedEvent { get; private set; }
		internal EntityComponentChangedEvent AnyComponentRemovedEvent { get; private set; }
		internal EntityComponentReplacedEvent AnyComponentReplacedEvent { get; private set; }

		public Entity CreateEntity()
		{
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);

			EntityInfo entityInfo;
			if (_reuseableEntityInfos.Count > 0)
			{
				entityInfo = _reuseableEntityInfos.Dequeue();
				entityInfo.Generation++;
				entityInfo.IsAlive = true;
			}
			else
			{
				entityInfo = new EntityInfo(_entityInfos.Count, 1, _world, _componentPools);
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
				throw new WorldDoesNotHaveEntityException(_world, entity);

			var entityInfo = _entityInfos[entity.Id];

			AnyEntityWillBeDestroyedEvent.Invoke(entity);
			RemoveAllComponents(entity);
			entityInfo.Reset();

			_reuseableEntityInfos.Enqueue(entityInfo);
			_entitiesCache.IsDirty = true;
		}

		public void DestroyAllEntities()
		{
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);

			var entities = GetEntities();
			for (int i = 0; i < entities.Length; i++)
				DestroyEntity(entities[i]);
		}

		public Entity GetEntity(int id)
		{
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);
			if (id <= 0 && id >= _entityInfos.Count)
				throw new ArgumentOutOfRangeException();

			var entity = new Entity { Info = _entityInfos[id] };
			if (!entity.Info.IsAlive)
				throw new WorldDoesNotHaveEntityException(_world, entity);

			return entity;
		}

		public Entity[] GetEntities()
		{
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);

			return _entitiesCache.Data;
		}

		public bool HasEntity(Entity entity)
		{
			if (_world.IsDestroyed)
				throw new WorldIsDestroyedException(_world);
			if (entity.Id <= 0 && entity.Id >= _entityInfos.Count)
				throw new ArgumentOutOfRangeException();
			return entity.WorldId == _world.WorldId && _entityInfos[entity.Id].IsAlive;
		}

		public bool HasComponent<TComponent>(Entity entity)
			where TComponent : IComponent
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(_world, entity);

			return _entityInfos[entity.Id][ComponentIndex<TComponent>.Index] != 0;
		}

		public TComponent AddComponent<TComponent>(Entity entity, TComponent component = default)
			where TComponent : IComponent
		{
			if (HasComponent<TComponent>(entity))
				throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

			var entityInfo = _entityInfos[entity.Id];
			var componentPoolIndex = ComponentIndex<TComponent>.Index;
			var componentPool = _componentPools[componentPoolIndex];

			entityInfo[componentPoolIndex] = componentPool.AddComponent(component);
			AnyComponentAddedEvent.Invoke(entity, componentPoolIndex, component);

			return component;
		}

		public TComponent GetComponent<TComponent>(Entity entity)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>(entity))
				throw new EntityNotHaveComponentException(entity, typeof(TComponent));

			var entityInfo = _entityInfos[entity.Id];
			var componentPoolIndex = ComponentIndex<TComponent>.Index;
			var componentPool = _componentPools[componentPoolIndex];

			return (TComponent)componentPool.GetComponent(entityInfo[componentPoolIndex]);
		}

		public void RemoveComponent<TComponent>(Entity entity)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>(entity))
				throw new EntityNotHaveComponentException(entity, typeof(TComponent));

			var entityInfo = _entityInfos[entity.Id];
			var componentPoolIndex = ComponentIndex<TComponent>.Index;
			var componentPool = _componentPools[componentPoolIndex];
			var componentIndex = entityInfo[componentPoolIndex];
			var component = componentPool.GetComponent(componentIndex);

			componentPool.ClearComponent(componentIndex);
			entityInfo[componentPoolIndex] = 0;
			AnyComponentRemovedEvent.Invoke(entity, componentPoolIndex, component);
		}

		public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>(entity))
				AddComponent(entity, newComponent);
			{
				var entityInfo = _entityInfos[entity.Id];
				var componentPoolIndex = ComponentIndex<TComponent>.Index;
				var componentPool = _componentPools[componentPoolIndex];
				var componentIndex = entityInfo[componentPoolIndex];
				var prevComponent = componentPool.GetComponent(componentIndex);

				componentPool.SetComponent(componentIndex, newComponent);
				AnyComponentReplacedEvent.Invoke(entity, componentPoolIndex, prevComponent, newComponent);
			}
		}

		public IComponent[] GetAllComponents(Entity entity)
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(_world, entity);
			return _entityInfos[entity.Id].GetComponents();
		}

		public void RemoveAllComponents(Entity entity)
		{
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(_world, entity);

			var entityInfo = _entityInfos[entity.Id];
			for (int i = 0; i < _componentPools.Length; i++)
			{
				var componentIndex = entityInfo[i];
				if (componentIndex != 0)
				{
					var componentPool = _componentPools[i];
					var prevComponent = componentPool.GetComponent(componentIndex);

					componentPool.ClearComponent(componentIndex);
					entityInfo[i] = 0;

					AnyComponentRemovedEvent.Invoke(entity, i, prevComponent);
				}
			}
		}

		private Entity[] UpdateEntitiesCache()
			=> _entityInfos
					.Where(x => x.IsAlive)
					.Select(x => new Entity { Info = x })
					.ToArray();
	}
}