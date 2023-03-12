using EcsLte.Data;
using EcsLte.Utilities;
using System;

namespace EcsLte
{
    /*internal interface IComponentPool
    {
        int Length { get; }

        IComponent GetComponent(int chunkIndex, int entityIndex);
        void SetComponent(int chunkIndex, int entityIndex, in IComponent component);
        void SetComponents(int poolIndex, int entityCount, in IComponent component);
        void CopyFromSame(int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount);
        void CopyFromSameSingle(int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex);
        void CopyFromSame(int srcPoolIndex, int destPoolIndex,
            int entityCount);
        void CopyFromDifferent(IComponentPool srcPool, int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount);
        void CopyFromDifferent(IComponentPool srcPool, int srcPoolIndex, int destPoolIndex,
            int entityCount);
        void ClearPool(int srcPoolIndex, int entityCount);
        void ClearComponents(int chunkIndex, int entityIndex, int entityCount);
        void Resize(int newSize);
    }

    internal class ComponentPool<TComponent> : IComponentPool
        where TComponent : IManagedComponent
    {
        private TComponent[] _components;

        public ComponentPool() => _components = new TComponent[1];

        public int Length => _components.Length;

        IComponent IComponentPool.GetComponent(int chunkIndex, int entityIndex)
            => GetComponent(chunkIndex, entityIndex);

        public TComponent GetComponent(int chunkIndex, int entityIndex)
            => _components[Chunk(chunkIndex) + entityIndex];

        void IComponentPool.SetComponent(int chunkIndex, int entityIndex, in IComponent component)
            => SetComponent(chunkIndex, entityIndex, (TComponent)component);

        public void SetComponent(int chunkIndex, int entityIndex, in TComponent component)
            => _components[Chunk(chunkIndex) + entityIndex] = component;

        void IComponentPool.SetComponents(int poolIndex, int entityCount, in IComponent component)
            => SetComponents(poolIndex, entityCount, (TComponent)component);

        public void SetComponents(int poolIndex, int entityCount, in TComponent component)
        {
            for (var i = 0; i < entityCount; i++, poolIndex++)
                _components[poolIndex] = component;
        }

        public void CopyFromSame(int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount)
        {
            srcChunkIndex = Chunk(srcChunkIndex) + srcEntityIndex;
            Helper.ArrayCopy(_components,
                srcChunkIndex,
                _components,
                Chunk(destChunkIndex) + destEntityIndex,
                entityCount);
            Array.Clear(_components, srcChunkIndex, entityCount);
        }

        public void CopyFromSameSingle(int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex)
        {
            srcChunkIndex = Chunk(srcChunkIndex) + srcEntityIndex;
            _components[Chunk(destChunkIndex) + destEntityIndex] = _components[srcChunkIndex];
            _components[srcChunkIndex] = default;
        }

        public void CopyFromSame(int srcPoolIndex, int destPoolIndex,
            int entityCount)
        {
            Helper.ArrayCopy(_components,
                srcPoolIndex,
                _components,
                destPoolIndex,
                entityCount);
            Array.Clear(_components, srcPoolIndex, entityCount);
        }

        void IComponentPool.CopyFromDifferent(IComponentPool srcPool, int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount)
            => CopyFromDifferent((ComponentPool<TComponent>)srcPool, srcChunkIndex, srcEntityIndex,
                destChunkIndex, destEntityIndex, entityCount);

        public void CopyFromDifferent(ComponentPool<TComponent> srcPool, int srcChunkIndex, int srcEntityIndex,
            int destChunkIndex, int destEntityIndex, int entityCount)
        {
            Helper.ArrayCopy(srcPool._components,
                Chunk(srcChunkIndex) + srcEntityIndex,
                _components,
                Chunk(destChunkIndex) + destEntityIndex,
                entityCount);
            Array.Clear(srcPool._components, Chunk(srcChunkIndex) + srcEntityIndex, entityCount);
        }

        void IComponentPool.CopyFromDifferent(IComponentPool srcPool, int srcPoolIndex, int destPoolIndex,
            int entityCount)
            => CopyFromDifferent((ComponentPool<TComponent>)srcPool, srcPoolIndex, destPoolIndex,
                entityCount);

        public void CopyFromDifferent(ComponentPool<TComponent> srcPool, int srcPoolIndex, int destPoolIndex,
            int entityCount)
        {
            Helper.ArrayCopy(srcPool._components,
                srcPoolIndex,
                _components,
                destPoolIndex,
                entityCount);
            Array.Clear(srcPool._components, srcPoolIndex, entityCount);
        }

        public void ClearPool(int srcPoolIndex, int entityCount)
            => Array.Clear(_components, srcPoolIndex, entityCount);

        public void ClearComponents(int chunkIndex, int entityIndex, int entityCount)
            => Array.Clear(_components, Chunk(chunkIndex) + entityIndex, entityCount);

        public void Resize(int newSize)
            => Array.Resize(ref _components, newSize);

        public ref TComponent GetComponentRef(EntityData entityData)
            => ref _components[Chunk(entityData.ChunkIndex) + entityData.EntityIndex];

        public void GetAllComponents(ref TComponent[] components, int startingIndex, int entityCount)
            => Helper.ArrayCopy(_components,
                0,
                components,
                startingIndex,
                entityCount);

        public void GetChunkComponents(ref TComponent[] components, int chunkIndex, int startingIndex, int chunkEntityCount)
            => Helper.ArrayCopy(_components,
                Chunk(chunkIndex),
                components,
                startingIndex,
                chunkEntityCount);

        private int Chunk(int chunkIndex)
            => chunkIndex * DataManager.PageCapacity;
    }*/
}
