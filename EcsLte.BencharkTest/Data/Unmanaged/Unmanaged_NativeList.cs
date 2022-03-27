using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using EcsLte.Data.Unmanaged;

namespace EcsLte.BencharkTest.Data.Unmanaged
{
	/*[MemoryDiagnoser]
	public class Unmanaged_NativeList_Add
	{
		private NativeList _nativeList;
		private List<NativeItem1> _list;

		[IterationSetup]
		public void Setup()
		{
			_nativeList = NativeList.Alloc<NativeItem1>();
			_list = new List<NativeItem1>();
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeList.Dispose();
			_list = null;
		}

		[Benchmark]
		public void AddNative()
		{
			for (int i = 0; i < TestConsts.LargeCount; i++)
				_nativeList.Add(new NativeItem1());
		}

		[Benchmark(Baseline = true)]
		public void Add()
		{
			for (int i = 0; i < TestConsts.LargeCount; i++)
				_list.Add(new NativeItem1());
		}
	}

	[MemoryDiagnoser]
	public class Unmanaged_NativeList_Get
	{
		private NativeList _nativeList;
		private List<NativeItem1> _list;

		[IterationSetup]
		public void Setup()
		{
			_nativeList = NativeList.Alloc<NativeItem1>();
			_list = new List<NativeItem1>();

			var item = new NativeItem1();
			for (int i = 0; i < TestConsts.LargeCount; i++)
			{
				_nativeList.Add(item);
				_list.Add(item);
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeList.Dispose();
			_list = null;
		}

		[Benchmark]
		public void GetNative()
		{
			NativeItem1 item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = _nativeList.Get<NativeItem1>(i);
		}

		[Benchmark(Baseline = true)]
		public void Get()
		{
			NativeItem1 item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = _list[i];
		}
	}

	[MemoryDiagnoser]
	public class Unmanaged_NativeList_Clear
	{
		private NativeList _nativeList;
		private List<NativeItem1> _list;

		[IterationSetup]
		public void Setup()
		{
			_nativeList = NativeList.Alloc<NativeItem1>();
			_list = new List<NativeItem1>();

			var item = new NativeItem1();
			for (int i = 0; i < TestConsts.LargeCount; i++)
			{
				_nativeList.Add(item);
				_list.Add(item);
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeList.Dispose();
			_list = null;
		}

		[Benchmark]
		public void ClearNative()
		{
			_nativeList.Clear();
		}

		[Benchmark(Baseline = true)]
		public void Clear()
		{
			_list.Clear();
		}
	}*/
}
