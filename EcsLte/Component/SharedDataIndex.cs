using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal struct SharedDataIndex : IEquatable<SharedDataIndex>, IComparable<SharedDataIndex>
    {
        internal int SharedIndex;
        internal int DataIndex;

        #region Equals

        public static bool operator !=(SharedDataIndex lhs, SharedDataIndex rhs)
            => !(lhs == rhs);

        public static bool operator ==(SharedDataIndex lhs, SharedDataIndex rhs)
            => lhs.SharedIndex == rhs.SharedIndex
                && lhs.DataIndex == rhs.DataIndex;

        public bool Equals(SharedDataIndex other)
            => this == other;

        public override bool Equals(object other)
            => other is SharedDataIndex obj && this == obj;

        #endregion

        public int CompareTo(SharedDataIndex other)
        {
            var compare = SharedIndex.CompareTo(other.SharedIndex);
            if (compare == 0)
                compare = DataIndex.CompareTo(other.DataIndex);
            return compare;
        }

        public override int GetHashCode()
            => HashCodeHelper.StartHashCode()
                .AppendHashCode(SharedIndex)
                .AppendHashCode(DataIndex)
                .HashCode;

        public override string ToString() => $"{ComponentConfigs.AllSharedConfigs[SharedIndex]}, DataIndex: {DataIndex}";
    }
}
