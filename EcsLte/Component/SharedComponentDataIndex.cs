using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal struct SharedComponentDataIndex : IEquatable<SharedComponentDataIndex>, IComparable<SharedComponentDataIndex>
    {
        internal int SharedIndex { get; set; }
        internal int SharedDataIndex { get; set; }

        public static bool operator !=(SharedComponentDataIndex lhs, SharedComponentDataIndex rhs)
            => !(lhs == rhs);

        public static bool operator ==(SharedComponentDataIndex lhs, SharedComponentDataIndex rhs)
            => lhs.SharedIndex == rhs.SharedIndex && lhs.SharedDataIndex == rhs.SharedDataIndex;

        public bool Equals(SharedComponentDataIndex other)
            => this == other;

        public override bool Equals(object other)
            => other is SharedComponentDataIndex obj && this == obj;

        public override int GetHashCode() => HashCodeHelper.StartHashCode()
                .AppendHashCode(SharedIndex)
                .AppendHashCode(SharedDataIndex)
                .HashCode;

        public int CompareTo(SharedComponentDataIndex other)
        {
            var compare = SharedIndex.CompareTo(other.SharedIndex);
            if (compare == 0)
                compare = SharedDataIndex.CompareTo(other.SharedDataIndex);
            return compare;
        }
    }
}
