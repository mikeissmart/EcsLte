using System;

namespace EcsLte.Utilities
{
    public class DataCache<TUncached, TCached>
    {
        private readonly Func<TCached> _recacheFunc;
        private TCached _cachedData;

        public DataCache(TUncached initializeUncache, Func<TCached> recacheFunc)
        {
            _recacheFunc = recacheFunc;
            UncachedData = initializeUncache;
        }

        public DataCache(bool initializeDirty, TUncached initializeUncache, Func<TCached> recacheFunc) : this(initializeUncache, recacheFunc)
        {
            IsDirty = initializeDirty;
        }

        public DataCache(TCached initializeCache, TUncached initializeUncache, Func<TCached> recacheFunc) : this(false, initializeUncache, recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public DataCache(bool initializeDirty, TCached initializeCache, TUncached initializeUncache, Func<TCached> recacheFunc) : this(initializeDirty,
            initializeUncache, recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public TCached CachedData
        {
            get
            {
                if (IsDirty)
                {
                    _cachedData = _recacheFunc();
                    IsDirty = false;
                }

                return _cachedData;
            }
            set
            {
                IsDirty = false;
                _cachedData = value;
            }
        }

        public TUncached UncachedData;

        public bool IsDirty { get; set; } = true;
    }
}