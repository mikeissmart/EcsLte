using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EcsLte.Utilities;

[assembly: InternalsVisibleTo("EcsLte.UnitTest")]

namespace EcsLte
{
    public partial struct Filter : IEquatable<Filter>
    {
        private int _hashCode;
        private int[] _indexes;

        internal int[] Indexes
        {
            get
            {
                if (_indexes == null)
                    _indexes = Helpers.MergeDistinctIndexes(AllOfIndexes, AnyOfIndexes, NoneOfIndexes);
                return _indexes;
            }
        }

        internal int[] AllOfIndexes { get; private set; }
        internal int[] AnyOfIndexes { get; private set; }
        internal int[] NoneOfIndexes { get; private set; }

        public static Filter Combine(params Filter[] filters)
        {
            var filter = new Filter();
            foreach (var f in filters)
                filter.AllOfIndexes = Helpers.MergeDistinctIndexes(filter.AllOfIndexes, f.AllOfIndexes);
            foreach (var f in filters)
                filter.AnyOfIndexes = Helpers.MergeDistinctIndexes(filter.AnyOfIndexes, f.AnyOfIndexes);
            foreach (var f in filters)
                filter.NoneOfIndexes = Helpers.MergeDistinctIndexes(filter.NoneOfIndexes, f.NoneOfIndexes);
            filter.GenerateHasCode();

            return filter;
        }

        public static bool operator !=(Filter lhs, Filter rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Filter lhs, Filter rhs)
        {
            return lhs.GetHashCode() == rhs.GetHashCode();
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

        private void GenerateHasCode()
        {
            _hashCode = (
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(AllOfIndexes),
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(AnyOfIndexes),
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(NoneOfIndexes)
            ).GetHashCode();
        }
    }
}