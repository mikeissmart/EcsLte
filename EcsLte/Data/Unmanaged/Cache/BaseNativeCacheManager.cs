using System;
using System.Collections.Generic;
using System.Text;
using EcsLte.Utilities;

namespace EcsLte.Data.Unmanaged.Cache
{
	public class BaseNativeCacheManager<T> where T : unmanaged, IDisposable
	{
		private Queue<T> _cacheQueue;
		private Func<T> _allocAction;
		private Action<T> _freeAction;

		protected BaseNativeCacheManager(Func<T> allocAction, Action<T> freeAction)
		{
			_cacheQueue = new Queue<T>();
			_allocAction = allocAction;
			_freeAction = freeAction;
		}

		public T Get()
		{
			T dataChunk;
			if (_cacheQueue.Count > 0)
				dataChunk = _cacheQueue.Dequeue();
			else
				dataChunk = _allocAction.Invoke();

			return dataChunk;
		}

		public unsafe T* GetPtr()
		{
			T dataChunk;
			if (_cacheQueue.Count > 0)
				dataChunk = _cacheQueue.Dequeue();
			else
				dataChunk = _allocAction.Invoke();

			return &dataChunk;
		}

		public void Cache(ref T item)
		{
			_cacheQueue.Enqueue(item);
			if (EcsSettings.ClearUnmanagedCacheCount > 0 &&
				EcsSettings.ClearUnmanagedCacheCount == _cacheQueue.Count)
			{
				for (int i = 0; i < EcsSettings.ClearUnmanagedCacheCount; i++)
					_freeAction.Invoke(_cacheQueue.Dequeue());
			}
		}
	}
}
