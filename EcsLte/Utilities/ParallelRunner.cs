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
        private static readonly int _threadCount = Environment.ProcessorCount;
        private static BaseParallelOptions[] _parallelOptions;
        private static int _runningThreads;

        public static bool IsMainThread => _mainThreadId == Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        ///     Run callback on multiple threads in batches. Callback params are (index, startIndex, endIndex, data[index])
        /// </summary>
        public static void RunParallel<TData>(ICollection<TData> datas, ParallelDataCallback<TData> callback)
        {
            InitializeOptions();
            _runningThreads = _threadCount;

            var batchCount = datas.Count / _runningThreads +
                             (datas.Count % _runningThreads != 0
                                 ? 1
                                 : 0);
            for (var i = 0; i < _threadCount; i++)
            {
                var startIndex = i * batchCount;
                var endIndex = startIndex + batchCount > datas.Count
                    ? datas.Count
                    : startIndex + batchCount;

                if (startIndex != endIndex)
                {
                    var options = new ParallelOptions<TData>
                    {
                        StartIndex = startIndex,
                        EndIndex = endIndex,
                        Datas = datas,
                        Callback = callback
                    };

                    _parallelOptions[i] = options;
                    ThreadPool.QueueUserWorkItem(ParallelBatch, options);
                }
                else
                {
                    Interlocked.Decrement(ref _runningThreads);
                }
            }

            while (_runningThreads > 0)
            {
            }
        }

        /// <summary>
        ///     Incase I dont attempt to run independant systems seperatly
        /// </summary>
        public static void RunParallelForEach<TSource>(IEnumerable<TSource> sources, Action<TSource> callback)
        {
            Parallel.ForEach(
                sources,
                callback);
        }

        /// <summary>
        ///     Run callback on multiple threads in batches. Callback params are (index, startIndex, endIndex)
        /// </summary>
        public static void RunParallel(int count, ParallelCallback callback)
        {
            InitializeOptions();
            _runningThreads = _threadCount;

            var batchCount = count / _runningThreads +
                             (count % _runningThreads != 0
                                 ? 1
                                 : 0);
            for (var i = 0; i < _threadCount; i++)
            {
                var startIndex = i * batchCount;
                var endIndex = startIndex + batchCount > count
                    ? count
                    : startIndex + batchCount;

                if (startIndex < endIndex)
                {
                    var options = new ParallelOptions
                    {
                        StartIndex = startIndex,
                        EndIndex = endIndex,
                        Callback = callback
                    };

                    _parallelOptions[i] = options;
                    ThreadPool.QueueUserWorkItem(ParallelBatch, options);
                }
                else
                {
                    Interlocked.Decrement(ref _runningThreads);
                }
            }

            while (_runningThreads > 0) Thread.Sleep(1);

            foreach (var option in _parallelOptions)
            {
                if (option.Exception != null)
                    throw new Exception($"{option.Exception.Message}\n{option.Exception.StackTrace}");
                option.Exception = null;
            }
        }

        private static void InitializeOptions()
        {
            if (_parallelOptions == null)
            {
                _parallelOptions = new BaseParallelOptions[_threadCount];
                for (var i = 0; i < _threadCount; i++)
                    _parallelOptions[i] = new ParallelOptions();
            }
        }

        private static void ParallelBatch(object batchOptions)
        {
            var options = (BaseParallelOptions)batchOptions;
            try
            {
                for (var i = options.StartIndex; i < options.EndIndex; i++)
                    options.InvokeCallback(i);
            }
            catch (Exception ex)
            {
                options.Exception = ex;
            }
            finally
            {
                Interlocked.Decrement(ref _runningThreads);
            }
        }

        private abstract class BaseParallelOptions
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public Exception Exception { get; set; }

            public abstract void InvokeCallback(int index);
        }

        private class ParallelOptions : BaseParallelOptions
        {
            public ParallelCallback Callback { get; set; }

            public override void InvokeCallback(int index)
            {
                Callback.Invoke(index, StartIndex, EndIndex);
            }
        }

        private class ParallelOptions<TData> : BaseParallelOptions
        {
            public ICollection<TData> Datas { get; set; }
            public ParallelDataCallback<TData> Callback { get; set; }

            public override void InvokeCallback(int index)
            {
                Callback.Invoke(index, StartIndex, EndIndex, Datas.ElementAt(index));
            }
        }
    }
}