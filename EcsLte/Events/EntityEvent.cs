using System;
using System.Collections.Generic;

namespace EcsLte.Events
{
    internal class EntityEvent
    {
        private HashSet<Action<Entity>> _actions;

        public void Subscribe(Action<Entity> action)
        {
            if (_actions == null)
                _actions = new HashSet<Action<Entity>>();
            _actions.Add(action);
        }

        public void Unsubscribe(Action<Entity> action)
        {
            if (_actions != null)
                _actions.Remove(action);
        }

        public void Invoke(Entity entity)
        {
            if (_actions != null)
                foreach (var action in _actions)
                    action.Invoke(entity);
        }

        public void Clear()
        {
            if (_actions != null)
                _actions.Clear();
        }
    }
}