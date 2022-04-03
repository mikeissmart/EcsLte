using System;
using System.Collections.Generic;

namespace EcsLte.Data
{
    internal class DictionaryDataCache<TKey, TValue> : DataCache<Dictionary<TKey, TValue>, TValue[]>
    {
        public DictionaryDataCache(Func<Dictionary<TKey, TValue>, TValue[]> recacheFunc) :
            base(recacheFunc, new Dictionary<TKey, TValue>(), null, true)
        {

        }

        public DictionaryDataCache(Func<Dictionary<TKey, TValue>, TValue[]> recacheFunc,
            Dictionary<TKey, TValue> initUncached, TValue[] initCached = null, bool initDirty = true) :
            base(recacheFunc, initUncached, initCached, initDirty)
        {
        }

        public bool Has(TKey key) => UncachedData.ContainsKey(key);

        public void Add(TKey key, TValue value)
        {
            UncachedData.Add(key, value);
            SetDirty();
        }

        public TValue Get(TKey key) => UncachedData[key];

        public void Set(TKey key, TValue value)
        {
            UncachedData[key] = value;
            SetDirty();
        }

        public void Remove(TKey key)
        {
            UncachedData.Remove(key);
            SetDirty();
        }

        public void Clear()
        {
            UncachedData.Clear();
            SetDirty();
        }
    }
}
