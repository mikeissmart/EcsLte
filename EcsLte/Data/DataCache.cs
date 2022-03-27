using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data
{
	internal class DataCache<TUncached, TCached>
	{
		private readonly Func<TUncached, TCached> _recacheFunc;
		private TCached _cachedData;
		private bool _isDirty;

		public TUncached UncachedData;

		public TCached CachedData
		{
			get
			{
				if (_isDirty)
				{
					_cachedData = _recacheFunc(UncachedData);
					_isDirty = false;
				}

				return _cachedData;
			}
		}

		public DataCache(Func<TUncached, TCached> recacheFunc, TUncached initUncached, TCached initCached, bool initDirty = true)
		{
			_recacheFunc = recacheFunc;
			_cachedData = initCached;
			_isDirty = initDirty;

			UncachedData = initUncached;
		}

		public void SetDirty() => _isDirty = true;
	}
}
