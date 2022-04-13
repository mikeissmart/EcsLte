using System;

namespace EcsLte
{
    public struct SharedComponentDataIndex : IEquatable<SharedComponentDataIndex>, IComparable<SharedComponentDataIndex>
    {
        public int SharedIndex { get; set; }
        public int SharedDataIndex { get; set; }

        public static bool operator !=(SharedComponentDataIndex lhs, SharedComponentDataIndex rhs) => !(lhs == rhs);

        public static bool operator ==(SharedComponentDataIndex lhs, SharedComponentDataIndex rhs) => lhs.SharedIndex == rhs.SharedIndex &&
                lhs.SharedDataIndex == rhs.SharedDataIndex;

        public bool Equals(SharedComponentDataIndex other)
            => this == other;

        public override bool Equals(object other)
            => other is SharedComponentDataIndex obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = 1193469065;
            hashCode = hashCode * -1521134295 + SharedIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + SharedDataIndex.GetHashCode();
            return hashCode;
        }

        public int CompareTo(SharedComponentDataIndex other)
        {
            var compare = SharedIndex.CompareTo(other.SharedIndex);
            if (compare == 0)
                compare = SharedDataIndex.CompareTo(other.SharedDataIndex);
            return compare;
        }
    }
}
