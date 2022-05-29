using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal struct ArcheTypeIndex : IEquatable<ArcheTypeIndex>, IComparable<ArcheTypeIndex>
    {
        public int ComponentConfigLength { get; set; }
        public int Index { get; set; }

        public static bool operator !=(ArcheTypeIndex lhs, ArcheTypeIndex rhs)
            => !(lhs == rhs);

        public static bool operator ==(ArcheTypeIndex lhs, ArcheTypeIndex rhs)
            => lhs.ComponentConfigLength == rhs.ComponentConfigLength &&
                lhs.Index == rhs.Index;

        public int CompareTo(ArcheTypeIndex other)
        {
            var compareTo = ComponentConfigLength.CompareTo(other.ComponentConfigLength);
            if (compareTo == 0)
                Index.CompareTo(other.Index);

            return compareTo;
        }

        public bool Equals(ArcheTypeIndex other)
            => this == other;

        public override int GetHashCode() => HashCodeHelper
            .StartHashCode()
            .AppendHashCode(ComponentConfigLength)
            .AppendHashCode(Index)
            .GetHashCode();
    }
}