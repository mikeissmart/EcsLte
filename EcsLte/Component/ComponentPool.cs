using System;
using System.Collections.Generic;

namespace EcsLte
{
	public interface IComponentPool
	{
		int Length { get; }

		int AddComponent(IComponent component = null);

		void SetComponent(int index, IComponent component);

		IComponent GetComponent(int index);

		void ClearComponent(int index);

		void ClearAll();
	}

	public class ComponentPool<TComponent> : IComponentPool
		where TComponent : IComponent
	{
		private readonly List<TComponent> _components;
		private readonly Queue<int> _unusedIndexes;

		public ComponentPool()
		{
			_components = new List<TComponent>();
			_unusedIndexes = new Queue<int>();
			AddComponent(Activator.CreateInstance<TComponent>());
		}

		public int Length { get => _components.Count; }

		public int AddComponent(IComponent component)
		{
			int index;
			if (_unusedIndexes.Count > 0)
			{
				index = _unusedIndexes.Dequeue();
				_components[index] = (TComponent)component;
			}
			else
			{
				index = _components.Count;
				_components.Add((TComponent)component);
			}

			return index;
		}

		public void SetComponent(int index, IComponent component)
			=> _components[index] = (TComponent)component;

		public IComponent GetComponent(int index)
			=> _components[index];

		public void ClearComponent(int index)
			=> _components[index] = default;

		public void ClearAll()
		{
			_unusedIndexes.Clear();
			_components.Clear();
			AddComponent(Activator.CreateInstance<TComponent>());
		}
	}
}