using System;
using System.Collections.Generic;

namespace EcsLte
{
	internal class EntityInfo
	{
		private int[] _componentIndexes;
		private DataCache<IComponent[]> _componentsCache;
		private IComponentPool[] _componentPools;
		private EntityComponentChangedEvent _componentAddedEvent;
		private EntityComponentChangedEvent _componentRemovedEvent;
		private EntityComponentReplacedEvent _componentReplacedEvent;
		private EntityEvent _entityWillBeDestroyedEvent;

		public EntityInfo(int id, int generation, World world, IComponentPool[] componentPools)
		{
			Id = id;
			Generation = generation;
			IsAlive = true;
			World = world;

			_componentAddedEvent = new EntityComponentChangedEvent();
			_componentRemovedEvent = new EntityComponentChangedEvent();
			_componentReplacedEvent = new EntityComponentReplacedEvent();
			_entityWillBeDestroyedEvent = new EntityEvent();

			_componentIndexes = new int[ComponentIndexes.Count];
			_componentsCache = new DataCache<IComponent[]>(UpdateComponentsCache);
			_componentPools = componentPools;

			_componentReplacedEvent.Subscribe((entity, componentPoolIndex, prevComponent, newComponent) =>
			{
				_componentsCache.IsDirty = true;
			});
		}

		public int Id { get; set; }
		public int Generation { get; set; }
		public bool IsAlive { get; set; }
		public World World { get; set; }

		public int[] AllComponentIndexes { get => _componentIndexes; }

		public int this[int componentPoolIndex]
		{
			get => _componentIndexes[componentPoolIndex];
			set
			{
				_componentIndexes[componentPoolIndex] = value;
				_componentsCache.IsDirty = true;
			}
		}

		public IComponent[] GetComponents()
			=> _componentsCache.Data;

		public void Reset()
		{
			Array.Clear(_componentIndexes, 0, _componentIndexes.Length);
			_componentAddedEvent.Clear();
			_componentRemovedEvent.Clear();
			_componentReplacedEvent.Clear();
			_entityWillBeDestroyedEvent.Clear();
			IsAlive = false;
		}

		private IComponent[] UpdateComponentsCache()
		{
			var components = new List<IComponent>();
			for (int i = 0; i < _componentIndexes.Length; i++)
			{
				if (_componentIndexes[i] != 0)
					components.Add(_componentPools[i].GetComponent(_componentIndexes[i]));
			}

			return components.ToArray();
		}
	}
}