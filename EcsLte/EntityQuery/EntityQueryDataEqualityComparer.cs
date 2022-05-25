using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal class EntityQueryDataEqualityComparer : IEqualityComparer<EntityQueryData>
    {
        internal static EntityQueryDataEqualityComparer Comparer => new EntityQueryDataEqualityComparer();

        public bool Equals(EntityQueryData x, EntityQueryData y) => x == y;
        public int GetHashCode(EntityQueryData obj) => obj.GetHashCode();
    }
}
