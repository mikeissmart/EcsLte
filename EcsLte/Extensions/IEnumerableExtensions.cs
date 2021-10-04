using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> callback)
        {
            foreach (var item in source)
                callback.Invoke(item);
        }

        public static void ForEachParallel<T>(this IEnumerable<T> source, Action<T> callback)
        {
            ParallelRunner.RunParallelForEach(source, callback);
        }
    }
}