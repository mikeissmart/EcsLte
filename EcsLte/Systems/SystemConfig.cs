using System;

namespace EcsLte
{
    internal class SystemConfig : IEquatable<SystemConfig>, IComparable<SystemConfig>
    {
        internal int SystemIndex { get; set; }
        internal bool IsAutoAdd { get; set; }
        internal Type SystemType => SystemConfigs.AllSystemTypes[SystemIndex];

        #region Equals

        public static bool operator !=(SystemConfig lhs, SystemConfig rhs)
            => !(lhs == rhs);

        public static bool operator ==(SystemConfig lhs, SystemConfig rhs)
            => lhs.SystemIndex == rhs.SystemIndex;

        public int CompareTo(SystemConfig other)
            => SystemIndex.CompareTo(other.SystemIndex);

        public bool Equals(SystemConfig other)
            => this == other;

        public override bool Equals(object other)
            => other is SystemConfig obj && this == obj;

        #endregion

        public override int GetHashCode() => SystemIndex.GetHashCode();
    }

    internal class SystemConfig<TSystem> where TSystem : SystemBase
    {
        private static SystemConfig _config;
        private static bool _hasConfig;

        internal static SystemConfig Config
        {
            get
            {
                if (!_hasConfig)
                {
                    _config = SystemConfigs.GetConfig(typeof(TSystem));
                    _hasConfig = true;
                }

                return _config;
            }
        }
    }
}
