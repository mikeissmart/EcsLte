namespace EcsLte
{
    internal delegate void EntityEvent(Entity entity);
    internal delegate void EntityComponentChanged(Entity entity, int componentPoolIndex, IComponent component);
    internal delegate void EntityComponentReplaced(Entity entity, int componentPoolIndex, IComponent oldComponent, IComponent newComponent);

    internal struct EntityEventHandler
    {
        private event EntityEvent _event;

        internal void Invoke(Entity entity)
        {
            if (_event != null)
            {
                _event(entity);
            }
        }

        internal bool HasSubscriptions { get => _event != null; }

        internal void Clear()
        {
            _event = null;
        }

        public static EntityEventHandler operator +(EntityEventHandler lhs, EntityEvent rhs)
        {
            lhs._event += rhs;
            return lhs;
        }

        public static EntityEventHandler operator -(EntityEventHandler lhs, EntityEvent rhs)
        {
            lhs._event -= rhs;
            return lhs;
        }
    }

    internal struct EntityComponentChangedHandler
    {
        private event EntityComponentChanged _event;

        internal void Invoke(Entity entity, int componentPoolIndex, IComponent component)
        {
            if (_event != null)
            {
                _event(entity, componentPoolIndex, component);
            }
        }

        internal bool HasSubscriptions { get => _event != null; }

        internal void Clear()
        {
            _event = null;
        }

        public static EntityComponentChangedHandler operator +(EntityComponentChangedHandler lhs, EntityComponentChanged rhs)
        {
            lhs._event += rhs;
            return lhs;
        }

        public static EntityComponentChangedHandler operator -(EntityComponentChangedHandler lhs, EntityComponentChanged rhs)
        {
            lhs._event -= rhs;
            return lhs;
        }
    }

    internal struct EntityComponentReplacedHandler
    {
        private event EntityComponentReplaced _event;

        internal void Invoke(Entity entity, int componentPoolIndex, IComponent oldComponent, IComponent newComponent)
        {
            if (_event != null)
            {
                _event(entity, componentPoolIndex, oldComponent, newComponent);
            }
        }

        internal bool HasSubscriptions { get => _event != null; }

        internal void Clear()
        {
            _event = null;
        }

        public static EntityComponentReplacedHandler operator +(EntityComponentReplacedHandler lhs, EntityComponentReplaced rhs)
        {
            lhs._event += rhs;
            return lhs;
        }

        public static EntityComponentReplacedHandler operator -(EntityComponentReplacedHandler lhs, EntityComponentReplaced rhs)
        {
            lhs._event -= rhs;
            return lhs;
        }
    }

}