using System;

namespace EcsLte
{
    internal struct SystemConfig : IEquatable<SystemConfig>, IComparable<SystemConfig>
    {
        public int SystemIndex { get; set; }
        public int InitializeIndex { get; set; }
        public int ExecuteIndex { get; set; }
        public int CleanupIndex { get; set; }
        public int TearDownIndex { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsExecute { get; set; }
        public bool IsCleanup { get; set; }
        public bool IsTearDown { get; set; }
        public bool IsAutoAdd { get; set; }

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
                    _config = SystemConfigs.Instance.GetConfig(typeof(TSystem));
                    _hasConfig = true;
                }

                return _config;
            }
        }
    }
}
