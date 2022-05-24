using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.Data
{
    internal interface IIndexDictionary
    {
        int GetOrAdd(object key);

        int GetOrAdd(object key, Action<int> addAction);

        int GetOrAdd(object key, Func<int, object> addAction);

        object GetKey(int index);

        bool PopKeyObj(out object key);
    }

    internal class IndexDictionary<TKey> : IIndexDictionary
    {
        private readonly Dictionary<TKey, int> _indexes;
        private readonly List<TKey> _values;
        private readonly object _lockObj;

        internal IndexDictionary()
        {
            _indexes = new Dictionary<TKey, int>();
            _values = new List<TKey>();
            _lockObj = new object();
        }

        internal int GetOrAdd(TKey key)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out var index))
                {
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

        internal int GetOrAdd(TKey key, Action<int> addAction)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out var index))
                {
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

        internal bool PopKey(out TKey key)
        {
            key = default;
            lock (_lockObj)
            {
                if (_indexes.Count > 0)
                {
                    var pair = _indexes.First();
                    _indexes.Remove(pair.Key);
                    _values[pair.Value] = default;
                    key = pair.Key;

                    return true;
                }
            }

            return false;
        }

        public bool PopKeyObj(out object key)
        {
            key = default;
            lock (_lockObj)
            {
                if (_indexes.Count > 0)
                {
                    var pair = _indexes.First();
                    _indexes.Remove(pair.Key);
                    _values[pair.Value] = default;
                    key = pair.Key;

                    return true;
                }
            }

            return false;
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