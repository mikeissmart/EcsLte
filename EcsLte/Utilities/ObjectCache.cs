using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EcsLte.Utilities
{
    internal delegate void RefCountZeroEvent<TObject>(TObject obj);

    public static class ObjectCache<T> where T : new()
    {
        private static readonly Queue<T> _pool = new Queue<T>();

        public static T Pop()
        {
            if (ObjectCache.IsCacheEnabled)
                lock (_pool)
                {
                    if (_pool.Count > 0)
                        return _pool.Dequeue();
                }

            return new T();
        }

        public static void Push(T obj)
        {
            if (ObjectCache.IsCacheEnabled)
                lock (_pool)
                {
                    _pool.Enqueue(obj);
                }
        }
    }

    public static class ObjectCache
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _objectPools
            = new ConcurrentDictionary<Type, ConcurrentQueue<object>>();

        public static bool IsCacheEnabled { get; set; } = true;

        /*public static T Pop<T>() where T : new()
        {
            if (IsCacheEnabled)
            {
                var type = typeof(T);
                if (!_objectPools.TryGetValue(type, out var cacheables))
                {
                    cacheables = new ConcurrentQueue<object>();
                    _objectPools.TryAdd(type, cacheables);
                }

                if (cacheables.TryDequeue(out var result))
                    return (T)result;
            }

            return new T();
        }

        public static void Push<T>(T obj) where T : new()
        {
            if (IsCacheEnabled)
            {
                var type = typeof(T);
                if (!_objectPools.TryGetValue(type, out var cacheables))
                {
                    cacheables = new ConcurrentQueue<object>();
                    _objectPools.TryAdd(type, cacheables);
                }

                cacheables.Enqueue(obj);
            }
        }*/
    }
}