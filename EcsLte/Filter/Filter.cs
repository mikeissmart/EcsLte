using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EcsLte.UnitTest")]

namespace EcsLte
{
    public partial struct Filter : IEquatable<Filter>
    {
        private static HashSet<int> _distinctIndicesBuffer = new HashSet<int>();

        private int _hashCode;
        private int[] _indexes;

        internal int[] Indexes
        {
            get
            {
                if (_indexes == null)
                    _indexes = MergeDistinctIndices(AllOfIndexes, AnyOfIndexes, NoneOfIndexes);
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
                filter.AllOfIndexes = MergeDistinctIndices(filter.AllOfIndexes, f.AllOfIndexes);
            foreach (var f in filters)
                filter.AnyOfIndexes = MergeDistinctIndices(filter.AnyOfIndexes, f.AnyOfIndexes);
            foreach (var f in filters)
                filter.NoneOfIndexes = MergeDistinctIndices(filter.NoneOfIndexes, f.NoneOfIndexes);
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

        private static int[] MergeDistinctIndex(int[] indices, int index)
        {
            if (indices == null)
            {
                indices = new int[1] { index };
            }
            else
            {
                if (!indices.Any(x => x == index))
                {
                    Array.Resize(ref indices, indices.Length + 1);
                    indices[indices.Length - 1] = index;
                    Array.Sort(indices);
                }
            }

            return indices;
        }

        private static int[] MergeDistinctIndices(params int[][] allIndices)
        {
            int[] mergedIndices = null;
            lock (_distinctIndicesBuffer)
            {
                _distinctIndicesBuffer.Clear();
                foreach (var indices in allIndices)
                {
                    if (indices != null)
                    {
                        foreach (var index in indices)
                            _distinctIndicesBuffer.Add(index);
                    }
                }

                mergedIndices = new int[_distinctIndicesBuffer.Count];
                _distinctIndicesBuffer.CopyTo(mergedIndices);
            }
            Array.Sort(mergedIndices);

            return mergedIndices;
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