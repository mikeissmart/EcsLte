using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal unsafe struct ComponentBuffer
    {
        internal byte* Buffer;
        internal int ComponentSize;

        internal byte* PtrChunk(int chunkIndex)
            => Buffer + (chunkIndex * ArcheTypeDataChunk.ChunkMaxCapacity * ComponentSize);

        internal byte* PtrComponent(int chunkIndex, int entityIndex)
            => PtrChunk(chunkIndex) + (ComponentSize * entityIndex);

        internal void CopyFromSame(int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount)
            => MemoryHelper.Copy(
                PtrComponent(srcChunkIndex, srcEntityIndex),
                PtrComponent(destChunkIndex, destEntityIndex),
                ComponentSize * entityCount);

        internal void CopyFromSame(int srcBufferIndex, int destBufferIndex,
            int entityCount)
            => MemoryHelper.Copy(
                Buffer + (srcBufferIndex * ComponentSize),
                Buffer + (destBufferIndex * ComponentSize),
                ComponentSize * entityCount);

        internal void CopyFromDifferent(ComponentBuffer srcBuffer, int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount)
            => MemoryHelper.Copy(
                srcBuffer.PtrComponent(srcChunkIndex, srcEntityIndex),
                PtrComponent(destChunkIndex, destEntityIndex),
                ComponentSize * entityCount);

        internal void CopyFromDifferent(ComponentBuffer srcBuffer, int srcBufferIndex, int destBufferIndex,
            int entityCount)
            => MemoryHelper.Copy(
                srcBuffer.Buffer + srcBufferIndex,
                Buffer + destBufferIndex,
                ComponentSize * entityCount);

        internal void ClearBuffer(int srcBufferIndex, int entityCount)
            => MemoryHelper.Clear(Buffer + srcBufferIndex, ComponentSize * entityCount);

        internal void ClearComponents(int chunkIndex, int entityIndex, int entityCount)
            => MemoryHelper.Clear(PtrComponent(chunkIndex, entityIndex), ComponentSize * entityCount);
    }
}
