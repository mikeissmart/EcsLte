using System;
using System.Linq;

namespace EcsLte
{
    internal struct GroupWithCollection : IEquatable<GroupWithCollection>
    {
        private int _hashCode;

        internal GroupWithCollection(IComponent[] componentKeyes)
        {
            _hashCode = 0;
            CalculateHashCode(componentKeyes);
        }

        public static bool operator !=(GroupWithCollection lhs, GroupWithCollection rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(GroupWithCollection lhs, GroupWithCollection rhs)
        {
            return lhs._hashCode == rhs._hashCode;
        }

        public bool Equals(GroupWithCollection other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is GroupWithCollection other && this == other;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private void CalculateHashCode(IComponent[] components)
        {
            var componentHashes = components
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);
            _hashCode = -1663471673;
            _hashCode = _hashCode * -1521134295 + componentHashes.Count();
            foreach (var key in componentHashes)
                _hashCode = _hashCode * -1521134295 + key.GetHashCode();
        }
    }
}