using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public struct ChangeVersion : IEquatable<ChangeVersion>, IComparable<ChangeVersion>
    {
        public static readonly ChangeVersion New = new ChangeVersion();

        /// <summary>
        /// Ex param1 > param2 || param2 == 0
        /// </summary>
        /// <param name="changeVersion"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static bool DidChange(ChangeVersion changeVersion, ChangeVersion required)
            => required.Version == 0 || changeVersion.Version > required.Version;

        public static void IncVersion(ref ChangeVersion changeVersion)
            => changeVersion.Version++;

        public ulong Version { get; set; }

        #region Equals

        public static bool operator !=(ChangeVersion lhs, ChangeVersion rhs)
            => !(lhs == rhs);

        public static bool operator ==(ChangeVersion lhs, ChangeVersion rhs)
            => lhs.Version == rhs.Version;

        public bool Equals(ChangeVersion other)
            => this == other;

        public override bool Equals(object obj)
            => obj is ChangeVersion changeVersion && changeVersion == this;

        #endregion

        public int CompareTo(ChangeVersion other)
            => Version.CompareTo(other.Version);

        public override int GetHashCode()
            => Version.GetHashCode();

        public override string ToString()
            => Version.ToString();
    }
}
