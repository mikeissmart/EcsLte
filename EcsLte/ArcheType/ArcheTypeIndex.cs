using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal struct ArcheTypeIndex : IEquatable<ArcheTypeIndex>, IComparable<ArcheTypeIndex>
    {
        public static readonly ArcheTypeIndex Null = new ArcheTypeIndex();

        public bool IsNotNull => this != Null;
        public bool IsNull => this == Null;

        internal int ConfigLength;
        internal int Index;

        internal ArcheTypeIndex(int componentConfigLength, int index)
        {
            ConfigLength = componentConfigLength;
            Index = index;
        }

        #region Equals

        public static bool operator !=(ArcheTypeIndex lhs, ArcheTypeIndex rhs)
            => !(lhs == rhs);

        public static bool operator ==(ArcheTypeIndex lhs, ArcheTypeIndex rhs)
            => lhs.ConfigLength == rhs.ConfigLength &&
                lhs.Index == rhs.Index;

        public bool Equals(ArcheTypeIndex other)
            => this == other;

        public override bool Equals(object obj)
            => obj is ArcheTypeIndex archeTypeIndex && archeTypeIndex == this;

        #endregion

        public int CompareTo(ArcheTypeIndex other)
        {
            var compareTo = ConfigLength.CompareTo(other.ConfigLength);
            if (compareTo == 0)
                compareTo = Index.CompareTo(other.Index);

            return compareTo;
        }

        public override int GetHashCode()
            => HashCodeHelper
                .StartHashCode()
                .AppendHashCode(ConfigLength)
                .AppendHashCode(Index)
                .GetHashCode();

        public override string ToString() => $"{ConfigLength}, {Index}";
    }
}
