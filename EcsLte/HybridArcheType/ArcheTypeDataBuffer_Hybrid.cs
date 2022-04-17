namespace EcsLte.HybridArcheType
{
    /*internal unsafe struct ArcheTypeDataBuffer_Hybrid : IDisposable
    {
        internal byte* Buffer;
        internal long BufferSizeInBytes;
        internal int ComponentsOffsetInBytes;
        internal int EntityCount;
        internal int EntityLength;
        internal int EntityUnusedCount { get => EntityLength - EntityCount; }

        internal void Resize(int count, int batchGrowthLength, int componentsSizeInBytes)
        {
            if (count > EntityUnusedCount)
            {
                int newEntityLength;
                if (EntityLength == 0 && count == 1)
                    // Could have unique components
                    newEntityLength = 1;
                else
                {
                    newEntityLength = (((EntityLength + count) / batchGrowthLength + ((EntityLength + count) % batchGrowthLength > 0 ? 1 : 0))
                        * batchGrowthLength);
                }
                var newBufferLengthInBytes = newEntityLength *
                    (TypeCache<Entity>.SizeInBytes + componentsSizeInBytes);
                var newComponentsOffsetInBytes = newEntityLength * TypeCache<Entity>.SizeInBytes;
                var newBuffer = MemoryHelper.Alloc<byte>(newBufferLengthInBytes);
                if (Buffer != null)
                {
                    MemoryHelper.Copy(
                        Buffer,
                        newBuffer,
                        ComponentsOffsetInBytes);
                    MemoryHelper.Copy(
                        Buffer + ComponentsOffsetInBytes,
                        newBuffer + newComponentsOffsetInBytes,
                        EntityLength * componentsSizeInBytes);
                    MemoryHelper.Free(Buffer);
                }
                Buffer = newBuffer;
                BufferSizeInBytes = newBufferLengthInBytes;
                ComponentsOffsetInBytes = newComponentsOffsetInBytes;
                EntityLength = newEntityLength;
            }
        }

        internal int GetEntityOffset(int entityIndex)
        {
            return entityIndex * TypeCache<Entity>.SizeInBytes;
        }

        internal unsafe Entity GetEntity(int entityIndex)
        {
            return *(Entity*)(Buffer + GetEntityOffset(entityIndex));
        }

        internal unsafe void CopyEntity(Entity entity, int entityIndex)
        {
            *(Entity*)(Buffer + GetEntityOffset(entityIndex)) = entity;
        }

        internal int GetComponentsOffset(int entityIndex, int componentsSizeInBytes)
        {
            return ComponentsOffsetInBytes + (entityIndex * componentsSizeInBytes);
        }

        internal unsafe byte* GetComponents(int entityIndex, int componentsSizeInBytes)
        {
            return Buffer + GetComponentsOffset(entityIndex, componentsSizeInBytes);
        }

        public unsafe void Dispose()
        {
            if (Buffer != null)
                MemoryHelper.Free(Buffer);
            Buffer = null;
            BufferSizeInBytes = 0;
            EntityCount = 0;
            EntityLength = 0;
        }
    }*/
}
