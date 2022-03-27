using EcsLte.Data.Unmanaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BencharkTest.Data.Unmanaged
{
	public struct NativeItem1
	{
		public int Id { get; set; }
	}

	public struct NativeItem2
	{
		public int Id { get; set; }
		public int Id2 { get; set; }
	}

	public struct NativeItem3
	{
		public int Id { get; set; }
		public int Id2 { get; set; }
		public int Id3 { get; set; }
	}
}
