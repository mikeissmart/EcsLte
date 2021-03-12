namespace EcsLte
{
	/*internal class EntityChangedEvent
	{
		private HashSet<Action<World, Entity>> _actions;

		public void Subscribe(Action<World, Entity> action)
		{
			if (_actions == null)
				_actions = new HashSet<Action<World, Entity>>();
			_actions.Add(action);
		}

		public void Unsubscribe(Action<World, Entity> action)
		{
			if (_actions != null)
				_actions.Remove(action);
		}

		public void Invoke(World world, Entity entity)
		{
			if (_actions != null)
			{
				foreach (var action in _actions)
					action.Invoke(world, entity);
			}
		}

		public void Clear()
		{
			if (_actions != null)
				_actions.Clear();
		}
	}*/
}