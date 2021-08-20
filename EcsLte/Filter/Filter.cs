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
        private static HashSet<int> _distinctIndicesBuffer;

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

        internal bool Filtered(EntityInfo entityInfo)
        {
            return FilteredAllOf(entityInfo) &&
                   FilteredAnyOf(entityInfo) &&
                   FilteredNoneOf(entityInfo);
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
            if (_distinctIndicesBuffer == null)
                _distinctIndicesBuffer = new HashSet<int>();
            else
                _distinctIndicesBuffer.Clear();

            foreach (var indices in allIndices)
                if (indices != null)
                    foreach (var index in indices)
                        _distinctIndicesBuffer.Add(index);

            var mergedIndices = new int[_distinctIndicesBuffer.Count];
            _distinctIndicesBuffer.CopyTo(mergedIndices);
            Array.Sort(mergedIndices);

            return mergedIndices;
        }

        private bool FilteredAllOf(EntityInfo entityInfo)
        {
            if (AllOfIndexes == null || AllOfIndexes.Length == 0)
                return true;

            var isOk = true;
            foreach (var index in AllOfIndexes)
                if (entityInfo[index] == null)
                {
                    isOk = false;
                    break;
                }

            return isOk;
        }

        private bool FilteredAnyOf(EntityInfo entityInfo)
        {
            if (AnyOfIndexes == null || AnyOfIndexes.Length == 0)
                return true;

            var isOk = false;
            foreach (var index in AnyOfIndexes)
                if (entityInfo[index] != null)
                {
                    isOk = true;
                    break;
                }

            return isOk;
        }

        private bool FilteredNoneOf(EntityInfo entityInfo)
        {
            if (NoneOfIndexes == null || NoneOfIndexes.Length == 0)
                return true;

            var isOk = true;
            foreach (var index in NoneOfIndexes)
                if (entityInfo[index] != null)
                {
                    isOk = false;
                    break;
                }

            return isOk;
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