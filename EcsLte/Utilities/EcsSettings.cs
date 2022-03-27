using System;

namespace EcsLte.Utilities
{
	public class EcsSettings
	{
		private static int _initialEntityCapacity = 4;
		private static int _componentUnmanagedDataChunkInBytes = 16372; // 16384 (16KB - 12B)
        private static int _componentUnmanagedDataChunkClearCache = 128;
        private static bool _isObjectCacheEnabled = true;

		public static int InitialComponentEntityCapacity
		{
			get => _initialEntityCapacity;
			set
			{
				if (value < 4)
					throw new ArgumentOutOfRangeException("value", "Must be greater than 4.");
				_initialEntityCapacity = value;
			}
		}

		public static int UnmanagedDataChunkInBytes
		{
			get => _componentUnmanagedDataChunkInBytes;
			set {
				if (value < 1024)
					throw new ArgumentOutOfRangeException("value", "Must be greater than 1024 (1K).");
				if (value > 1073741824)
					throw new ArgumentOutOfRangeException("value", "Must be less than 1073741824 (1G).");
				_componentUnmanagedDataChunkInBytes = value;
			}
		}

		public static int ClearUnmanagedCacheCount
		{
			get => _componentUnmanagedDataChunkClearCache;
			set {
				if (value < -1)
					throw new ArgumentOutOfRangeException("value", "Use -1 to disable.");
				if (value == 0)
					throw new ArgumentOutOfRangeException("value", "Cannot be 0.");
				_componentUnmanagedDataChunkClearCache = value;
			}
		}

		public static bool IsObjectCacheEnabled
		{
			get => _isObjectCacheEnabled;
			set => _isObjectCacheEnabled = value;
		}
	}
}
