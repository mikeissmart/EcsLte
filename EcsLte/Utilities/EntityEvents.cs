namespace EcsLte
{
    internal delegate void EntityEvent(Entity entity);
    internal delegate void EntityArrayResize(int newSize);
    internal delegate void EntityComponentChanged(Entity entity, int componentPoolIndex);

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

        internal void Invoke(Entity entity, int componentPoolIndex)
        {
            if (_event != null)
            {
                _event(entity, componentPoolIndex);
            }
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
}