using System;

namespace EcsLte
{
    internal interface IEntityQueryFilterComponent : IComparable<IEntityQueryFilterComponent>
    {
        ISharedComponent Component { get; }
        ComponentConfig Config { get; }
        bool IsEqual(IComponent component);
        int GetHashCode();
    }

    internal class EntityQueryFilterComponent<TComponent> : IEntityQueryFilterComponent
        where TComponent : ISharedComponent
    {
        private readonly TComponent _component;

        public ISharedComponent Component => _component;
        public ComponentConfig Config { get; private set; }

        public EntityQueryFilterComponent(TComponent component)
        {
            _component = component;
            Config = ComponentConfig<TComponent>.Config;
        }

        public bool IsEqual(IComponent component)
        {
            if (component is TComponent sharedComponent)
                return _component.Equals(sharedComponent);
            return false;
        }

        public int CompareTo(IEntityQueryFilterComponent other) => Config.CompareTo(other.Config);

        public static bool operator !=(IEntityQueryFilterComponent lhs, EntityQueryFilterComponent<TComponent> rhs)
            => !(lhs == rhs);

        public static bool operator !=(EntityQueryFilterComponent<TComponent> lhs, IEntityQueryFilterComponent rhs)
            => !(lhs == rhs);

        public static bool operator ==(IEntityQueryFilterComponent lhs, EntityQueryFilterComponent<TComponent> rhs)
            => lhs.Config == rhs.Config;

        public static bool operator ==(EntityQueryFilterComponent<TComponent> rhs, IEntityQueryFilterComponent lhs)
            => lhs.Config == rhs.Config;

        public bool Equals(IEntityQueryFilterComponent other) => this == other;

        public override bool Equals(object other) => other is IEntityQueryFilterComponent obj && this == obj;

        public override int GetHashCode() => Config.GetHashCode();
    }
}
