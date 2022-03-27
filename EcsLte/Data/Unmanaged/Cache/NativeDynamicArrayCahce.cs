using System;
using System.Collections.Generic;
using System.Text;
using EcsLte.Utilities;

namespace EcsLte.Data.Unmanaged.Cache
{
	public class NativeDynamicArrayCahce : BaseNativeCacheManager<NativeDynamicArray>
	{
		private static NativeDynamicArrayCahce _instance;

		public static NativeDynamicArrayCahce Instance
		{
			get
			{
				if (_instance == null)
					_instance = new NativeDynamicArrayCahce();
				return _instance;
			}
		}

		private NativeDynamicArrayCahce() : base(Alloc, x => Free(x))
		{

		}

		private static NativeDynamicArray Alloc()
		{
			return NativeDynamicArray.Alloc(EcsSettings.UnmanagedDataChunkInBytes);
		}

		public static void Free(NativeDynamicArray array)
		{
			array.Dispose();
		}
	}
}
