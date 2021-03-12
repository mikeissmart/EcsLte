using System;
using System.Collections.Generic;

namespace EcsLte
{
	internal class EntityComponentReplacedEvent
	{
		private HashSet<Action<Entity, int, IComponent, IComponent>> _actions;

		public void Subscribe(Action<Entity, int, IComponent, IComponent> action)
		{
			if (_actions == null)
				_actions = new HashSet<Action<Entity, int, IComponent, IComponent>>();
			_actions.Add(action);
		}

		public void Unsubscribe(Action<Entity, int, IComponent, IComponent> action)
		{
			if (_actions != null)
				_actions.Remove(action);
		}

		public void Invoke(Entity entity, int componentPoolIndex, IComponent prevComponent, IComponent newComponent)
		{
			if (_actions != null)
			{
				foreach (var action in _actions)
					action.Invoke(entity, componentPoolIndex, prevComponent, newComponent);
			}
		}

		public void Clear()
		{
			if (_actions != null)
				_actions.Clear();
		}
	}
}