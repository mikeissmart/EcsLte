using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data.Unmanaged
{
	public struct SafePtr
	{
		public unsafe void* Ptr;
		public int LengthInBytes;
    }
}
