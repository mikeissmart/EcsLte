using System;

namespace EcsLte.Utilities
{
    public class DataCache<TUncached, TCached>
    {
        private readonly object _isDirtyLock;
        private readonly Func<TUncached, TCached> _recacheFunc;
        private TCached _cachedData;
        private bool _isDirty;

        public TUncached UncachedData;

        public DataCache(TUncached initializeUncache, Func<TUncached, TCached> recacheFunc)
        {
            _recacheFunc = recacheFunc;
            _isDirty = true;
            _isDirtyLock = new object();

            UncachedData = initializeUncache;
        }

        public DataCache(bool initializeDirty, TUncached initializeUncache, Func<TUncached, TCached> recacheFunc) : this(
            initializeUncache, recacheFunc)
        {
            _isDirty = initializeDirty;
        }

        public DataCache(TCached initializeCache, TUncached initializeUncache, Func<TUncached, TCached> recacheFunc) : this(false,
            initializeUncache, recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public DataCache(bool initializeDirty, TCached initializeCache, TUncached initializeUncache,
            Func<TUncached, TCached> recacheFunc) : this(initializeDirty,
            initializeUncache, recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public TCached CachedData
        {
            get
            {
                lock (_isDirtyLock)
                {
                    if (_isDirty)
                    {
                        lock (this)
                        {
                            _cachedData = _recacheFunc(UncachedData);
                        }
                        _isDirty = false;
                    }
                }

                return _cachedData;
            }
        }

        public void SetDirty()
        {
            _isDirty = true;
        }
    }
}