using System;
using System.Linq;

namespace EcsLte
{
    internal struct KeyCollection : IEquatable<KeyCollection>
    {
        private int _hashCode;

        public IComponent[] ComponentKeyes { get; private set; }

        internal KeyCollection(IComponent[] componentKeyes)
        {
            ComponentKeyes = componentKeyes;
            _hashCode = 0;
            CalculateHashCode();
        }

        public static bool operator !=(KeyCollection lhs, KeyCollection rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(KeyCollection lhs, KeyCollection rhs)
        {
            return lhs.ComponentKeyes.SequenceEqual(rhs.ComponentKeyes);
        }

        public bool Equals(KeyCollection other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is KeyCollection other && this == other;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private void CalculateHashCode()
        {
            _hashCode = -1663471673;
            _hashCode = _hashCode * -1521134295 + ComponentKeyes.Length;
            foreach (var key in ComponentKeyes)
                _hashCode = _hashCode * -1521134295 + key.GetHashCode();
        }
    }
}