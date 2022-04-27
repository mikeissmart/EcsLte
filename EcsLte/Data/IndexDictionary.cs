using System;
using System.Collections.Generic;
using System.Text;

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
        void Clear();
    }

    internal static class IndexDictionary
    {
        public static IIndexDictionary[] CreateSharedComponentIndexDictionaries()
        {
            var sharedIndexes = new IIndexDictionary[ComponentConfigs.Instance.SharedComponentCount];
            var indexDicType = typeof(IndexDictionary<>);
            for (var i = 0; i < sharedIndexes.Length; i++)
            {
                sharedIndexes[i] = (IIndexDictionary)Activator
                    .CreateInstance(indexDicType
                        .MakeGenericType(ComponentConfigs.Instance.AllSharedTypes[i]));
            }

            return sharedIndexes;
        }
    }

    internal class IndexDictionary<TKey> : IIndexDictionary
    {
        private readonly Dictionary<TKey, int> _indexes;
        private int _nextIndex;

        public IndexDictionary() => _indexes = new Dictionary<TKey, int>();

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
            if (!_indexes.TryGetValue(key, out index))
            {
                index = _nextIndex++;
                _indexes.Add(key, index);
                return true;
            }

            return false;
        }

        public int GetIndex(TKey key)
        {
            if (!_indexes.TryGetValue(key, out var index))
            {
                index = _nextIndex++;
                _indexes.Add(key, index);
            }

            return index;
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

        public void RemoveValue(TKey key) => _indexes.Remove(key);

        public void RemoveObj(object value)
        {
            if (value is TKey val)
                RemoveValue(val);
            else
                throw new InvalidCastException("key");
        }

        public void Clear()
        {
            _indexes.Clear();
            _nextIndex = 0;
        }
    }
}
