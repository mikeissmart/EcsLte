using System;

namespace EcsLte.Utilities
{
    public class DataCache<TCached>
    {
        private readonly Func<TCached> _recacheFunc;
        private TCached _cachedData;

        public DataCache(Func<TCached> recacheFunc)
        {
            _recacheFunc = recacheFunc;
        }

        public DataCache(bool initializeDirty, Func<TCached> recacheFunc) : this(recacheFunc)
        {
            IsDirty = initializeDirty;
        }

        public DataCache(TCached initializeCache, Func<TCached> recacheFunc) : this(false, recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public DataCache(bool initializeDirty, TCached initializeCache, Func<TCached> recacheFunc) : this(initializeDirty,
            recacheFunc)
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

        public bool IsDirty { get; set; } = true;
    }

    public class DataCache<TUncached, TCached>
    {
        private readonly Func<TCached> _recacheFunc;
        private TUncached _uncachedData;
        private TCached _cachedData;

        public DataCache(Func<TCached> recacheFunc)
        {
            _recacheFunc = recacheFunc;
        }

        public DataCache(bool initializeDirty, Func<TCached> recacheFunc) : this(recacheFunc)
        {
            IsDirty = initializeDirty;
        }

        public DataCache(TCached initializeCache, Func<TCached> recacheFunc) : this(false, recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public DataCache(bool initializeDirty, TCached initializeCache, Func<TCached> recacheFunc) : this(initializeDirty,
            recacheFunc)
        {
            _cachedData = initializeCache;
        }

        public DataCache(bool initializeDirty, TCached initializeCache, TUncached initializeUncache, Func<TCached> recacheFunc) : this(initializeDirty,
            recacheFunc)
        {
            _uncachedData = initializeUncache;
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

        public TUncached UncachedData { get; set; } = default;

        public bool IsDirty { get; set; } = true;
    }
}