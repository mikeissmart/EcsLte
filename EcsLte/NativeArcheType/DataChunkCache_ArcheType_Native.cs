using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte.NativeArcheType
{
    public class DataChunkCache_ArcheType_Native : IDisposable
    {
        private readonly Queue<IntPtr> _cacheQueue;

        public DataChunkCache_ArcheType_Native() => _cacheQueue = new Queue<IntPtr>();

        public unsafe DataChunk_ArcheType_Native* GetDataChunk(bool clear)
        {
            DataChunk_ArcheType_Native* dataChunk;
            if (_cacheQueue.Count > 0)
            {
                dataChunk = (DataChunk_ArcheType_Native*)_cacheQueue.Dequeue();
                dataChunk->Count = 0;
                if (clear)
                    MemoryHelper.Clear(dataChunk->Buffer, dataChunk->ChunkLengthInBytes);
            }
            else
            {
                var dataChunkLengthInBytes = TypeCache<DataChunk_ArcheType_Native>.SizeInBytes;
                // Accounting for struct props offset
                dataChunk = (DataChunk_ArcheType_Native*)MemoryHelper.Alloc(EcsSettings.UnmanagedDataChunkInBytes + dataChunkLengthInBytes);
                dataChunk->ChunkLengthInBytes = EcsSettings.UnmanagedDataChunkInBytes;
                dataChunk->Count = 0;
                dataChunk->Buffer = ((byte*)dataChunk) + dataChunkLengthInBytes;
            }

            return dataChunk;
        }

        public unsafe void Cache(DataChunk_ArcheType_Native* dataChunk) => _cacheQueue.Enqueue((IntPtr)dataChunk);

        public unsafe void Dispose()
        {
            foreach (var ptr in _cacheQueue)
            {
                MemoryHelper.Free((void*)ptr);
            }
            _cacheQueue.Clear();
        }
    }
}
