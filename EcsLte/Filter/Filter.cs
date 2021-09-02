using System;
using System.Linq;
using System.Runtime.CompilerServices;
using EcsLte.Utilities;

[assembly: InternalsVisibleTo("EcsLte.UnitTest")]

namespace EcsLte
{
    public partial struct Filter : IEquatable<Filter>
    {
        private int _hashCode;

        internal int[] Indexes { get; private set; }
        internal int[] AllOfIndexes { get; private set; }
        internal int[] AnyOfIndexes { get; private set; }
        internal int[] NoneOfIndexes { get; private set; }

        public static Filter Combine(params Filter[] filters)
        {
            var filter = new Filter();
            foreach (var f in filters)
                filter.Indexes = IndexHelpers.MergeDistinctIndexes(filter.Indexes, f.Indexes);
            foreach (var f in filters)
                filter.AllOfIndexes = IndexHelpers.MergeDistinctIndexes(filter.AllOfIndexes, f.AllOfIndexes);
            foreach (var f in filters)
                filter.AnyOfIndexes = IndexHelpers.MergeDistinctIndexes(filter.AnyOfIndexes, f.AnyOfIndexes);
            foreach (var f in filters)
                filter.NoneOfIndexes = IndexHelpers.MergeDistinctIndexes(filter.NoneOfIndexes, f.NoneOfIndexes);
            filter.CalculateHashCode();

            return filter;
        }

        public static bool operator !=(Filter lhs, Filter rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Filter lhs, Filter rhs)
        {
            return lhs.AllOfIndexes.SequenceEqual(rhs.AllOfIndexes) &&
                   lhs.AnyOfIndexes.SequenceEqual(rhs.AnyOfIndexes) &&
                   lhs.NoneOfIndexes.SequenceEqual(rhs.NoneOfIndexes);
        }

        public bool Equals(Filter other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Filter other && this == other;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return string.Join(", ", Indexes);
        }

        private void CalculateHashCode()
        {
            _hashCode = -1663471673;
            _hashCode = _hashCode * -1521134295 + AllOfIndexes.Length;
            _hashCode = _hashCode * -1521134295 + AnyOfIndexes.Length;
            _hashCode = _hashCode * -1521134295 + NoneOfIndexes.Length;
            foreach (var index in AllOfIndexes)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
            foreach (var index in AnyOfIndexes)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
            foreach (var index in NoneOfIndexes)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
        }
    }
}