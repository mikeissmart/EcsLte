using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal struct ArcheTypeIndex : IEquatable<ArcheTypeIndex>, IComparable<ArcheTypeIndex>
    {
        internal int ComponentConfigLength { get; set; }
        internal int Index { get; set; }

        internal ArcheTypeIndex(int componentConfigLength, int index)
        {
            ComponentConfigLength = componentConfigLength;
            Index = index;
        }

        public static bool operator !=(ArcheTypeIndex lhs, ArcheTypeIndex rhs)
            => !(lhs == rhs);

        public static bool operator ==(ArcheTypeIndex lhs, ArcheTypeIndex rhs)
            => lhs.ComponentConfigLength == rhs.ComponentConfigLength &&
                lhs.Index == rhs.Index;

        public int CompareTo(ArcheTypeIndex other)
        {
            var compareTo = ComponentConfigLength.CompareTo(other.ComponentConfigLength);
            if (compareTo == 0)
                compareTo = Index.CompareTo(other.Index);

            return compareTo;
        }

        public bool Equals(ArcheTypeIndex other)
            => this == other;

        public override int GetHashCode() => HashCodeHelper
            .StartHashCode()
            .AppendHashCode(ComponentConfigLength)
            .AppendHashCode(Index)
            .GetHashCode();

        public override bool Equals(object obj) => obj is ArcheTypeIndex archeTypeIndex && archeTypeIndex == this;

        public override string ToString() => $"{ComponentConfigLength}, {Index}";
    }
}