using System;

namespace EcsLte
{
    internal interface IEntityQuery_SharedComponentData : IEquatable<IEntityQuery_SharedComponentData>, IComparable<IEntityQuery_SharedComponentData>
    {
        ISharedComponent Component { get; }
        ComponentConfig Config { get; }
        bool IsEqual(IComponent component);
        int GetHashCode();
    }

    internal class EntityQuery_SharedComponentData<TSharedComponentData> : IEntityQuery_SharedComponentData where TSharedComponentData : ISharedComponent
    {
        private readonly TSharedComponentData _component;

        public ISharedComponent Component => _component;
        public ComponentConfig Config { get; private set; }

        public EntityQuery_SharedComponentData(ComponentConfig config, TSharedComponentData component)
        {
            _component = component;
            Config = config;
        }

        public bool IsEqual(IComponent component)
        {
            if (component is TSharedComponentData sharedComponentData)
                return _component.Equals(sharedComponentData);
            return false;
        }

        public int CompareTo(IEntityQuery_SharedComponentData other) => Config.CompareTo(other.Config);

        public static bool operator !=(IEntityQuery_SharedComponentData lhs, EntityQuery_SharedComponentData<TSharedComponentData> rhs) => !(lhs == rhs);

        public static bool operator !=(EntityQuery_SharedComponentData<TSharedComponentData> lhs, IEntityQuery_SharedComponentData rhs) => !(lhs == rhs);

        public static bool operator ==(IEntityQuery_SharedComponentData lhs, EntityQuery_SharedComponentData<TSharedComponentData> rhs) => lhs.Config == rhs.Config &&
            lhs.IsEqual(rhs.Component);

        public static bool operator ==(EntityQuery_SharedComponentData<TSharedComponentData> rhs, IEntityQuery_SharedComponentData lhs) => lhs.Config == rhs.Config &&
            lhs.IsEqual(rhs.Component);

        public bool Equals(IEntityQuery_SharedComponentData other) => this == other;

        public override bool Equals(object other) => other is IEntityQuery_SharedComponentData obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = -612338121;
            hashCode = hashCode * -1521134295 + Config.GetHashCode();
            hashCode = hashCode * -1521134295 + Component.GetHashCode();
            return hashCode;
        }
    }
}
