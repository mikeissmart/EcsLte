using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
	public static class EcsContexts
	{
		private static readonly DataCache<Dictionary<string, EcsContext>, EcsContext[]> _ecsContexts =
			new DataCache<Dictionary<string, EcsContext>, EcsContext[]>(
				new Dictionary<string, EcsContext>(),
				UpdateContextsCache);

		public static EcsContext[] Contexts => _ecsContexts.CachedData;
		public static EcsContext DefaultContext { get; set; } = CreateContext("Default");

		public static bool HasContext(string name) => _ecsContexts.UncachedData.ContainsKey(name);

		public static EcsContext GetContext(string name)
		{
			if (!HasContext(name))
				throw new EcsContextDoesNotExistException(name);

			return _ecsContexts.UncachedData[name];
		}

		public static EcsContext CreateContext(string name)
		{
			if (HasContext(name))
				throw new EcsContextNameAlreadyExistException(name);

			var context = new EcsContext(name);
			_ecsContexts.UncachedData.Add(name, context);
			_ecsContexts.SetDirty();

			return context;
		}

		public static void DestroyContext(EcsContext context)
		{
			if (context == null)
				throw new ArgumentNullException();
			if (context.IsDestroyed)
				throw new EcsContextIsDestroyedException(context);

			context.InternalDestroy();

			_ecsContexts.UncachedData.Remove(context.Name);
			_ecsContexts.SetDirty();
		}

		private static EcsContext[] UpdateContextsCache(Dictionary<string, EcsContext> uncached) => uncached.Values.ToArray();
	}
}