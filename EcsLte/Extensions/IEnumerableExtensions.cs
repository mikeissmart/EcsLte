using System;
using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte
{
	public static class IEnumerableExtensions
	{
		public static void RunForEach<T>(this IEnumerable<T> source, Action<T> callback)
		{
			foreach (var item in source)
				callback.Invoke(item);
		}

		public static void RunForEachParallel<T>(this IEnumerable<T> source, Action<T> callback) => ParallelRunner.RunParallelForEach(source, callback);
	}
}