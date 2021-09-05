using System;
using System.Collections.Generic;

namespace EcsLte.Utilities
{
    public class ObjectCache
    {
        private static ObjectCache _instance;

        private readonly Dictionary<Type, object> _objectPools = new Dictionary<Type, object>();

        private ObjectCache()
        {
            RegisterCustomObjectPool(new ObjectPool<HashSet<int>>(
                () => new HashSet<int>(),
                x => x.Clear()
            ));
            RegisterCustomObjectPool(new ObjectPool<Entity[]>(
                () => new Entity[0],
                x => Array.Clear(x, 0, x.Length)
            ));
        }

        public static ObjectCache Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ObjectCache();
                return _instance;
            }
        }

        public ObjectPool<T> GetObjectPool<T>() where T : new()
        {
            object objectPool;
            var type = typeof(T);

            lock (_objectPools)
            {
                if (!_objectPools.TryGetValue(type, out objectPool))
                    throw new Exception("ObjectPool does not exist");
                /*objectPool = new ObjectPool<T>(() => new T());
                    _objectPools.Add(type, objectPool);*/
            }

            return (ObjectPool<T>) objectPool;
        }

        public T Get<T>() where T : new()
        {
            return GetObjectPool<T>().Get();
        }

        public void Push<T>(T obj) where T : new()
        {
            GetObjectPool<T>().Push(obj);
        }

        private void RegisterCustomObjectPool<T>(ObjectPool<T> objectPool)
        {
            _objectPools.Add(typeof(T), objectPool);
        }

        public void Reset()
        {
            _objectPools.Clear();
        }
    }
}