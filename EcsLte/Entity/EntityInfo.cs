using System;
using System.Collections.Generic;

namespace EcsLte
{
	internal class EntityInfo
	{
		private static List<IComponent> _componentsBuffer = new List<IComponent>();

		private IComponent[] _components;
		private DataCache<IComponent[]> _componentsCache;
		private EntityComponentChangedEvent _componentAddedEvent;
		private EntityComponentChangedEvent _componentRemovedEvent;
		private EntityComponentReplacedEvent _componentReplacedEvent;

		public EntityInfo(int id, int generation, World world)
		{
			Id = id;
			Generation = generation;
			IsAlive = true;
			World = world;

			_componentAddedEvent = new EntityComponentChangedEvent();
			_componentRemovedEvent = new EntityComponentChangedEvent();
			_componentReplacedEvent = new EntityComponentReplacedEvent();

			EntityWillBeDestroyedEvent = new EntityEvent();

			_components = new IComponent[ComponentIndexes.Instance.Count];
			_componentsCache = new DataCache<IComponent[]>(UpdateComponentsCache);

			_componentReplacedEvent.Subscribe((entity, componentPoolIndex, prevComponent, newComponent) =>
			{
				_componentsCache.IsDirty = true;
			});
		}

		public int Id { get; set; }
		public int Generation { get; set; }
		public bool IsAlive { get; set; }
		public World World { get; set; }

		private EntityEvent EntityWillBeDestroyedEvent { get; set; }

		public IComponent this[int componentIndex]
		{
			get => _components[componentIndex];
			set
			{
				_components[componentIndex] = value;
				_componentsCache.IsDirty = true;
			}
		}

		public IComponent[] GetComponents()
			=> _componentsCache.Data;

		public void Reset()
		{
			Array.Clear(_components, 0, _components.Length);
			_componentAddedEvent.Clear();
			_componentRemovedEvent.Clear();
			_componentReplacedEvent.Clear();
			EntityWillBeDestroyedEvent.Clear();
			IsAlive = false;
		}

		private IComponent[] UpdateComponentsCache()
		{
			_componentsBuffer.Clear();
			for (int i = 0; i < _components.Length; i++)
			{
				if (_components[i] != null)
				{
					_componentsBuffer.Add(_components[i]);
				}
			}

			return _componentsBuffer.ToArray();
		}
	}
}