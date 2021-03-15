using System;
using System.Collections.Generic;

namespace EcsLte
{
	internal class GroupEvent
	{
		private HashSet<Action<Group>> _actions;

		public void Subscribe(Action<Group> action)
		{
			if (_actions == null)
				_actions = new HashSet<Action<Group>>();
			_actions.Add(action);
		}

		public void Unsubscribe(Action<Group> action)
		{
			if (_actions != null)
				_actions.Remove(action);
		}

		public void Invoke(Group group)
		{
			if (_actions != null)
			{
				foreach (var action in _actions)
					action.Invoke(group);
			}
		}

		public void Clear()
		{
			if (_actions != null)
				_actions.Clear();
		}
	}
}