namespace EcsLte.BencharkTest.Data.Unmanaged
{
    /*[MemoryDiagnoser]
    [NativeMemoryProfiler]
    public class Unmanaged_MemoryAlloc
	{
        private unsafe byte** _chunks;
        private unsafe byte* _data;
        private unsafe byte* _dataSmart;
        private int _chunksLength;
        private int _dataLength;
        private int _dataSmartLength;
        private readonly int _goalByteLength = 1000 * 1000;

        [IterationSetup]
        public void Setup()
        {
            unsafe
            {
                _dataSmartLength = 10 * 1000;
                _dataSmart = MemoryHelper.Alloc<byte>(_dataSmartLength);
            }
        }

		[IterationCleanup]
		public void Cleanup()
		{
            unsafe
            {
                if (_chunksLength != 0)
                {
                    for (int i = 0; i < _chunksLength; i++)
                        MemoryHelper.Free(_chunks[i]);
                    MemoryHelper.Free(_chunks);
                }
                if (_dataLength != 0)
                {
                    MemoryHelper.Free(_data);
                }
                if (_dataSmartLength != 0)
                {
                    MemoryHelper.Free(_dataSmart);
                }

                _chunks = null;
                _data = null;
                _dataSmart = null;
                _chunksLength = 0;
                _dataLength = 0;
                _dataSmartLength = 0;
            }
        }

		[Benchmark]
        public void Single_ChunkAlloc()
        {
            unsafe
            {
                _chunksLength = 1000;
                _chunks = (byte**)MemoryHelper.Alloc(Marshal.SizeOf(typeof(byte**)) * _chunksLength);

                for (int i = 0; i < _chunksLength; i++)
                    _chunks[i] = MemoryHelper.Alloc<byte>(1000);
            }
        }

        [Benchmark]
        public void Single_DataAlloc()
        {
            unsafe
            {
                _dataLength = 1000 * 1000;
                _data = MemoryHelper.Alloc<byte>(_dataLength);
            }
        }

        [Benchmark]
        public void Many_ChunkAlloc()
        {
            unsafe
            {
                while (_chunksLength * 1000 < _goalByteLength)
                {
                    if (_chunksLength == 0)
                    {
                        _chunks = (byte**)MemoryHelper.Alloc(Marshal.SizeOf(typeof(byte**)));
                        _chunks[0] = MemoryHelper.Alloc<byte>(1000);
                        _chunksLength = 1;
                    }
                    else
                    {
                        _chunks = (byte**)MemoryHelper.Realloc(
                            _chunks,
                            _chunksLength * Marshal.SizeOf(typeof(byte**)),
                            (_chunksLength + 1) * Marshal.SizeOf(typeof(byte**)));

                        _chunks[_chunksLength] = MemoryHelper.Alloc<byte>(1000);
                        _chunksLength++;
                    }
                }
            }
        }

        [Benchmark]
        public void Many_DataAlloc()
        {
            unsafe
            {
                while (_dataLength < _goalByteLength)
                {
                    if (_dataLength == 0)
                    {
                        _dataLength = 1000;
                        _data = MemoryHelper.Alloc<byte>(_dataLength);
                    }
                    else
                    {
                        var newLength = _dataLength * 2;
                        _data = (byte*)MemoryHelper.Realloc(
                            _data,
                            _dataLength * Marshal.SizeOf(typeof(byte*)),
                            newLength * Marshal.SizeOf(typeof(byte*)));

                        _dataLength = newLength;
                    }
                }
            }
        }

        [Benchmark]
        public void Many_DataSmartAlloc()
        {
            unsafe
            {
                while (_dataSmartLength < _goalByteLength)
                {
                    if (_dataSmartLength == 0)
                    {
                        _dataSmartLength = 1000;
                        _dataSmart = MemoryHelper.Alloc<byte>(_dataSmartLength);
                    }
                    else
                    {
                        var newCapacity = (int)Math.Pow(2, (int)Math.Log(_goalByteLength, 2) + 1);
                        _dataSmart = (byte*)MemoryHelper.Realloc(
                            _dataSmart,
                            _dataSmartLength * Marshal.SizeOf(typeof(byte)),
                            newCapacity * Marshal.SizeOf(typeof(byte)));

                        _dataSmartLength = newCapacity;
                    }
                }
            }
        }
    }*/
}
