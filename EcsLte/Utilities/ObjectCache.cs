using System;
using System.Collections.Concurrent;

namespace EcsLte.Utilities
{
    public static class ObjectCache
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _objectPools
            = new ConcurrentDictionary<Type, ConcurrentQueue<object>>();

        public static bool IsCacheEnabled { get; set; } = true;

        public static T Pop<T>() where T : new()
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
                    return (T) result;
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
        }
    }
}