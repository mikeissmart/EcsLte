using System;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    public partial struct CollectorTrigger : IEquatable<CollectorTrigger>
    {
        private int _hashCode;

        internal int[] Indexes { get; private set; }
        internal int[] AddedIndexes { get; private set; }
        internal int[] RemovedIndexes { get; private set; }
        internal int[] ReplacedIndexes { get; private set; }

        public static CollectorTrigger Combine(params CollectorTrigger[] collectorTriggers)
        {
            var collectorTrigger = new CollectorTrigger();
            foreach (var f in collectorTriggers)
                collectorTrigger.Indexes = IndexHelpers.MergeDistinctIndexes(collectorTrigger.Indexes, f.Indexes);
            foreach (var f in collectorTriggers)
                collectorTrigger.AddedIndexes =
                    IndexHelpers.MergeDistinctIndexes(collectorTrigger.AddedIndexes, f.AddedIndexes);
            foreach (var f in collectorTriggers)
                collectorTrigger.RemovedIndexes =
                    IndexHelpers.MergeDistinctIndexes(collectorTrigger.RemovedIndexes, f.RemovedIndexes);
            foreach (var f in collectorTriggers)
                collectorTrigger.ReplacedIndexes =
                    IndexHelpers.MergeDistinctIndexes(collectorTrigger.ReplacedIndexes, f.ReplacedIndexes);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static bool operator !=(CollectorTrigger lhs, CollectorTrigger rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(CollectorTrigger lhs, CollectorTrigger rhs)
        {
            return lhs.AddedIndexes.SequenceEqual(rhs.AddedIndexes) &&
                   lhs.RemovedIndexes.SequenceEqual(rhs.RemovedIndexes) &&
                   lhs.ReplacedIndexes.SequenceEqual(rhs.ReplacedIndexes);
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
            _hashCode = -1663471673;
            _hashCode = _hashCode * -1521134295 + AddedIndexes.Length;
            _hashCode = _hashCode * -1521134295 + RemovedIndexes.Length;
            _hashCode = _hashCode * -1521134295 + ReplacedIndexes.Length;
            foreach (var index in AddedIndexes)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
            foreach (var index in RemovedIndexes)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
            foreach (var index in ReplacedIndexes)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
        }
    }
}