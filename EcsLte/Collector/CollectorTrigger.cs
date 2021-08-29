using System;
using System.Collections;
using EcsLte.Utilities;

namespace EcsLte
{
    public partial struct CollectorTrigger : IEquatable<CollectorTrigger>
    {
        private int _hashCode;
        private int[] _indexes;

        internal int[] Indexes
        {
            get
            {
                if (_indexes == null)
                    _indexes = Helpers.MergeDistinctIndexes(AddedIndexes, RemovedIndexes, UpdatedIndexes);
                return _indexes;
            }
        }

        internal int[] AddedIndexes { get; private set; }
        internal int[] RemovedIndexes { get; private set; }
        internal int[] UpdatedIndexes { get; private set; }

        public static CollectorTrigger Combine(params CollectorTrigger[] collectorTriggers)
        {
            var collectorTrigger = new CollectorTrigger();
            foreach (var f in collectorTriggers)
                collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndexes(collectorTrigger.AddedIndexes, f.AddedIndexes);
            foreach (var f in collectorTriggers)
                collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndexes(collectorTrigger.RemovedIndexes, f.RemovedIndexes);
            foreach (var f in collectorTriggers)
                collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndexes(collectorTrigger.UpdatedIndexes, f.UpdatedIndexes);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static bool operator !=(CollectorTrigger lhs, CollectorTrigger rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(CollectorTrigger lhs, CollectorTrigger rhs)
        {
            return lhs.GetHashCode() == rhs.GetHashCode();
        }

        public bool Equals(CollectorTrigger other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is CollectorTrigger other && this == other;
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
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(AddedIndexes),
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(RemovedIndexes),
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(UpdatedIndexes)
            ).GetHashCode();
        }
    }
}