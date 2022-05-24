using System;

namespace EcsLte
{
    internal struct ComponentConfig : IEquatable<ComponentConfig>, IComparable<ComponentConfig>
    {
        internal int ComponentIndex { get; set; }
        internal int RecordableIndex { get; set; }
        internal int UniqueIndex { get; set; }
        internal int SharedIndex { get; set; }
        internal int BlittableIndex { get; set; }
        internal int ManagedIndex { get; set; }
        internal int UnmanagedSizeInBytes { get; set; }
        internal bool IsRecordable { get; set; }
        internal bool IsUnique { get; set; }
        internal bool IsShared { get; set; }
        internal bool IsBlittable { get; set; }
        internal bool IsManaged { get; set; }

        public static bool operator !=(ComponentConfig lhs, ComponentConfig rhs)
            => !(lhs == rhs);

        public static bool operator ==(ComponentConfig lhs, ComponentConfig rhs)
            => lhs.ComponentIndex == rhs.ComponentIndex;

        public int CompareTo(ComponentConfig other)
            => ComponentIndex.CompareTo(other.ComponentIndex);

        public bool Equals(ComponentConfig other)
            => this == other;

        public override bool Equals(object other)
            => other is ComponentConfig obj && this == obj;

        public override int GetHashCode() => ComponentIndex.GetHashCode();
    }

    internal class ComponentConfig<TComponent> where TComponent : IComponent
    {
        private static ComponentConfig _config;
        private static bool _hasConfig;

        internal static ComponentConfig Config
        {
            get
            {
                if (!_hasConfig)
                {
                    _config = ComponentConfigs.Instance.GetConfig(typeof(TComponent));
                    _hasConfig = true;
                }

                return _config;
            }
        }
    }
}