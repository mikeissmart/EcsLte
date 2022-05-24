using System;
using System.Collections.Generic;

namespace EcsLte.Data
{
    internal interface IIndexDictionary
    {
        /// <summary>
        /// Return true = new index for value, false = existing index for value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool GetIndexObj(object key, out int index);

        int GetIndexObj(object key);

        object GetObject(int index);

        void Clear();
    }

    /*internal interface ISharedComponentIndexDictionary : IIndexDictionary
    {
        IComponentData GetComponentData(SharedComponentDataIndex dataIndex);
    }*/

    internal class IndexDictionary<TKey> : IIndexDictionary
    {
        private readonly Dictionary<TKey, int> _indexes;
        private readonly List<TKey> _values;
        private readonly object _lockObj;

        public IndexDictionary()
        {
            _indexes = new Dictionary<TKey, int>();
            _values = new List<TKey>();
            _lockObj = new object();
        }

        public IEnumerable<TKey> Keys => _indexes.Keys;
        public IEnumerable<int> Indexes => _indexes.Values;
        public IEnumerable<KeyValuePair<TKey, int>> KeyValuePairs => _indexes;

        public Dictionary<TKey, int> GetDictionary() => _indexes;

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns>true = new index for value, false = existing index for value</returns>
        public bool GetIndex(TKey key, out int index)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out index))
                {
                    index = _values.Count;
                    _values.Add(key);
                    _indexes.Add(key, index);
                    return true;
                }
            }

            return false;
        }

        public int GetIndex(TKey key)
        {
            lock (_lockObj)
            {
                if (!_indexes.TryGetValue(key, out var index))
                {
                    index = _values.Count;
                    _values.Add(key);
                    _indexes.Add(key, index);
                }

                return index;
            }
        }

        public bool GetIndexObj(object key, out int index)
        {
            if (key is TKey val)
                return GetIndex(val, out index);
            throw new InvalidCastException("key");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns>true = new index for value, false = existing index for value</returns>
        public int GetIndexObj(object key)
        {
            if (key is TKey val)
                return GetIndex(val);
            throw new InvalidCastException("key");
        }

        public object GetObject(int index) => _values[index];

        public TKey GetKey(int index) => _values[index];

        public void RemoveValue(TKey key)
        {
            lock (_lockObj)
            {
                _indexes.Remove(key);
            }
        }

        public void RemoveObj(object value)
        {
            if (value is TKey val)
                RemoveValue(val);
            else
                throw new InvalidCastException("key");
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