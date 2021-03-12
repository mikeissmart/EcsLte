namespace EcsLte
{
	/*internal class WorldGroupChangedEvent
	{
		private HashSet<Action<World, Group>> _actions;

		public void Subscribe(Action<World, Group> action)
		{
			if (_actions == null)
				_actions = new HashSet<Action<World, Group>>();
			_actions.Add(action);
		}

		public void Unsubscribe(Action<World, Group> action)
		{
			if (_actions != null)
				_actions.Remove(action);
		}

		public void Invoke(World world, Group group)
		{
			if (_actions != null)
			{
				foreach (var action in _actions)
					action.Invoke(world, group);
			}
		}

		public void Clear()
		{
			if (_actions != null)
				_actions.Clear();
		}
	}*/
}