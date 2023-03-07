using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal unsafe class ArcheTypeData
    {
        private Entity* _entities;
        private int _entitiesLength;
        private IComponentPool[] _managedPools;
        private ComponentConfigOffset* _configOffsets;
        private ComponentBuffer* _generalBuffers;
        private ArcheTypeDataChunk[] _chunks;

        internal ArcheType ArcheType { get; private set; }
        internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
        internal Entity* Entities { get => _entities; }
        internal int EntityCount { get; private set; }
        internal ArcheTypeDataChunk[] Chunks { get => _chunks; }
        internal int ChunksCount { get; private set; }
        internal ArcheTypeDataChunk LastChunk => _chunks[ChunksCount - 1];
        internal ChangeVersion[] SharedVersions { get; private set; }
        internal ComponentBuffer* GeneralBuffers { get => _generalBuffers; }
        internal IComponentPool[] ManagedPools { get => _managedPools; }
        internal ComponentConfigOffset[] GeneralConfigs { get; private set; }
        internal ComponentConfigOffset[] ManagedConfigs { get; private set; }
        internal ComponentConfigOffset[] SharedConfigs { get; private set; }
        internal ISharedComponentData[] SharedComponentDatas { get; private set; }

        internal ArcheTypeData(ArcheType archeType, ArcheTypeIndex archeTypeIndex, SharedComponentDictionaries sharedDics)
        {
            var generalConfigs = new List<ComponentConfigOffset>();
            var managedConfigs = new List<ComponentConfigOffset>();
            var sharedConfigs = new List<ComponentConfigOffset>();

            var generalIndex = 0;
            var managedIndex = 0;
            var sharedIndex = 0;
            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset>(ComponentConfigs.Instance.AllComponentCount);
            for (var i = 0; i < archeType.ConfigsLength; i++)
            {
                var config = archeType.Configs[i];
                var configOffset = new ComponentConfigOffset
                {
                    Config = config
                };

                if (config.IsGeneral)
                {
                    configOffset.ConfigIndex = generalIndex++;
                    generalConfigs.Add(configOffset);
                }
                else if (config.IsManaged)
                {
                    configOffset.ConfigIndex = managedIndex++;
                    managedConfigs.Add(configOffset);
                }
                else
                {
                    configOffset.ConfigIndex = sharedIndex++;
                    sharedConfigs.Add(configOffset);
                }

                _configOffsets[config.ComponentIndex] = configOffset;
            }

            ArcheType = archeType;
            ArcheTypeIndex = archeTypeIndex;

            GeneralConfigs = generalConfigs.ToArray();
            if (GeneralConfigs.Length > 0)
            {
                _generalBuffers = MemoryHelper.Alloc<ComponentBuffer>(GeneralConfigs.Length);
                for (var i = 0; i < GeneralConfigs.Length; i++)
                {
                    var configOffset = GeneralConfigs[i];
                    _generalBuffers[i] = new ComponentBuffer
                    {
                        Buffer = MemoryHelper.Alloc<byte>(configOffset.Config.UnmanagedSizeInBytes),
                        ComponentSize = configOffset.Config.UnmanagedSizeInBytes
                    };
                }
            }

            ManagedConfigs = managedConfigs.ToArray();
            _managedPools = ComponentConfigs.Instance.CreateComponentPools(ManagedConfigs);

            SharedConfigs = sharedConfigs.ToArray();
            SharedComponentDatas = new ISharedComponentData[SharedConfigs.Length];
            if (SharedConfigs.Length > 0)
            {
                for (var i = 0; i < SharedConfigs.Length; i++)
                {
                    SharedComponentDatas[i] = sharedDics.GetDic(SharedConfigs[i].Config)
                        .GetComponentData(archeType.SharedDataIndexes[i]);
                }
            }
            SharedVersions = new ChangeVersion[SharedConfigs.Length];

            _entities = MemoryHelper.Alloc<Entity>(1);
            _entitiesLength = 1;

            _chunks = new ArcheTypeDataChunk[1];
            _chunks[0] = new ArcheTypeDataChunk(this, 0);
            ChunksCount = 0;
        }

        #region GetChunks

        /// <summary>
        /// Doesnt resize ref chunks
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="startingIndex"></param>
        /// <exception cref="Exception"></exception>
        internal int GetAllChunks(ref ArcheTypeDataChunk[] chunks, int startingIndex)
        {
            Helper.ArrayCopy(_chunks, 0, chunks, startingIndex, ChunksCount);
            return ChunksCount;
        }

        #endregion

        #region TransferEntity

        internal static void TransferEntity(
            ChangeVersion changeVersion,
            Entity entity,
            ArcheTypeData prevArcheTypeData,
            ArcheTypeData nextArcheTypeData,
            EntityData* allEntityDatas)
        {
            var prevEntityData = allEntityDatas[entity.Id];
            var nextEntityData = nextArcheTypeData.AddEntityNoChangeVersion(entity, false);

            for (var i = 0; i < prevArcheTypeData.GeneralConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.GeneralConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextArcheTypeData._generalBuffers[nextConfigOffset.ConfigIndex]
                        .CopyFromDifferent(prevArcheTypeData._generalBuffers[prevConfigOffset.ConfigIndex],
                            prevEntityData.ChunkIndex,
                            prevEntityData.EntityIndex,
                            nextEntityData.ChunkIndex,
                            nextEntityData.EntityIndex,
                            1);
                }
            }
            for (var i = 0; i < prevArcheTypeData.ManagedConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.ManagedConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextArcheTypeData._managedPools[nextConfigOffset.ConfigIndex]
                        .CopyFromDifferent(prevArcheTypeData._managedPools[prevConfigOffset.ConfigIndex],
                            prevEntityData.ChunkIndex,
                            prevEntityData.EntityIndex,
                            nextEntityData.ChunkIndex,
                            nextEntityData.EntityIndex,
                            1);
                }
            }

            prevArcheTypeData.RemoveEntity(entity, allEntityDatas);
            allEntityDatas[entity.Id] = nextEntityData;
        }

        internal static void TransferAllEntities(
            ChangeVersion changeVersion,
            ArcheTypeData prevArcheTypeData,
            ArcheTypeData nextArcheTypeData,
            EntityData* allEntityDatas)
        {
            if (prevArcheTypeData.EntityCount == 0)
                return;

            var nextArcheTypeDataEntityCount = nextArcheTypeData.EntityCount;
            nextArcheTypeData.AddEntitiesNoChangeVersion(prevArcheTypeData._entities,
                0,
                prevArcheTypeData.EntityCount,
                allEntityDatas,
                false);

            for (var i = 0; i < prevArcheTypeData.GeneralConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.GeneralConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextArcheTypeData._generalBuffers[nextConfigOffset.ConfigIndex]
                        .CopyFromDifferent(prevArcheTypeData._generalBuffers[prevConfigOffset.ConfigIndex],
                            0,
                            nextArcheTypeDataEntityCount,
                            prevArcheTypeData.EntityCount);
                }
            }
            for (var i = 0; i < prevArcheTypeData.ManagedConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.ManagedConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextArcheTypeData._managedPools[nextConfigOffset.ConfigIndex]
                        .CopyFromDifferent(prevArcheTypeData._managedPools[prevConfigOffset.ConfigIndex],
                            0,
                            nextArcheTypeDataEntityCount,
                            prevArcheTypeData.EntityCount);
                }
            }

            prevArcheTypeData.RemoveAllEntities();
        }

        #endregion

        #region GetEntity

        internal Entity GetEntity(EntityData entityData)
            => _entities[(entityData.ChunkIndex * ArcheTypeDataChunk.ChunkMaxCapacity) + entityData.EntityIndex];

        /// <summary>
        /// Doesnt resize ref entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="startingIndex"></param>
        /// <exception cref="Exception"></exception>
        internal int GetAllEntities(ref Entity[] entities, int startingIndex)
        {
            if (EntityCount > 0)
            {
                fixed (Entity* entitiesPtr = &entities[startingIndex])
                {
                    MemoryHelper.Copy(
                        _entities,
                        entitiesPtr,
                        EntityCount);
                }
            }

            return EntityCount;
        }

        #endregion

        #region AddEntity

        internal EntityData AddEntity(ChangeVersion changeVersion, Entity entity, bool clearComponents)
        {
            CheckCapacity(1);

            EntityCount++;
            if (ChunksCount == 0)
                ChunksCount++;

            var lastChunk = LastChunk;
            var entityData = lastChunk.AddEntity(changeVersion, entity, clearComponents);
            if (lastChunk.IsFull)
                ChunksCount++;

            return entityData;
        }

        internal void AddEntities(ChangeVersion changeVersion, in Entity[] entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            CheckCapacity(count);

            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    entitiesPtr,
                    _entities + EntityCount,
                    count);

                EntityCount += count;
                if (ChunksCount == 0)
                    ChunksCount++;

                var availableCount = count;
                while (availableCount > 0)
                {
                    var chunk = LastChunk;
                    availableCount -= chunk.AddEntities(changeVersion, entitiesPtr, count - availableCount, availableCount,
                        allEntityDatas, clearComponents);
                    if (chunk.IsFull)
                        ChunksCount++;
                }
            }
        }

        #endregion

        #region RemoveEntity

        internal void RemoveEntity(Entity entity, EntityData* allEntityDatas)
        {
            var entityData = allEntityDatas[entity.Id];
            var chunk = _chunks[entityData.ChunkIndex];
            var lastChunk = LastChunk;

            if (chunk.ChunkIndex != lastChunk.ChunkIndex)
            {
                // Move last entity to removed entity index
                chunk.RemoveEntityAndCopyFromDifferentChunk(
                    entity,
                    _entities[EntityCount - 1],
                    lastChunk,
                    allEntityDatas);
            }
            else
                chunk.RemoveEntityFromChunk(entity, allEntityDatas);

            if (lastChunk.IsEmpty)
                ChunksCount--;
            EntityCount--;
        }

        internal void RemoveAllEntities()
        {
            // Clear managed so gc can clean up too
            for (var i = 0; i < ManagedConfigs.Length; i++)
                ManagedPools[i].ClearComponents(0, 0, EntityCount);

            for (var i = 0; i < ChunksCount; i++)
                _chunks[i].RemoveAllEntities();

            ChunksCount = 0;
            EntityCount = 0;
        }

        #endregion

        #region GetComponents

        internal int GetAllEntityComponents(EntityData entityData, ref IComponent[] components, int startingIndex)
        {
            var componentIndex = startingIndex;

            for (var i = 0; i < GeneralConfigs.Length; i++)
            {
                components[componentIndex++] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)_generalBuffers[i].PtrComponent(entityData.ChunkIndex, entityData.EntityIndex),
                    GeneralConfigs[i].Config.ComponentType);
            }
            for (var i = 0; i < ManagedConfigs.Length; i++)
                components[componentIndex++] = _managedPools[i].GetComponent(entityData.ChunkIndex, entityData.EntityIndex);
            for (var i = 0; i < SharedConfigs.Length; i++)
                components[componentIndex++] = SharedComponentDatas[i].Component;

            return componentIndex - startingIndex;
        }

        internal byte* GetComponentPtr(EntityData entityData, ComponentConfig config)
            => GetComponentPtr(entityData, GetConfigOffset(config));

        internal byte* GetComponentPtr(EntityData entityData, ComponentConfigOffset configOffset)
            => _generalBuffers[configOffset.ConfigIndex].PtrComponent(entityData.ChunkIndex, entityData.EntityIndex);

        internal IComponentPool GetManagedComponentPool(ComponentConfig config)
            => GetManagedComponentPool(GetConfigOffset(config));

        internal IComponentPool GetManagedComponentPool(ComponentConfigOffset configOffset)
            => _managedPools[configOffset.ConfigIndex];

        internal IComponent GetSharedComponentData(ComponentConfig config)
            => GetSharedComponentData(GetConfigOffset(config));

        internal IComponent GetSharedComponentData(ComponentConfigOffset configOffset)
            => SharedComponentDatas[configOffset.ConfigIndex].Component;

        #endregion

        #region GetGeneral

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponent<TComponent>(entityData, GetConfigOffset(config));

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => *(TComponent*)_generalBuffers[configOffset.ConfigIndex].PtrComponent(entityData.ChunkIndex, entityData.EntityIndex);

        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetAllComponents(ref components, startingIndex, GetConfigOffset(config));

        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
        {
            if (EntityCount > 0)
            {
                fixed (TComponent* componentPtr = &components[startingIndex])
                {
                    MemoryHelper.Copy(
                        (TComponent*)_generalBuffers[configOffset.ConfigIndex].Buffer,
                        componentPtr,
                        EntityCount);
                }
            }

            return EntityCount;
        }

        #endregion

        #region GetManaged

        internal TComponent GetManagedComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetManagedComponent<TComponent>(entityData, GetConfigOffset(config));

        internal TComponent GetManagedComponent<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex]).GetComponent(entityData.ChunkIndex, entityData.EntityIndex);

        internal ref TComponent GetManagedComponentRef<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IManagedComponent
            => ref GetManagedComponentRef<TComponent>(entityData, GetConfigOffset(config));

        internal ref TComponent GetManagedComponentRef<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ref ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex]).GetComponentRef(entityData);

        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetAllManagedComponents(ref components, startingIndex, GetConfigOffset(config));

        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
        {
            if (EntityCount > 0)
            {
                ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex])
                    .GetAllComponents(ref components, startingIndex, EntityCount);
            }

            return EntityCount;
        }

        #endregion

        #region GetShared

        internal TComponent GetSharedComponent<TComponent>(ComponentConfig config)
            where TComponent : unmanaged, ISharedComponent
            => GetSharedComponent<TComponent>(GetConfigOffset(config));

        internal TComponent GetSharedComponent<TComponent>(ComponentConfigOffset configOffset)
            where TComponent : unmanaged, ISharedComponent
            => ((SharedComponentData<TComponent>)SharedComponentDatas[configOffset.ConfigIndex]).Component;

        #endregion

        #region SetGeneral

        internal void SetComponent<TComponent>(ChangeVersion changeVersion, EntityData entityData, ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetComponentOffset(changeVersion, entityData, GetConfigOffset(config), component);

        internal void SetComponentOffset<TComponent>(ChangeVersion changeVersion, EntityData entityData, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => _chunks[entityData.ChunkIndex].SetComponent(changeVersion, entityData.EntityIndex, configOffset, component);

        internal void SetComponentAdapter(ChangeVersion changeVersion, EntityData entityData, ComponentConfig config, IComponent component)
            => _chunks[entityData.ChunkIndex].SetComponentAdapter(changeVersion, entityData.EntityIndex, GetConfigOffset(config), component);

        internal void SetComponentAdapter2(ChangeVersion changeVersion, EntityData entityData, ComponentConfig config)
            => _chunks[entityData.ChunkIndex].SetComponentAdapter2(changeVersion, entityData.EntityIndex, GetConfigOffset(config));

        internal void SetChunkComponents<TComponent>(ChangeVersion changeVersion, int chunkIndex, ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetChunkComponents(changeVersion, chunkIndex, GetConfigOffset(config), component);

        internal void SetChunkComponents<TComponent>(ChangeVersion changeVersion, int chunkIndex, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => _chunks[chunkIndex].SetComponents(changeVersion, configOffset, component);

        internal void SetAllComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetAllComponents(changeVersion, srcChunkEntityIndex, entityCount, GetConfigOffset(config), component);

        internal void SetAllComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            var buffer = (TComponent*)_generalBuffers[configOffset.ConfigIndex].Buffer;
            for (var i = 0; i < entityCount; i++)
                buffer[srcChunkEntityIndex + i] = component;

            var startChunkIndex = srcChunkEntityIndex / ArcheTypeDataChunk.ChunkMaxCapacity;
            var endChunkIndex = (srcChunkEntityIndex + entityCount) / ArcheTypeDataChunk.ChunkMaxCapacity;
            for (; startChunkIndex < endChunkIndex; startChunkIndex++)
                _chunks[startChunkIndex].GeneralVersions[configOffset.ConfigIndex] = changeVersion;
        }

        #endregion

        #region SetManaged

        internal void SetManagedComponent<TComponent>(ChangeVersion changeVersion, EntityData entityData, ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetManagedComponentOffset(changeVersion, entityData, GetConfigOffset(config), component);

        internal void SetManagedComponentOffset<TComponent>(ChangeVersion changeVersion, EntityData entityData, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
            => _chunks[entityData.ChunkIndex].SetManagedComponent(changeVersion, entityData.EntityIndex, configOffset, component);

        internal void SetManagedComponentAdapter(ChangeVersion changeVersion, EntityData entityData, ComponentConfig config, in IComponent component)
            => _chunks[entityData.ChunkIndex].SetManagedComponentAdapter(changeVersion, entityData.EntityIndex, GetConfigOffset(config), component);

        internal void SetManagedComponentAdapter2(ChangeVersion changeVersion, EntityData entityData, ComponentConfig config)
            => _chunks[entityData.ChunkIndex].SetManagedComponentAdapter2(changeVersion, entityData.EntityIndex, GetConfigOffset(config));

        internal void SetChunkManagedComponents<TComponent>(ChangeVersion changeVersion, int chunkIndex, ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetChunkManagedComponents(changeVersion, chunkIndex, GetConfigOffset(config), component);

        internal void SetChunkManagedComponents<TComponent>(ChangeVersion changeVersion, int chunkIndex, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
            => _chunks[chunkIndex].SetManagedComponents(changeVersion, configOffset, component);

        internal void SetAllManagedComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetAllManagedComponents(changeVersion, srcChunkEntityIndex, entityCount, GetConfigOffset(config), component);

        internal void SetAllManagedComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
        {
            _managedPools[configOffset.ConfigIndex].SetComponents(srcChunkEntityIndex, entityCount, component);

            var startChunkIndex = srcChunkEntityIndex / ArcheTypeDataChunk.ChunkMaxCapacity;
            var endChunkIndex = (srcChunkEntityIndex + entityCount) / ArcheTypeDataChunk.ChunkMaxCapacity;
            for (; startChunkIndex < endChunkIndex; startChunkIndex++)
                _chunks[startChunkIndex].ManagedVersions[configOffset.ConfigIndex] = changeVersion;
        }

        #endregion

        #region SetShared

        internal void SetSharedComponent(ChangeVersion changeVersion, ComponentConfig config)
            => SetSharedComponent(changeVersion, GetConfigOffset(config));

        internal void SetSharedComponent(ChangeVersion changeVersion, ComponentConfigOffset configOffset)
            => SharedVersions[configOffset.ConfigIndex] = changeVersion;

        #endregion

        #region CopyEntity

        internal void CopyComponentsFrom(ChangeVersion changeVersion, EntityData srcEntityData, EntityData destEntityData)
            => _chunks[destEntityData.ChunkIndex].CopyComponentsFrom(changeVersion, srcEntityData, destEntityData);

        internal void CopyComponentsFromSameArcheTypeData(ChangeVersion changeVersion,
            int srcChunkEntityIndex, int destChunkEntityIndex, int entityCount)
        {
            for (var i = 0; i < GeneralConfigs.Length; i++)
            {
                _generalBuffers[i].CopyFromSame(
                    srcChunkEntityIndex,
                    destChunkEntityIndex,
                    entityCount);
            }
            for (var i = 0; i < ManagedConfigs.Length; i++)
            {
                _managedPools[i].CopyFromSame(
                    srcChunkEntityIndex,
                    destChunkEntityIndex,
                    entityCount);
            }

            var destStartChunkIndex = destChunkEntityIndex / ArcheTypeDataChunk.ChunkMaxCapacity;
            var destEndChunkIndex = (destChunkEntityIndex + entityCount) / ArcheTypeDataChunk.ChunkMaxCapacity;
            for (; destStartChunkIndex <= destEndChunkIndex; destStartChunkIndex++)
                _chunks[destStartChunkIndex].UpdateComponentVersions(changeVersion);
        }

        internal void CopyComponentsFromDifferentArcheTypeDataSameComponents(ChangeVersion changeVersion, ArcheTypeData srcArcheTypeData,
            EntityData srcEntityData, EntityData destEntityData)
        {
            for (var i = 0; i < GeneralConfigs.Length; i++)
            {
                _generalBuffers[i].CopyFromDifferent(srcArcheTypeData._generalBuffers[i],
                    srcEntityData.ChunkIndex,
                    srcEntityData.EntityIndex,
                    destEntityData.ChunkIndex,
                    destEntityData.EntityIndex,
                    1);
            }
            for (var i = 0; i < ManagedConfigs.Length; i++)
            {
                _managedPools[i].CopyFromDifferent(srcArcheTypeData._managedPools[i],
                    srcEntityData.ChunkIndex,
                    srcEntityData.EntityIndex,
                    destEntityData.ChunkIndex,
                    destEntityData.EntityIndex,
                    1);
            }

            _chunks[srcEntityData.ChunkIndex].UpdateComponentVersions(changeVersion);
        }

        internal void CopyComponentsFromDifferentArcheTypeDataSameComponents(ChangeVersion changeVersion, ArcheTypeData srcArcheTypeData,
            int srcChunkEntityIndex, int destChunkEntityIndex, int entityCount)
        {
            for (var i = 0; i < GeneralConfigs.Length; i++)
            {
                _generalBuffers[i].CopyFromDifferent(srcArcheTypeData._generalBuffers[i],
                    srcChunkEntityIndex,
                    destChunkEntityIndex,
                    entityCount);
            }
            for (var i = 0; i < ManagedConfigs.Length; i++)
            {
                _managedPools[i].CopyFromDifferent(srcArcheTypeData._managedPools[i],
                    srcChunkEntityIndex,
                    destChunkEntityIndex,
                    entityCount);
            }

            var destStartChunkIndex = destChunkEntityIndex / ArcheTypeDataChunk.ChunkMaxCapacity;
            var destEndChunkIndex = (destChunkEntityIndex + entityCount) / ArcheTypeDataChunk.ChunkMaxCapacity;
            for (; destStartChunkIndex < destEndChunkIndex; destStartChunkIndex++)
                _chunks[destStartChunkIndex].UpdateComponentVersions(changeVersion);
        }

        #endregion

        internal void UpdateSharedComponentVersions(ChangeVersion changeVersion)
        {
            for (var i = 0; i < SharedVersions.Length; i++)
                SharedVersions[i] = changeVersion;
        }

        internal ComponentConfigOffset GetConfigOffset(ComponentConfig config)
        {
#if DEBUG
            var checkConfig = _configOffsets[config.ComponentIndex];
            if (checkConfig.Config != config)
                throw new Exception();
#endif
            return _configOffsets[config.ComponentIndex];
        }

        internal bool HasConfigOffset(ComponentConfig config, out ComponentConfigOffset configOffset)
        {
            configOffset = _configOffsets[config.ComponentIndex];
            if (configOffset.Config != config)
                return false;

            return true;
        }

        internal bool HasConfig(ComponentConfig config)
            => _configOffsets[config.ComponentIndex].Config == config;

        internal void InternalDestroy()
        {
            MemoryHelper.Free(_entities);
            _entitiesLength = 0;
            _managedPools = null;
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            if (GeneralConfigs.Length > 0)
                MemoryHelper.Free(_generalBuffers);
            _generalBuffers = null;
            SharedComponentDatas = null;
            ArcheTypeIndex = new ArcheTypeIndex();
            ArcheType.Dispose();
            ArcheType = new ArcheType();
            EntityCount = 0;
            GeneralConfigs = null;
            ManagedConfigs = null;
            SharedConfigs = null;
        }

        private void CheckCapacity(int count)
        {
            if (count > (_entitiesLength - EntityCount))
            {
                var newLength = Helper.NextPow2(_entitiesLength + count);
                _entities = MemoryHelper.ReallocCopy(_entities, _entitiesLength, newLength);
                for (var i = 0; i < GeneralConfigs.Length; i++)
                {
                    var genBuffer = _generalBuffers[i];
                    _generalBuffers[i] = new ComponentBuffer
                    {
                        Buffer = MemoryHelper.ReallocCopy(genBuffer.Buffer,
                            genBuffer.ComponentSize * _entitiesLength,
                            genBuffer.ComponentSize * newLength),
                        ComponentSize = genBuffer.ComponentSize
                    };
                }
                for (var i = 0; i < _managedPools.Length; i++)
                    _managedPools[i].Resize(newLength);

                if (newLength > _chunks.Length * ArcheTypeDataChunk.ChunkMaxCapacity)
                {
                    var newChunkLength = (newLength / ArcheTypeDataChunk.ChunkMaxCapacity) +
                        (newLength % ArcheTypeDataChunk.ChunkMaxCapacity != 0 ? 1 : 0);

                    var oldChunkLength = _chunks.Length;
                    if (newChunkLength > _chunks.Length)
                        Array.Resize(ref _chunks, newChunkLength);

                    for (; oldChunkLength < newChunkLength; oldChunkLength++)
                        _chunks[oldChunkLength] = new ArcheTypeDataChunk(this, oldChunkLength);
                }

                _entitiesLength = newLength;
            }
        }

        private EntityData AddEntityNoChangeVersion(Entity entity, bool clearComponents)
        {
            CheckCapacity(1);

            EntityCount++;
            if (ChunksCount == 0)
                ChunksCount++;

            var lastChunk = LastChunk;
            var entityData = lastChunk.AddEntityNoChangeVersion(entity, clearComponents);
            if (lastChunk.IsFull)
                ChunksCount++;

            return entityData;
        }

        private void AddEntitiesNoChangeVersion(Entity* entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            CheckCapacity(count);

            MemoryHelper.Copy(
                entities,
                _entities + EntityCount,
                count);

            EntityCount += count;
            if (ChunksCount == 0)
                ChunksCount++;

            var availableCount = count;
            while (availableCount > 0)
            {
                var chunk = LastChunk;
                availableCount -= chunk.AddEntitiesNoChangeVersion(entities, count - availableCount, availableCount,
                    allEntityDatas, clearComponents);
                if (chunk.IsFull)
                    ChunksCount++;
            }
        }
    }
}
