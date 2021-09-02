using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.Utilities
{
    public static class IndexHelpers
    {
        public static int[] HashIndexes(int[] indexes)
        {
            var hash = new HashSet<int>();
            foreach (var i in indexes)
                hash.Add(i);

            return hash.OrderBy(x => x).ToArray();
        }

        public static int[] MergeDistinctIndex(int[] indexes, int index)
        {
            if (indexes == null)
            {
                indexes = new int[1] {index};
            }
            else if (!indexes.Any(x => x == index))
            {
                Array.Resize(ref indexes, indexes.Length + 1);
                indexes[indexes.Length - 1] = index;
                Array.Sort(indexes);
            }

            return indexes;
        }

        public static int[] MergeDistinctIndexes(params int[][] allIndexes)
        {
            var mergedIndices = new HashSet<int>();
            foreach (var indices in allIndexes)
                if (indices != null)
                    foreach (var index in indices)
                        mergedIndices.Add(index);

            return mergedIndices.OrderBy(x => x).ToArray();
        }
    }
}