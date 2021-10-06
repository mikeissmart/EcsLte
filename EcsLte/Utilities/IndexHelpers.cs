using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.Utilities
{
    public static class IndexHelpers
    {
        public static int[] HashIndexes(int[] indexes)
        {
            var hash = ObjectCache<HashSet<int>>.Pop();
            foreach (var i in indexes)
                hash.Add(i);

            var temp = hash.OrderBy(x => x).ToArray();
            hash.Clear();
            ObjectCache<HashSet<int>>.Push(hash);

            return temp;
        }

        public static int[] MergeDistinctIndex(int[] indexes, int index)
        {
            if (indexes == null)
            {
                indexes = new int[1] { index };
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
            var hash = ObjectCache<HashSet<int>>.Pop();
            foreach (var indices in allIndexes)
                if (indices != null)
                    hash.UnionWith(indices);

            var temp = hash.OrderBy(x => x).ToArray();
            hash.Clear();
            ObjectCache<HashSet<int>>.Push(hash);

            return temp;
        }
    }
}