using System;
using System.Collections.Generic;

namespace EcsLte
{
	public interface IComponentPool
	{
		Type ComponentType { get; }
		int Length { get; }

		int AddComponent();

		int AddComponent(IComponent component);

		void SetComponent(int index, IComponent component);

		IComponent GetComponent(int index);

		void ClearComponent(int index);

		void ClearAll();
	}

	public class ComponentPool<TComponent> : IComponentPool
		where TComponent : IComponent
	{
		private List<TComponent> _components;
		private Queue<int> _unusedIndexes;

		public ComponentPool()
		{
			_components = new List<TComponent>();
			_unusedIndexes = new Queue<int>();
			ComponentType = typeof(TComponent);
			AddComponent();
		}

		public Type ComponentType { get; private set; }
		public int Length { get => _components.Count; }

		public int AddComponent()
		{
			int index;
			if (_unusedIndexes.Count > 0)
				index = _unusedIndexes.Dequeue();
			else
			{
				index = _components.Count;
				_components.Add(Activator.CreateInstance<TComponent>());
			}

			return index;
		}

		public int AddComponent(IComponent component)
		{
			int index = AddComponent();
			if (component != default)
				_components[index] = (TComponent)component;
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
			AddComponent();
		}
	}
}