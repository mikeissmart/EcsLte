namespace EcsLte.BencharkTest.Data.Unmanaged
{
    /*[MemoryDiagnoser]
	[BenchmarkCategory("Alloc")]
	public class Unmanaged_NativeArray_Alloc
	{
		private readonly int _arrayCount = BenchmarkTestConsts.MediumCount;
		private readonly int _itemCount = BenchmarkTestConsts.SmallCount;
		private NativeArray _nativeArray;
		private NativeArrayPtr _nativeArrayPtr;
		private NativeItem1[][] _array;
		private unsafe NativeItem1** _ptrArray;

		[IterationSetup]
		public void Setup()
		{
			_nativeArray = NativeArray.Alloc<NativeArray>(_arrayCount);
			_nativeArrayPtr = NativeArrayPtr.Alloc<NativeArrayPtr>(_arrayCount);
			_array = new NativeItem1[_arrayCount][];
			unsafe
			{
				int sizeInBytes = Marshal.SizeOf(typeof(NativeItem1*)) * _arrayCount;
				_ptrArray = (NativeItem1**)Marshal.AllocHGlobal(sizeInBytes);
				Unsafe.InitBlock(_ptrArray, 0, (uint)sizeInBytes);
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			unsafe
			{
				int sizeInBytes = Marshal.SizeOf(typeof(NativeItem1*));
				for (int i = 0; i < _arrayCount; i++)
				{
					_nativeArray.Get<NativeArray>(i).Dispose();
					_nativeArrayPtr.Get<NativeArrayPtr>(i).Dispose();
					_array[i] = null;
					if (_ptrArray[i] != null)
						Marshal.FreeHGlobal((IntPtr)_ptrArray[i]);
				}
				_nativeArray.Dispose();
				_nativeArrayPtr.Dispose();
				_array = null;
				Marshal.FreeHGlobal((IntPtr)_ptrArray);
			}
		}

		/*[Benchmark]
		public void AllocNative()
		{
			for (int i = 0; i < _arrayCount; i++)
				_nativeArray.Set(i, NativeArray.Alloc<NativeItem1>(_itemCount));
		}* /

		[Benchmark]
		public void AllocNativeArrayPtr()
		{
			for (int i = 0; i < _arrayCount; i++)
			{
				var array = NativeArrayPtr.Alloc<NativeItem1>(_itemCount);
				_nativeArrayPtr.Set(i, array);
			}
		}

		/*[Benchmark(Baseline = true)]
		public void Alloc()
		{
			for (int i = 0; i < _arrayCount; i++)
				_array[i] = new NativeItem1[_itemCount];
		}* /

		[Benchmark]
		public void AllocPtr()
		{
			unsafe
			{
				for (int i = 0; i < _arrayCount; i++)
					_ptrArray[i] = (NativeItem1*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeItem1)) * _itemCount);
			}
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("AllocSetGet")]
	public class Unmanaged_NativeArray_AllocSetGet
	{
		/*[Benchmark]
		public void AllocSetGetNative()
		{
			var item = new NativeItem1();
			var array = NativeArray.Alloc<NativeItem1>(TestConsts.LargeCount);
			for (int i = 0; i < TestConsts.LargeCount; i++)
				array.Set(i, item);
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = array.Get<NativeItem1>(i);
			array.Dispose();
		}* /

		[Benchmark]
		public void AllocSetGetNativePtr()
		{
			var item = new NativeItem1();
			var array = NativeArrayPtr.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				array.Set(i, item);
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				item = array.Get<NativeItem1>(i);
			array.Dispose();
		}

		/*[Benchmark(Baseline = true)]
		public void AllocSetGet()
		{
			var item = new NativeItem1();
			var array = new NativeItem1[TestConsts.LargeCount];
			for (int i = 0; i < TestConsts.LargeCount; i++)
				array[i] = item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = array[i];
			array = null;
		}* /

		[Benchmark]
		public unsafe void AllocSetGetPtr()
		{
			var item = new NativeItem1();
			var array = (NativeItem1*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeItem1)) * (BenchmarkTestConsts.LargeCount));
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				array[i] = item;
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				item = array[i];
			Marshal.FreeHGlobal((IntPtr)array);
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("Set")]
	public class Unmanaged_NativeArray_Set
	{
		private NativeArray _nativeArray;
		private NativeArrayPtr _nativeArrayPtr;
		private NativeItem1[] _array;
		private unsafe NativeItem1* _ptrArray;

		[IterationSetup]
		public void Setup()
		{
			_nativeArray = NativeArray.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_nativeArrayPtr = NativeArrayPtr.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_array = new NativeItem1[BenchmarkTestConsts.LargeCount];
			unsafe
			{
				_ptrArray = (NativeItem1*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeItem1)) * BenchmarkTestConsts.LargeCount);
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeArray.Dispose();
			_nativeArrayPtr.Dispose();
			_array = null;
			unsafe
            {
				Marshal.FreeHGlobal((IntPtr)_ptrArray);
            }
		}

		/*[Benchmark]
		public void SetNative()
		{
			var item = new NativeItem1();
			for (int i = 0; i < TestConsts.LargeCount; i++)
				_nativeArray.Set(i, item);
		}* /

		[Benchmark]
		public void SetNativePtr()
		{
			var item = new NativeItem1();
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				_nativeArrayPtr.Set(i, item);
		}

		/*[Benchmark(Baseline = true)]
		public void Set()
		{
			var item = new NativeItem1();
			for (int i = 0; i < TestConsts.LargeCount; i++)
				_array[i] = item;
		}* /

		[Benchmark]
		public void SetPtr()
		{
			unsafe
			{
				var item = new NativeItem1();
				for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
					_ptrArray[i] = item;
			}
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("Get")]
	public class Unmanaged_NativeArray_Get
	{
		private NativeArray _nativeArray;
		private NativeArrayPtr _nativeArrayPtr;
		private NativeItem1[] _array;
		private unsafe NativeItem1* _ptrArray;

		[IterationSetup]
		public void Setup()
		{
			_nativeArray = NativeArray.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_nativeArrayPtr = NativeArrayPtr.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_array = new NativeItem1[BenchmarkTestConsts.LargeCount];
			unsafe
			{
				_ptrArray = (NativeItem1*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeItem1)) * BenchmarkTestConsts.LargeCount);

				var item = new NativeItem1();
				for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				{
					_nativeArray.Set(i, item);
					_nativeArrayPtr.Set(i, item);
					_array[i] = item;
					_ptrArray[i] = new NativeItem1();
				}
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeArray.Dispose();
			_nativeArrayPtr.Dispose();
			_array = null;
			unsafe
			{
				Marshal.FreeHGlobal((IntPtr)_ptrArray);
			}
		}

		/*[Benchmark]
		public void GetNative()
		{
			NativeItem1 item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = _nativeArray.Get<NativeItem1>(i);
		}* /

		[Benchmark]
		public void GetNativePtr()
		{
			NativeItem1 item;
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				item = _nativeArrayPtr.Get<NativeItem1>(i);
		}

		/*[Benchmark(Baseline = true)]
		public void Get()
		{
			NativeItem1 item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = _array[i];
		}* /

		[Benchmark]
		public void GetPtr()
		{
			unsafe
			{
				NativeItem1 item;
				for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
					item = _ptrArray[i];
			}
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("GetRandom")]
	public class Unmanaged_NativeArray_GetRandom
	{
		private int[] _random;
		private NativeArray _nativeArray;
		private NativeArrayPtr _nativeArrayPtr;
		private NativeItem1[] _array;
		private unsafe NativeItem1* _ptrArray;

		[IterationSetup]
		public void Setup()
		{
			var random = new Random(DateTime.Now.Millisecond);

			_random = new int[BenchmarkTestConsts.LargeCount];
			_nativeArray = NativeArray.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_nativeArrayPtr = NativeArrayPtr.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_array = new NativeItem1[BenchmarkTestConsts.LargeCount];
			unsafe
			{
				_ptrArray = (NativeItem1*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeItem1)) * BenchmarkTestConsts.LargeCount);

				var item = new NativeItem1();
				for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				{
					_random[i] = random.Next(0, BenchmarkTestConsts.LargeCount);
					_nativeArray.Set(i, item);
					_nativeArrayPtr.Set(i, item);
					_array[i] = item;
					_ptrArray[i] = new NativeItem1();
				}
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeArray.Dispose();
			_nativeArrayPtr.Dispose();
			_array = null;
			unsafe
			{
				Marshal.FreeHGlobal((IntPtr)_ptrArray);
			}
		}

		/*[Benchmark]
		public void GetRandomNative()
		{
			NativeItem1 item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = _nativeArray.Get<NativeItem1>(_random[i]);
		}* /

		[Benchmark]
		public void GetRandomNativePtr()
		{
			NativeItem1 item;
			for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				item = _nativeArrayPtr.Get<NativeItem1>(_random[i]);
		}

		/*[Benchmark(Baseline = true)]
		public void GetRandom()
		{
			NativeItem1 item;
			for (int i = 0; i < TestConsts.LargeCount; i++)
				item = _array[_random[i]];
		}* /

		[Benchmark]
		public void GetRandomPtr()
		{
			unsafe
			{
				NativeItem1 item;
				for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
					item = _ptrArray[_random[i]];
			}
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("Copy")]
	public class Unmanaged_NativeArray_Copy
	{
		private int _nativeItem1SizeInBytes;
		private int _nativeItem1SizeInBytesLargeCount;
		private NativeArray _nativeArray1;
		private NativeArray _nativeArray2;
		private NativeArrayPtr _nativeArrayPtr1;
		private NativeArrayPtr _nativeArrayPtr2;
		private NativeItem1[] _array1;
		private NativeItem1[] _array2;
		private unsafe NativeItem1* _ptrArray1;
		private unsafe NativeItem1* _ptrArray2;

		[IterationSetup]
		public void Setup()
		{
			_nativeItem1SizeInBytes = Marshal.SizeOf(typeof(NativeItem1));
			_nativeItem1SizeInBytesLargeCount = _nativeItem1SizeInBytes * BenchmarkTestConsts.LargeCount;

			_nativeArray1 = NativeArray.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_nativeArray2 = NativeArray.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_nativeArrayPtr1 = NativeArrayPtr.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_nativeArrayPtr2 = NativeArrayPtr.Alloc<NativeItem1>(BenchmarkTestConsts.LargeCount);
			_array1 = new NativeItem1[BenchmarkTestConsts.LargeCount];
			_array2 = new NativeItem1[BenchmarkTestConsts.LargeCount];
			unsafe
			{
				_ptrArray1 = (NativeItem1*)Marshal.AllocHGlobal(_nativeItem1SizeInBytesLargeCount);
				_ptrArray2 = (NativeItem1*)Marshal.AllocHGlobal(_nativeItem1SizeInBytesLargeCount);

				var item = new NativeItem1();
				for (int i = 0; i < BenchmarkTestConsts.LargeCount; i++)
				{
					item.Id = i;
					_nativeArray1.Set(i, item);
					_nativeArrayPtr1.Set(i, item);
					_array1[i] = item;
					_ptrArray1[i] = item;
				}
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			_nativeArray1.Dispose();
			_nativeArray2.Dispose();
			_nativeArrayPtr1.Dispose();
			_nativeArrayPtr2.Dispose();
			_array1 = null;
			_array2 = null;
			unsafe
			{
				Marshal.FreeHGlobal((IntPtr)_ptrArray1);
				Marshal.FreeHGlobal((IntPtr)_ptrArray2);
			}
		}

		[Benchmark]
		public void CopyNative()
		{
			_nativeArray1.CopyTo(0, ref _nativeArray2, 0, BenchmarkTestConsts.LargeCount);
		}

		[Benchmark]
		public void CopyNativePtr()
		{
			_nativeArrayPtr1.CopyTo(0, ref _nativeArrayPtr2, 0, BenchmarkTestConsts.LargeCount);
		}

		[Benchmark(Baseline = true)]
		public void Copy()
		{
			Array.Copy(_array1, _array2, BenchmarkTestConsts.LargeCount);
		}

		[Benchmark]
		public unsafe void CopyPtr()
		{
			Buffer.MemoryCopy(_ptrArray1, _ptrArray2, _nativeItem1SizeInBytesLargeCount, _nativeItem1SizeInBytesLargeCount);
		}
	}*/
}
