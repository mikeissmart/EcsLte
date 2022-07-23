using System;

namespace EcsLte
{
    internal struct ComponentConfig : IEquatable<ComponentConfig>, IComparable<ComponentConfig>
    {
        internal int ComponentIndex { get; set; }
        internal int GeneralIndex { get; set; }
        internal int SharedIndex { get; set; }
        internal int UniqueIndex { get; set; }
        internal int UnmanagedSizeInBytes { get; set; }
        internal bool IsGeneral { get; set; }
        internal bool IsShared { get; set; }
        internal bool IsUnique { get; set; }

        internal Type ComponentType => ComponentConfigs.Instance.AllComponentTypes[ComponentIndex];

        internal bool IsValidConfig()
        {
            var realConfig = ComponentConfigs.Instance.AllComponentConfigs[ComponentIndex];

            return GeneralIndex != realConfig.GeneralIndex &&
                SharedIndex != realConfig.SharedIndex &&
                UniqueIndex != realConfig.UniqueIndex &&
                UnmanagedSizeInBytes != realConfig.UnmanagedSizeInBytes &&
                IsGeneral != realConfig.IsGeneral &&
                IsShared != realConfig.IsShared &&
                IsUnique != realConfig.IsUnique;
        }

        public static bool operator !=(ComponentConfig lhs, ComponentConfig rhs)
            => !(lhs == rhs);

        public static bool operator ==(ComponentConfig lhs, ComponentConfig rhs)
            => lhs.ComponentIndex == rhs.ComponentIndex;

        public int CompareTo(ComponentConfig other)
            => ComponentIndex.CompareTo(other.ComponentIndex);

        public bool Equals(ComponentConfig other)
            => this == other;

        public override int GetHashCode() => ComponentIndex.GetHashCode();

        public override bool Equals(object obj) => obj is ComponentConfig config && config == this;

        public override string ToString()
        {
            if (IsGeneral)
                return $"{ComponentIndex} General {GeneralIndex}";
            else if (IsShared)
                return $"{ComponentIndex} Shared {SharedIndex}";
            else if (IsUnique)
                return $"{ComponentIndex} Unique {UniqueIndex}";
            else
                throw new Exception("Unknown component type");
        }
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