using System;
using System.Collections.Generic;

namespace EcsLte
{
	internal class GroupChangedEvent
	{
		private HashSet<Action<Entity, int, IComponent>> _actions;

		public void Subscribe(Action<Entity, int, IComponent> action)
		{
			if (_actions == null)
				_actions = new HashSet<Action<Entity, int, IComponent>>();
			_actions.Add(action);
		}

		public void Unsubscribe(Action<Entity, int, IComponent> action)
		{
			if (_actions != null)
				_actions.Remove(action);
		}

		public void Invoke(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (_actions != null)
			{
				foreach (var action in _actions)
					action.Invoke(entity, componentPoolIndex, component);
			}
		}

		public void Clear()
		{
			if (_actions != null)
				_actions.Clear();
		}
	}
}