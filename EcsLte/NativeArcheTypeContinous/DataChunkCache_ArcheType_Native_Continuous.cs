using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;

namespace EcsLte.NativeArcheTypeContinous
{
    public struct DataChunkCache_ArcheType_Native_Continuous : IDisposable
    {
        private static readonly int _dataChunkInitLength = 4;

        private unsafe byte* _dataChunks;
        private int _dataChunksLength;
        private int _dataChunkSizeInBytes;
        private unsafe int* _unusedDataChunkIndexes;
        private int _unusedDataChunksCount;
        private int _unusedDataChunksLength;

        public static unsafe DataChunkCache_ArcheType_Native_Continuous* Alloc()
        {
            var data = MemoryHelper.Alloc<DataChunkCache_ArcheType_Native_Continuous>(1);
            data->Initialize();

            return data;
        }

        public unsafe DataChunk_ArcheType_Native_Continuous* GetDataChunk(int index) => (DataChunk_ArcheType_Native_Continuous*)(_dataChunks + (index * _dataChunkSizeInBytes));

        public unsafe int GetDataChunkIndex()
        {
            CheckCapacity(1);

            return _unusedDataChunkIndexes[--_unusedDataChunksCount];
        }

        public unsafe void CacheDataChunkIndex(int dataChunkIndex)
        {
            CheckUnusedCapacity(1);

            MemoryHelper.Clear(_dataChunks + (dataChunkIndex * _dataChunkSizeInBytes), _dataChunkSizeInBytes);
            _unusedDataChunkIndexes[_unusedDataChunksCount++] = dataChunkIndex;
            var a = GetDataChunk(dataChunkIndex);
        }

        public unsafe void CacheDataChunkIndexes(params int[] dataChunkIndexes)
        {
            CheckUnusedCapacity(dataChunkIndexes.Length);

            foreach (var index in dataChunkIndexes)
            {
                MemoryHelper.Clear(_dataChunks + (index * _dataChunkSizeInBytes), _dataChunkSizeInBytes);
                _unusedDataChunkIndexes[_unusedDataChunksCount++] = index;
            }
        }

        public unsafe void CacheDataChunkIndexes(int* dataChunkIndexes, int length)
        {
            CheckUnusedCapacity(length);

            for (var i = 0; i < length; i++)
            {
                var index = dataChunkIndexes[i];
                MemoryHelper.Clear(_dataChunks + (index * _dataChunkSizeInBytes), _dataChunkSizeInBytes);
                _unusedDataChunkIndexes[_unusedDataChunksCount++] = index;
            }
        }

        private unsafe void Initialize()
        {
            _dataChunkSizeInBytes = EcsSettings.UnmanagedDataChunkInBytes + TypeCache<DataChunk_ArcheType_Native_Continuous>.SizeInBytes;
            _dataChunks = (byte*)MemoryHelper.Alloc(_dataChunkInitLength * _dataChunkSizeInBytes);
            _dataChunksLength = _dataChunkInitLength;

            _unusedDataChunkIndexes = MemoryHelper.Alloc<int>(_dataChunkInitLength);
            _unusedDataChunksCount = _dataChunkInitLength;
            _unusedDataChunksLength = _dataChunkInitLength;

            for (var i = 0; i < _dataChunkInitLength; i++)
                _unusedDataChunkIndexes[i] = i;
        }

        private unsafe void CheckCapacity(int count)
        {
            if (_unusedDataChunksCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_dataChunksLength + count, 2) + 1);
                _dataChunks = (byte*)MemoryHelper.Realloc(
                    _dataChunks,
                    _dataChunksLength * _dataChunkSizeInBytes,
                    newCapacity * _dataChunkSizeInBytes);

                CheckUnusedCapacity(newCapacity - _dataChunksLength);

                for (var i = _dataChunksLength; i < newCapacity; i++)
                    _unusedDataChunkIndexes[_unusedDataChunksCount++] = i;

                _dataChunksLength = newCapacity;
            }
        }

        private unsafe void CheckUnusedCapacity(int count)
        {
            var unusedCount = _unusedDataChunksLength - _unusedDataChunksCount;
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_unusedDataChunksLength + count, 2) + 1);
                _unusedDataChunkIndexes = (int*)MemoryHelper.Realloc(
                    _unusedDataChunkIndexes,
                    _unusedDataChunksLength * TypeCache<int>.SizeInBytes,
                    newCapacity * TypeCache<int>.SizeInBytes);

                _unusedDataChunksLength = newCapacity;
            }
        }

        public unsafe void Dispose()
        {
            _dataChunkSizeInBytes = 0;
            MemoryHelper.Free(_dataChunks);
            _dataChunks = null;
            _dataChunksLength = 0;
            MemoryHelper.Free(_unusedDataChunkIndexes);
            _unusedDataChunkIndexes = null;
            _unusedDataChunksCount = 0;
        }
    }
}
