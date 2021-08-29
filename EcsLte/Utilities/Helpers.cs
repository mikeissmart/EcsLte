using System.Collections.Generic;
using System;
using System.Linq;

namespace EcsLte.Utilities
{
    public static class Helpers
    {
        public static int[] MergeDistinctIndex(int[] indices, int index)
        {
            if (indices == null)
            {
                indices = new int[1] { index };
            }
            else if (!indices.Any(x => x == index))
            {
                Array.Resize(ref indices, indices.Length + 1);
                indices[indices.Length - 1] = index;
                Array.Sort(indices);
            }

            return indices;
        }

        public static int[] MergeDistinctIndexes(params int[][] allIndices)
        {
            var mergedIndices = new List<int>();
            foreach (var indices in allIndices)
            {
                if (indices != null)
                {
                    foreach (var index in indices)
                        mergedIndices.Add(index);
                }
            }

            return mergedIndices.OrderBy(x => x).ToArray();
        }
    }
}