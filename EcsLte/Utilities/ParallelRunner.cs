using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EcsLte.Utilities
{
    public delegate void ParallelCallback(int index, int startIndex, int endIndex);

    public delegate void ParallelDataCallback<TData>(int index, int startIndex, int endIndex, TData data);

    public static class ParallelRunner
    {
        private static readonly int _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        private static readonly int _threadCount = (int) (Environment.ProcessorCount * 0.75);
        private static readonly object _parallelCountLock = new object();
        private static int _parallelCount;

        public static bool IsMainThread =>
            _mainThreadId == Thread.CurrentThread.ManagedThreadId
            && _parallelCount == 0;

        /// <summary>
        ///     Incase I dont attempt to run independant systems seperatly
        /// </summary>
        public static void RunParallelForEach<TSource>(IEnumerable<TSource> sources, Action<TSource> callback)
        {
            if (sources.Count() != 0)
            {
                lock (_parallelCountLock)
                {
                    _parallelCount++;
                }

                var result = Parallel.ForEach(
                    sources,
                    new ParallelOptions {MaxDegreeOfParallelism = _threadCount},
                    callback);
                while (!result.IsCompleted) Thread.Sleep(1);
                lock (_parallelCountLock)
                {
                    _parallelCount--;
                }
            }
        }

        public static void RunParallelFor(int count, Action<int> callback)
        {
            var batches = new List<ParallelForBatch>();

            var batchCount = count / _threadCount +
                             (count % _threadCount != 0
                                 ? 1
                                 : 0);
            for (var i = 0; i < _threadCount; i++)
            {
                var batchStartIndex = i * batchCount;
                var batchEndIndex = batchStartIndex + batchCount > count
                    ? count
                    : batchStartIndex + batchCount;

                if (batchStartIndex < batchEndIndex)
                    batches.Add(new ParallelForBatch
                    {
                        StartIndex = batchStartIndex,
                        EndIndex = batchEndIndex
                    });
                else
                    break;
            }

            lock (_parallelCountLock)
            {
                _parallelCount++;
            }

            var result = Parallel.ForEach(
                batches,
                new ParallelOptions {MaxDegreeOfParallelism = _threadCount},
                batchIndex =>
                {
                    for (var i = batchIndex.StartIndex; i < batchIndex.EndIndex; i++)
                        callback.Invoke(i);
                });
            while (!result.IsCompleted) Thread.Sleep(1);
            lock (_parallelCountLock)
            {
                _parallelCount--;
            }
        }

        private class ParallelForBatch
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }
    }
}