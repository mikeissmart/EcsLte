using System;

namespace EcsLte.Utilities
{
    public class DataCache<TCached>
    {
        private readonly Func<TCached> _recache;
        private TCached _data;

        public DataCache(Func<TCached> recache)
        {
            _recache = recache;
        }

        public DataCache(bool initializeDirty, Func<TCached> recache) : this(recache)
        {
            IsDirty = initializeDirty;
        }

        public DataCache(TCached initializeCache, Func<TCached> recache) : this(false, recache)
        {
            _data = initializeCache;
        }

        public DataCache(bool initializeDirty, TCached initializeCache, Func<TCached> recache) : this(initializeDirty,
            recache)
        {
            _data = initializeCache;
        }

        public TCached Data
        {
            get
            {
                if (IsDirty)
                {
                    _data = _recache();
                    IsDirty = false;
                }

                return _data;
            }
            set
            {
                IsDirty = false;
                _data = value;
            }
        }

        public bool IsDirty { get; set; } = true;
    }
}