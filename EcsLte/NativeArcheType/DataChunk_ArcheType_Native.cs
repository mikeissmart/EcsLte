using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using EcsLte.Data.Unmanaged.Cache;
using EcsLte.Utilities;

namespace EcsLte.NativeArcheType
{
	[StructLayout(LayoutKind.Explicit)]
	public struct DataChunk_ArcheType_Native
	{
		[FieldOffset(0)]
		public int ChunkLengthInBytes;
		[FieldOffset(4)]
		public int Count;
		[FieldOffset(8)]
		public unsafe byte* Buffer;

		public bool IsFull(int capacity)
		{
			return Count == capacity;
		}

		public unsafe void Clear()
		{
			MemoryHelper.Clear(Buffer, ChunkLengthInBytes);
		}
	}
}
