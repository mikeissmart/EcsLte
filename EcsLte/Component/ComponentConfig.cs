using System;

namespace EcsLte
{
    public struct ComponentConfig : IEquatable<ComponentConfig>, IComparable<ComponentConfig>
    {
        public int ComponentIndex { get; set; }
        public int GeneralIndex { get; set; }
        public int ManagedIndex { get; set; }
        public int SharedIndex { get; set; }
        public int UnmanagedSizeInBytes { get; set; }
        public bool IsGeneral { get; set; }
        public bool IsManaged { get; set; }
        public bool IsShared { get; set; }

        public Type ComponentType => ComponentConfigs.Instance.AllComponentTypes[ComponentIndex];

        internal IComponentAdapter Adapter => ComponentConfigs.Instance.AllComponentAdapters[ComponentIndex];

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
                return $"ComponentIndex {ComponentIndex}";
        }
    }

    public class ComponentConfig<TComponent> where TComponent : IComponent
    {
        private static ComponentConfig _config;
        private static bool _hasConfig;

        public static ComponentConfig Config
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