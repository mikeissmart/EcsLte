using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal class ArcheTypeEqualityComparer : IEqualityComparer<ArcheType>
    {
        internal static ArcheTypeEqualityComparer Comparer => new ArcheTypeEqualityComparer();

        public bool Equals(ArcheType x, ArcheType y) => x == y;
        public int GetHashCode(ArcheType obj) => obj.GetHashCode();
    }
}
