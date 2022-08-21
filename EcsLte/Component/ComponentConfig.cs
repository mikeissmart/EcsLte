using System;

namespace EcsLte
{
    internal struct ComponentConfig : IEquatable<ComponentConfig>, IComparable<ComponentConfig>
    {
        internal int ComponentIndex { get; set; }
        internal int GeneralIndex { get; set; }
        internal int ManagedIndex { get; set; }
        internal int SharedIndex { get; set; }
        internal int UnmanagedSizeInBytes { get; set; }
        internal bool IsGeneral { get; set; }
        internal bool IsManaged { get; set; }
        internal bool IsShared { get; set; }

        internal Type ComponentType => ComponentConfigs.AllComponentTypes[ComponentIndex];

        internal IComponentAdapter Adapter => ComponentConfigs.AllComponentAdapters[ComponentIndex];

        #region Equals

        public static bool operator !=(ComponentConfig lhs, ComponentConfig rhs)
            => !(lhs == rhs);

        public static bool operator ==(ComponentConfig lhs, ComponentConfig rhs) => lhs.ComponentIndex == rhs.ComponentIndex &&
                lhs.UnmanagedSizeInBytes == rhs.UnmanagedSizeInBytes &&
                lhs.IsGeneral == rhs.IsGeneral &&
                lhs.IsManaged == rhs.IsManaged &&
                lhs.IsShared == rhs.IsShared;

        public bool Equals(ComponentConfig other)
            => this == other;

        public override bool Equals(object obj) => obj is ComponentConfig config && config == this;

        #endregion

        public int CompareTo(ComponentConfig other)
            => ComponentIndex.CompareTo(other.ComponentIndex);

        public override int GetHashCode() => ComponentIndex.GetHashCode();

        public override string ToString()
        {
            if (IsGeneral)
                return $"ComponentIndex {ComponentIndex}, GeneralIndex {GeneralIndex}";
            else if (IsManaged)
                return $"ComponentIndex {ComponentIndex}, ManagedIndex {ManagedIndex}";
            else if (IsShared)
                return $"ComponentIndex {ComponentIndex}, SharedIndex {SharedIndex}";
            else
                throw new Exception();
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
                    _config = ComponentConfigs.GetConfig(typeof(TComponent));
                    _hasConfig = true;
                }

                return _config;
            }
        }
    }
}