using System;
using System.Collections.Generic;
using System.Text;
using EcsLte.Utilities;

namespace EcsLte.Data
{
	internal static class ObjectCache<TObj> where TObj : new()
	{
		private static readonly Queue<TObj> _pool = new Queue<TObj>();

		public static TObj Pop()
		{
			if (EcsSettings.IsObjectCacheEnabled && _pool.Count > 0)
				return _pool.Dequeue();
			return new TObj();
		}

		public static void Push(TObj obj)
		{
			if (EcsSettings.IsObjectCacheEnabled)
				_pool.Enqueue(obj);
		}
	}
}
