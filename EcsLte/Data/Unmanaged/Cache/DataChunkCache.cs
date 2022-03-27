using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data.Unmanaged.Cache
{
    /*public class DataChunkCache : BaseNativeCacheManager<DataChunk>
	{
		private static DataChunkCache _instance;

		public static DataChunkCache Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DataChunkCache();
				return _instance;
			}
		}

		private DataChunkCache() : base(Alloc, x => Free(x))
		{

		}

		private static DataChunk Alloc()
		{
			var data = new DataChunk
			{
				Data = NativeDynamicArray.Alloc(EcsSettings.UnmanagedComponentDataChunkInBytes)
			};

			return data;
		}

		public static void Free(DataChunk data)
		{
			data.Dispose();
		}
	}*/
}
