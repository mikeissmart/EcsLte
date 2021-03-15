using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
	public class World
	{
		internal World(int worldId) => WorldId = worldId;

		internal World()
		{
			EntityManager = new EntityManager(this);
			GroupManager = new GroupManager(this, EntityManager);
			KeyManager = new KeyManager(this, EntityManager, GroupManager);
		}

		public static World DefaultWorld { get; set; } = CreateWorld();

		public int WorldId { get; private set; }
		public bool IsDestroyed { get; private set; }
		public EntityManager EntityManager { get; private set; }
		public GroupManager GroupManager { get; private set; }
		public KeyManager KeyManager { get; private set; }

		internal static List<World> Worlds { get; private set; }

		public static World CreateWorld()
		{
			if (Worlds == null)
				Worlds = new List<World> { null };

			var world = new World
			{
				WorldId = Worlds.Count
			};

			Worlds.Add(world);
			return world;
		}

		public static void DestroyWorld(World world)
		{
			if (world == null)
				throw new ArgumentNullException();

			if (world.IsDestroyed)
				throw new WorldIsDestroyedException(world);

			world.Destroy();

			Worlds[world.WorldId] = null;
		}

		public static World GetWorld(int worldId)
		{
			if (worldId <= 0 || worldId >= Worlds.Count)
				throw new ArgumentOutOfRangeException();
			var world = Worlds[worldId];
			if (world == null)
				throw new WorldIsDestroyedException(new World(worldId));

			return world;
		}

		public override string ToString()
			=> WorldId.ToString();

		internal void Destroy()
		{
			IsDestroyed = true;
		}
	}

	//public class World
	//{
	//	private readonly Queue<EntityInfo> _reuseableEntityInfos;
	//	private readonly List<EntityInfo> _entityInfos;
	//	private readonly IComponentPool[] _componentPools;
	//	private readonly Dictionary<Filter, Group> _groupLookup;
	//	private readonly List<Group>[] _groupComponentIndexes;

	//	internal World()
	//	{
	//		_entityInfos = new List<EntityInfo>();
	//		_reuseableEntityInfos = new Queue<EntityInfo>();
	//		_componentPools = new IComponentPool[ComponentIndexes.Count];
	//		_groupLookup = new Dictionary<Filter, Group>();
	//		_groupComponentIndexes = new List<Group>[ComponentIndexes.Count];

	//		EntityCreatedEvent = new EntityChangedEvent();
	//		EntityWillBeDestroyedEvent = new EntityChangedEvent();

	//		// Create place holder for null entity
	//		_entityInfos.Add(new EntityInfo());

	//		// Create data dependent on component count
	//		for (int i = 0; i < ComponentIndexes.Count; i++)
	//		{
	//			// Create component pools
	//			_componentPools[i] = (IComponentPool)Activator
	//				.CreateInstance(typeof(ComponentPool<>).MakeGenericType(ComponentIndexes.AllComponentTypes[i]));

	//			_groupComponentIndexes[i] = new List<Group>();
	//		}
	//	}

	//	public static World DefaultWorld { get; set; } = CreateWorld();

	//	public bool IsDestroyed { get; private set; }
	//	public int WorldId { get; internal set; }

	//	internal EntityManager EntityManager { get; private set; }
	//	internal GroupManager GroupManager { get; private set; }

	//	internal static List<World> Worlds { get; private set; }

	//	internal EntityChangedEvent EntityCreatedEvent { get; private set; }
	//	internal EntityChangedEvent EntityWillBeDestroyedEvent { get; private set; }

	//	public static World CreateWorld()
	//	{
	//		if (Worlds == null)
	//			Worlds = new List<World> { null };

	//		var world = new World
	//		{
	//			WorldId = Worlds.Count
	//		};

	//		Worlds.Add(world);
	//		return world;
	//	}

	//	public static World CreateWorld(WorldRecordableData recordableData)
	//	{
	//		// TODO: code, entities and components with entity props will need to be remapped
	//		return CreateWorld();
	//	}

	//	public static void DestroyWorld(World world)
	//	{
	//		if (world == null)
	//			throw new ArgumentNullException();

	//		if (world.IsDestroyed)
	//			throw new WorldIsDestroyedException(world);

	//		world.FinializeDestroy();

	//		Worlds[world.WorldId] = null;
	//	}

	//	public static World GetWorld(int worldId)
	//		=> Worlds[worldId];

	//	public void DestroyWorld()
	//		=> DestroyWorld(this);

	//	public Entity CreateEntity()
	//	{
	//		if (IsDestroyed)
	//			throw new WorldIsDestroyedException(this);

	//		EntityInfo entityInfo;
	//		if (_reuseableEntityInfos.Count > 0)
	//		{
	//			entityInfo = _reuseableEntityInfos.Dequeue();
	//			entityInfo.Generation++;
	//		}
	//		else
	//		{
	//			entityInfo = new EntityInfo
	//			{
	//				Id = _entityInfos.Count,
	//				Generation = 1,
	//				WorldOwner = this,
	//				ComponentIndexes = new int[ComponentIndexes.Count],
	//				ComponentAddedEvent = new EntityComponentChangedEvent(),
	//				ComponentRemovedEvent = new EntityComponentChangedEvent(),
	//				EntityDestroy = new EntityEvent()
	//			};
	//			entityInfo.GetComponents = new DataCache<IComponent[]>(() => UpdateEntityGetComponentsCache(entityInfo));

	//			_entityInfos.Add(entityInfo);
	//		}
	//		entityInfo.IsAlive = true;

	//		entityInfo.ComponentAddedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
	//		entityInfo.ComponentRemovedEvent.Subscribe(OnEntityComponentAddedOrRemoved);
	//		entityInfo.EntityDestroy.Subscribe(OnEntityDestroy);

	//		_entityInfos[entityInfo.Id] = entityInfo;

	//		return new Entity() { Info = entityInfo };
	//	}

	//	public void DestroyEntity(Entity entity)
	//	{
	//		if (IsDestroyed)
	//			throw new WorldIsDestroyedException(this);
	//		if (!HasEntity(entity))
	//			throw new WorldDoesNotHaveEntityException(this, entity);
	//		if (!_entityInfos[entity.Id].IsAlive)
	//			throw new EntityIsNotAliveException(entity);

	//		var entityInfo = _entityInfos[entity.Id];

	//		entityInfo.EntityDestroy.Invoke(entity);

	//		EntityRemoveAllComponents(entity);

	//		entityInfo.IsAlive = false;
	//		entityInfo.ComponentAddedEvent.Clear();
	//		entityInfo.ComponentRemovedEvent.Clear();
	//		entityInfo.EntityDestroy.Clear();

	//		_reuseableEntityInfos.Enqueue(entityInfo);
	//		_entityInfos[entity.Id] = entityInfo;
	//	}

	//	public void DestroyAllEntities()
	//	{
	//		if (IsDestroyed)
	//			throw new WorldIsDestroyedException(this);

	//		var entities = GetEntities();
	//		for (int i = 0; i < entities.Length; i++)
	//			DestroyEntity(entities[i]);
	//	}

	//	public Entity GetEntity(int id)
	//	{
	//		if (IsDestroyed)
	//			throw new WorldIsDestroyedException(this);

	//		if (id <= 0 || id >= _entityInfos.Count)
	//			throw new ArgumentOutOfRangeException();
	//		return new Entity() { Info = _entityInfos[id] };
	//	}

	//	public bool HasEntity(Entity entity)
	//		=> !IsDestroyed && entity.WorldId == WorldId;

	//	public Entity[] GetEntities()
	//		=> IsDestroyed
	//			? new Entity[0]
	//			: _entityInfos // TODO: use cache
	//				.Where(x => x.IsAlive)
	//				.Select(x => new Entity() { Info = x })
	//				.ToArray();

	//	public Group CreateOrGetGroup(Filter filter)
	//	{
	//		if (!_groupLookup.TryGetValue(filter, out Group group))
	//		{
	//			group = new Group(this, filter);
	//			_groupLookup.Add(filter, group);

	//			foreach (var index in group.Indexes)
	//				_groupComponentIndexes[index].Add(group);

	//			foreach (var entity in GetEntities())
	//				group.FilterEntitySilent(entity);
	//		}
	//		return group;
	//	}

	//	public void RemoveGroup(Group group)
	//	{
	//		if (group == null)
	//			throw new ArgumentNullException();
	//		if (!_groupLookup.ContainsKey(group.Filter))
	//			// TODO: throw proper exception
	//			throw new Exception();

	//		_groupLookup.Remove(group.Filter);
	//		foreach (var index in group.Indexes)
	//			_groupComponentIndexes[index].Remove(group);
	//	}

	//	public WorldRecordableData GetRecordableData()
	//	{
	//		/*// This will be replaced with Matcher/Filter
	//		var recordableData = new WorldRecordableData();
	//		var entityConfigs = new List<EntityConfig>();

	//		var entities = GetEntities();
	//		var componentPools = ComponentPools;
	//		var recordableComponentIndexes = ComponentIndexes.RecordableComponentIndexes;
	//		var componentConfigs = new List<ComponentConfig>();
	//		for (int i = 0; i < entities.Length; i++)
	//		{
	//			componentConfigs.Clear();

	//			var entityInfo = EntityInfos[entities[i].Id];
	//			bool hasRecordableComponents = false;
	//			for (int j = 0; j < recordableComponentIndexes.Length; j++)
	//			{
	//				var recordableIndex = recordableComponentIndexes[j];
	//				var componentIndex = entityInfo.ComponentIndexes[recordableIndex];
	//				var componentPool = componentPools[recordableIndex];

	//				if (componentIndex != 0)
	//				{
	//					componentConfigs.Add(new ComponentConfig
	//					{
	//						Name = componentPool.ComponentType.Name,
	//						Component = componentPool.GetComponent(componentIndex)
	//					});
	//					hasRecordableComponents = true;
	//				}
	//			}

	//			if (hasRecordableComponents)
	//			{
	//				entityConfigs.Add(new EntityConfig
	//				{
	//					EntityId = entityInfo.Entity.Id,
	//					ComponentConfigs = componentConfigs.ToArray()
	//				});
	//			}
	//		}

	//		recordableData.EntityConfigs = entityConfigs.ToArray();

	//		return recordableData;*/
	//		return new WorldRecordableData();
	//	}

	//	internal bool EntityHasComponent<TComponent>(Entity entity)
	//		where TComponent : IComponent
	//		=> entity.Info.ComponentIndexes[ComponentIndex<TComponent>.Index] != 0;

	//	internal TComponent EntityAddComponent<TComponent>(Entity entity, TComponent component = default)
	//		where TComponent : IComponent
	//	{
	//		if (EntityHasComponent<TComponent>(entity))
	//			throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

	//		var componentPoolIndex = ComponentIndex<TComponent>.Index;
	//		var componentPool = _componentPools[componentPoolIndex];

	//		var componentIndex = componentPool.AddComponent(component);
	//		entity.Info.ComponentIndexes[componentPoolIndex] = componentIndex;
	//		entity.Info.GetComponents.IsDirty = true;

	//		entity.Info.ComponentAddedEvent.Invoke(entity, componentPoolIndex, component);

	//		return component;
	//	}

	//	internal TComponent EntityGetComponent<TComponent>(Entity entity)
	//		where TComponent : IComponent
	//	{
	//		if (!EntityHasComponent<TComponent>(entity))
	//			throw new EntityNotHaveComponentException(entity, typeof(TComponent));

	//		var componentPoolIndex = ComponentIndex<TComponent>.Index;
	//		var componentPool = _componentPools[componentPoolIndex];
	//		var componentIndex = entity.Info.ComponentIndexes[componentPoolIndex];

	//		return (TComponent)componentPool.GetComponent(componentIndex);
	//	}

	//	internal void EntityRemoveComponent<TComponent>(Entity entity)
	//		where TComponent : IComponent
	//	{
	//		if (!EntityHasComponent<TComponent>(entity))
	//			throw new EntityNotHaveComponentException(entity, typeof(TComponent));

	//		var componentPoolIndex = ComponentIndex<TComponent>.Index;
	//		var componentPool = _componentPools[componentPoolIndex];
	//		var componentIndex = entity.Info.ComponentIndexes[componentPoolIndex];
	//		var component = componentPool.GetComponent(componentIndex);

	//		componentPool.ClearComponent(componentIndex);
	//		entity.Info.ComponentIndexes[componentPoolIndex] = 0;
	//		entity.Info.GetComponents.IsDirty = true;

	//		entity.Info.ComponentRemovedEvent.Invoke(entity, componentPoolIndex, component);
	//	}

	//	internal void EntityReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
	//		where TComponent : IComponent
	//	{
	//		if (!EntityHasComponent<TComponent>(entity))
	//			EntityAddComponent(entity, newComponent);
	//		else
	//		{
	//			var componentPoolIndex = ComponentIndex<TComponent>.Index;
	//			var componentPool = _componentPools[componentPoolIndex];
	//			var componentIndex = entity.Info.ComponentIndexes[componentPoolIndex];
	//			var prevComponent = componentPool.GetComponent(componentIndex);

	//			componentPool.SetComponent(componentIndex, newComponent);
	//			entity.Info.GetComponents.IsDirty = true;

	//			entity.Info.ComponentReplacedEvent.Invoke(entity, componentPoolIndex, prevComponent, newComponent);
	//		}
	//	}

	//	internal IComponent[] EntityGetComponents(Entity entity)
	//		=> entity.Info.GetComponents.Data;

	//	internal void EntityRemoveAllComponents(Entity entity)
	//	{
	//		var componentIndexes = entity.Info.ComponentIndexes;

	//		for (int i = 0; i < _componentPools.Length; i++)
	//		{
	//			var componentIndex = componentIndexes[i];
	//			if (componentIndex != 0)
	//			{
	//				var componentPool = _componentPools[i];
	//				var component = componentPool.GetComponent(componentIndex);

	//				_componentPools[i].ClearComponent(componentIndexes[i]);
	//				entity.Info.ComponentIndexes[i] = 0;

	//				entity.Info.ComponentRemovedEvent.Invoke(entity, i, component);
	//			}
	//		}
	//		entity.Info.GetComponents.IsDirty = true;
	//	}

	//	internal void FinializeDestroy()
	//	{
	//		IsDestroyed = true;
	//	}

	//	private void OnEntityComponentAddedOrRemoved(Entity entity, int componentPoolIndex, IComponent component)
	//	{
	//		foreach (var group in _groupComponentIndexes[componentPoolIndex])
	//			group.FilterEntity(entity, componentPoolIndex, component);
	//	}

	//	private void OnEntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent prevComponent, IComponent newComponent)
	//	{
	//		foreach (var group in _groupComponentIndexes[componentPoolIndex])
	//			group.UpdateEntity(entity, componentPoolIndex, prevComponent, newComponent);
	//	}

	//	private void OnEntityDestroy(Entity entity)
	//	{
	//		// TODO: put group update code
	//	}

	//	private IComponent[] UpdateEntityGetComponentsCache(EntityInfo entityInfo)
	//	{
	//		var components = new List<IComponent>();
	//		for (int i = 0; i < entityInfo.ComponentIndexes.Length; i++)
	//		{
	//			if (entityInfo.ComponentIndexes[i] != 0)
	//				components.Add(_componentPools[i].GetComponent(entityInfo.ComponentIndexes[i]));
	//		}

	//		return components.ToArray();
	//	}
	//}
}