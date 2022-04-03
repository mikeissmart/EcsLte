using System;

namespace EcsLte.NativeArcheType
{
    public struct ArcheTypeIndex_ArcheType_Native : IEquatable<ArcheTypeIndex_ArcheType_Native>, IComparable<ArcheTypeIndex_ArcheType_Native>
    {
        public static readonly ArcheTypeIndex_ArcheType_Native Null = new ArcheTypeIndex_ArcheType_Native();

        public bool IsNotNull => this != Null;
        public bool IsNull => this == Null;

        public int ComponentsLength { get; set; }
        public int Index { get; set; }

        public static bool operator !=(ArcheTypeIndex_ArcheType_Native lhs, ArcheTypeIndex_ArcheType_Native rhs) => !(lhs == rhs);

        public static bool operator ==(ArcheTypeIndex_ArcheType_Native lhs, ArcheTypeIndex_ArcheType_Native rhs) => lhs.ComponentsLength == rhs.ComponentsLength && lhs.Index == rhs.Index;

        public int CompareTo(ArcheTypeIndex_ArcheType_Native other)
        {
            var compare = ComponentsLength.CompareTo(other.ComponentsLength);
            if (compare == 0)
                compare = Index.CompareTo(Index);
            return compare;
        }

        public bool Equals(ArcheTypeIndex_ArcheType_Native other) => this == other;

        public override bool Equals(object other) => other is ArcheTypeIndex_ArcheType_Native obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = -612338121;
            hashCode = hashCode * -1521134295 + ComponentsLength.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }
    }
}
