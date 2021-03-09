using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public class World
	{
		private Queue<EntityInfo> _reuseableEntityInfos;
		private List<EntityInfo> _entityInfos;

		internal World()
		{
			_entityInfos = new List<EntityInfo>();
			_reuseableEntityInfos = new Queue<EntityInfo>();
			ComponentPools = new IComponentPool[ComponentIndexes.Count];

			// Create place holder for null entity
			_entityInfos.Add(new EntityInfo());
			for (int i = 0; i < ComponentIndexes.Count; i++)
			{
				ComponentPools[i] = (IComponentPool)Activator
					.CreateInstance(typeof(ComponentPool<>).MakeGenericType(ComponentIndexes.AllComponentTypes[i]));
			}
		}

		public static World DefaultWorld { get; set; } = CreateWorld();

		public bool IsDestroyed { get; private set; }
		public int WorldId { get; internal set; }

		internal static List<World> Worlds { get; private set; }

		internal IComponentPool[] ComponentPools { get; private set; }

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

		public static World CreateWorld(WorldRecordableData recordableData)
		{
			// TODO: code, entities and components with entity props will need to be remapped
			return CreateWorld();
		}

		public static void DestroyWorld(World world)
		{
			if (world == null)
				throw new ArgumentNullException();

			if (world.IsDestroyed)
				throw new WorldIsDestroyedException(world);

			world.FinializeDestroy();

			Worlds[world.WorldId] = null;
		}

		public static World GetWorld(int worldId)
			=> Worlds[worldId];

		public Entity CreateEntity()
		{
			if (IsDestroyed)
				throw new WorldIsDestroyedException(this);

			EntityInfo entityInfo;
			if (_reuseableEntityInfos.Count > 0)
			{
				entityInfo = _reuseableEntityInfos.Dequeue();
				entityInfo.Generation++;
			}
			else
			{
				entityInfo = new EntityInfo
				{
					Id = _entityInfos.Count,
					Generation = 1,
					WorldOwner = this,
					ComponentIndexes = new int[ComponentIndexes.Count]
				};

				_entityInfos.Add(entityInfo);
			}
			entityInfo.IsAlive = true;

			_entityInfos[entityInfo.Id] = entityInfo;

			return new Entity() { Info = entityInfo };
		}

		public void DestroyEntity(Entity entity)
		{
			if (IsDestroyed)
				throw new WorldIsDestroyedException(this);
			if (!HasEntity(entity))
				throw new WorldDoesNotHaveEntityException(this, entity);
			if (!_entityInfos[entity.Id].IsAlive)
				throw new EntityIsNotAliveException(entity);

			entity.RemoveComponents();

			var entityInfo = _entityInfos[entity.Id];

			entityInfo.IsAlive = false;

			_reuseableEntityInfos.Enqueue(entityInfo);
			_entityInfos[entity.Id] = entityInfo;
		}

		public void DestroyAllEntities()
		{
			if (IsDestroyed)
				throw new WorldIsDestroyedException(this);

			var entities = GetEntities();
			for (int i = 0; i < entities.Length; i++)
				DestroyEntity(entities[i]);
		}

		public Entity GetEntity(int id)
		{
			if (IsDestroyed)
				throw new WorldIsDestroyedException(this);

			if (id <= 0 || id >= _entityInfos.Count)
				throw new ArgumentOutOfRangeException();
			return new Entity() { Info = _entityInfos[id] };
		}

		public bool HasEntity(Entity entity)
			=> !IsDestroyed && entity.WorldId == WorldId;

		public Entity[] GetEntities()
			=> IsDestroyed
				? new Entity[0]
				: _entityInfos
					.Where(x => x.IsAlive)
					.Select(x => new Entity() { Info = x })
					.ToArray();

		public void DestroyWorld()
			=> DestroyWorld(this);

		public WorldRecordableData GetRecordableData()
		{
			/*// This will be replaced with Matcher/Filter
			var recordableData = new WorldRecordableData();
			var entityConfigs = new List<EntityConfig>();

			var entities = GetEntities();
			var componentPools = ComponentPools;
			var recordableComponentIndexes = ComponentIndexes.RecordableComponentIndexes;
			var componentConfigs = new List<ComponentConfig>();
			for (int i = 0; i < entities.Length; i++)
			{
				componentConfigs.Clear();

				var entityInfo = EntityInfos[entities[i].Id];
				bool hasRecordableComponents = false;
				for (int j = 0; j < recordableComponentIndexes.Length; j++)
				{
					var recordableIndex = recordableComponentIndexes[j];
					var componentIndex = entityInfo.ComponentIndexes[recordableIndex];
					var componentPool = componentPools[recordableIndex];

					if (componentIndex != 0)
					{
						componentConfigs.Add(new ComponentConfig
						{
							Name = componentPool.ComponentType.Name,
							Component = componentPool.GetComponent(componentIndex)
						});
						hasRecordableComponents = true;
					}
				}

				if (hasRecordableComponents)
				{
					entityConfigs.Add(new EntityConfig
					{
						EntityId = entityInfo.Entity.Id,
						ComponentConfigs = componentConfigs.ToArray()
					});
				}
			}

			recordableData.EntityConfigs = entityConfigs.ToArray();

			return recordableData;*/
			return new WorldRecordableData();
		}

		internal void FinializeDestroy()
		{
			IsDestroyed = true;
		}
	}
}