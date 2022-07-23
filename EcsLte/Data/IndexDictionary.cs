using System;
using System.Collections.Generic;

namespace EcsLte.Data
{
    internal interface IIndexDictionary
    {
        int GetOrAdd(object key);

        unsafe int GetOrAdd(byte* sharedComponentPtr);

        int GetOrAdd(object key, Action<int> addAction);

        int GetOrAdd(object key, Func<int, object> addAction);

        object GetKey(int index);

        void Clear();
    }

    internal class IndexDictionary<TKey> : IIndexDictionary
        where TKey : unmanaged
    {
        private readonly Dictionary<TKey, int> _indexes;
        private readonly List<TKey> _values;
        private readonly object _lockObj;

        protected IndexDictionary()
        {
            _indexes = new Dictionary<TKey, int>();
            _values = new List<TKey>();
            _lockObj = new object();
        }

        internal IndexDictionary(IEqualityComparer<TKey> comparer)
        {
            _indexes = new Dictionary<TKey, int>(comparer);
            _values = new List<TKey>();
            _lockObj = new object();
        }

        internal int GetOrAdd(TKey key)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out var index))
                {
                    index = _values.Count;
                    _indexes.Add(key, index);
                    _values.Add(key);
                }

                return index;
            }
        }

        public int GetOrAdd(object key)
        {
            if (key is TKey val)
                return GetOrAdd(val);
            throw new InvalidCastException("key");
        }

        public unsafe int GetOrAdd(byte* sharedComponentPtr) => GetOrAdd(*(TKey*)sharedComponentPtr);

        internal int GetOrAdd(TKey key, Action<int> addAction)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out var index))
                {
                    index = _values.Count;
                    _indexes.Add(key, index);
                    _values.Add(key);
                    addAction.Invoke(index);
                }

                return index;
            }
        }

        public int GetOrAdd(object key, Action<int> addAction)
        {
            if (key is TKey val)
                return GetOrAdd(val, addAction);
            throw new InvalidCastException("key");
        }

        internal int GetOrAdd(TKey key, Func<int, TKey> addAction)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out var index))
                {
                    index = _values.Count;
                    key = addAction.Invoke(index);
                    _indexes.Add(key, index);
                    _values.Add(key);
                }

                return index;
            }
        }

        public int GetOrAdd(object key, Func<int, object> addAction)
        {
            if (key is TKey val)
                return GetOrAdd(val, addAction);
            throw new InvalidCastException("key");
        }

        internal TKey GetKey(int index)
        {
            lock (_lockObj)
            {
                return _values[index];
            }
        }

        object IIndexDictionary.GetKey(int index)
        {
            lock (_lockObj)
            {
                return _values[index];
            }
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _indexes.Clear();
                _values.Clear();
            }
        }
    }
}