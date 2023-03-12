using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal unsafe class ArcheTypeData
    {
        private DataManager _dataManager;
        private DataChunk[] _chunks;
        private ComponentConfigOffset* _configOffsets;
        private int _lastChunkIndex;

        internal ArcheType ArcheType { get; private set; }
        internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
        internal DataChunk[] Chunks { get => _chunks; }
        internal int ChunksCount { get; private set; }
        internal DataChunk LastChunk { get => _chunks[_lastChunkIndex]; }
        internal ChangeVersion[] SharedVersions { get; private set; }
        internal ComponentConfigOffset[] GeneralConfigs { get; private set; }
        internal ComponentConfigOffset[] ManagedConfigs { get; private set; }
        internal ComponentConfigOffset[] SharedConfigs { get; private set; }
        internal ISharedComponentData[] SharedComponentDatas { get; private set; }

        internal ArcheTypeData(ArcheType archeType, ArcheTypeIndex archeTypeIndex, SharedComponentDictionaries sharedDics, DataManager dataManager)
        {
            _dataManager = dataManager;

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
            ManagedConfigs = managedConfigs.ToArray();
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

            _chunks = new DataChunk[0];
            ChunksCount = 0;
            _lastChunkIndex = 0;
        }

        #region GetChunks

        /// <summary>
        /// Doesnt resize ref chunks
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="startingIndex"></param>
        /// <exception cref="Exception"></exception>
        internal int GetAllChunks(ref DataChunk[] chunks, int startingIndex)
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

            var prevChunk = prevArcheTypeData._chunks[prevEntityData.ChunkIndex];
            var nextChunk = nextArcheTypeData._chunks[nextEntityData.ChunkIndex];

            for (var i = 0; i < prevArcheTypeData.GeneralConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.GeneralConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextChunk.GeneralComponentPages[nextConfigOffset.ConfigIndex]
                        .Copy(
                            prevChunk.GeneralComponentPages[prevConfigOffset.ConfigIndex],
                            prevEntityData.EntityIndex,
                            nextEntityData.EntityIndex);
                }
            }
            for (var i = 0; i < prevArcheTypeData.ManagedConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.ManagedConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextChunk.ManagedComponentPages[nextConfigOffset.ConfigIndex]
                        .Copy(
                            prevChunk.ManagedComponentPages[prevConfigOffset.ConfigIndex],
                            prevEntityData.EntityIndex,
                            nextEntityData.EntityIndex);
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
            var prevEntityCount = prevArcheTypeData.EntityCount();
            if (prevEntityCount == 0)
                return;

            nextArcheTypeData.CheckCapacity(prevEntityCount);

            var preChunk = prevArcheTypeData._chunks[0];
            var preChunkEntityCount = preChunk.EntityCount;
            var nextChunk = nextArcheTypeData._chunks[nextArcheTypeData._lastChunkIndex];
            while (prevEntityCount > 0)
            {
                if (nextChunk.IsFull)
                    nextChunk = nextArcheTypeData._chunks[nextChunk.ChunkIndex + 1];
                if (preChunk.IsEmpty)
                {
                    // Set this back so clear can run corrrect entity count
                    preChunk.EntityCount = preChunkEntityCount;
                    preChunk = prevArcheTypeData._chunks[preChunk.ChunkIndex + 1];
                }

                var transferCount = Math.Min(nextChunk.EntityAvailableCount, preChunk.EntityCount);
                var transferIndex = preChunk.EntityCount - transferCount;

                nextChunk.EntityPage.Copy(
                    preChunk.EntityPage,
                    transferIndex,
                    nextChunk.EntityCount,
                    transferCount);
                for (var j = 0; j < transferCount; j++)
                {
                    allEntityDatas[preChunk.EntityPage.Get(transferIndex + j).Id] = new EntityData
                    {
                        ArcheTypeIndex = nextArcheTypeData.ArcheTypeIndex,
                        ChunkIndex = nextChunk.ChunkIndex,
                        EntityIndex = nextChunk.EntityCount + j
                    };
                }
                for (var j = 0; j < prevArcheTypeData.GeneralConfigs.Length; j++)
                {
                    var prevConfigOffset = prevArcheTypeData.GeneralConfigs[j];
                    if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                    {
                        nextChunk.GeneralComponentPages[nextConfigOffset.ConfigIndex].Copy(
                            preChunk.GeneralComponentPages[prevConfigOffset.ConfigIndex],
                            transferIndex,
                            nextChunk.EntityCount,
                            transferCount);
                    }
                }
                for (var j = 0; j < prevArcheTypeData.ManagedConfigs.Length; j++)
                {
                    var prevConfigOffset = prevArcheTypeData.ManagedConfigs[j];
                    if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                    {
                        nextChunk.ManagedComponentPages[nextConfigOffset.ConfigIndex].Copy(
                            preChunk.ManagedComponentPages[prevConfigOffset.ConfigIndex],
                            transferIndex,
                            nextChunk.EntityCount,
                            transferCount);
                    }
                }

                nextChunk.EntityCount += transferCount;
                preChunk.EntityCount -= transferCount;
                prevEntityCount -= transferCount;
            }

            prevArcheTypeData.RemoveAllEntities();
            nextArcheTypeData._lastChunkIndex = nextArcheTypeData.ChunksCount - 1;
        }

        #endregion

        #region GetEntity

        internal int EntityCount()
        {
            if (ChunksCount == 0)
                return 0;

            var chunkIndex = ChunksCount - 1;
            var count = chunkIndex * DataManager.PageCapacity;
            count += _chunks[chunkIndex].EntityCount;

            return count;
        }

        internal int EntityCapacity()
            => _chunks.Length * DataManager.PageCapacity;

        internal Entity GetEntity(EntityData entityData)
        {
            var chunk = _chunks[entityData.ChunkIndex];
            return chunk.EntityPage.Get(entityData.EntityIndex);
        }

        /// <summary>
        /// Doesnt resize ref entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="startingIndex"></param>
        /// <exception cref="Exception"></exception>
        internal int GetAllEntities(ref Entity[] entities, int startingIndex)
        {
            var entityIndex = startingIndex;
            for (var i = 0; i < ChunksCount; i++)
                entityIndex += _chunks[i].GetEntities(ref entities, entityIndex);

            return entityIndex - startingIndex;
        }

        #endregion

        #region AddEntity

        internal EntityData AddEntity(ChangeVersion changeVersion, Entity entity, bool clearComponents)
        {
            CheckCapacity(1);

            var lastChunk = LastChunk;
            if (lastChunk.IsFull)
            {
                _lastChunkIndex++;
                lastChunk = LastChunk;
            }

            return lastChunk.AddEntity(changeVersion, entity, clearComponents);
        }

        internal void AddEntities(ChangeVersion changeVersion, in Entity[] entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            CheckCapacity(count);

            while (count > 0)
            {
                var lastChunk = LastChunk;
                if (lastChunk.IsFull)
                {
                    _lastChunkIndex++;
                    lastChunk = LastChunk;
                }
                var addedCount = lastChunk.AddAvailableEntities(changeVersion, entities, startingIndex, count,
                    allEntityDatas, clearComponents);
                count -= addedCount;
                startingIndex += addedCount;
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
                // Move last entity to removed entity index
                chunk.RemoveEntityAndFill(entity, lastChunk, allEntityDatas);
            else
                chunk.RemoveEntity(entity, allEntityDatas);

            if (lastChunk.IsEmpty)
            {
                if (_lastChunkIndex != 0)
                    _lastChunkIndex--;
                ChunksCount--;
                _dataManager.EnqueuePages(GeneralConfigs, ManagedConfigs, ref _chunks, ChunksCount, 1);
            }
        }

        internal void RemoveAllEntities()
        {
            for (var i = 0; i < ChunksCount; i++)
                _chunks[i].RemoveAllEntities();

            _dataManager.EnqueuePages(GeneralConfigs, ManagedConfigs, ref _chunks, 0, ChunksCount);

            _lastChunkIndex = 0;
            ChunksCount = 0;
        }

        #endregion

        #region GetComponents

        internal int GetAllEntityComponents(EntityData entityData, ref IComponent[] components, int startingIndex)
        {
            var componentIndex = startingIndex + _chunks[entityData.ChunkIndex].GetAllEntityComponents(entityData.EntityIndex, ref components, startingIndex);

            for (var i = 0; i < SharedConfigs.Length; i++)
                components[componentIndex++] = SharedComponentDatas[i].Component;

            return componentIndex - startingIndex;
        }

        internal IDataPage GetComponentPage(EntityData entityData, ComponentConfig config)
            => GetComponentPage(entityData, GetConfigOffset(config));

        internal IDataPage GetComponentPage(EntityData entityData, ComponentConfigOffset configOffset)
            => _chunks[entityData.ChunkIndex].GetComponentPage(configOffset);

        internal IDataPage GetManagedComponentPage(EntityData entityData, ComponentConfig config)
            => GetManagedComponentPage(entityData, GetConfigOffset(config));

        internal IDataPage GetManagedComponentPage(EntityData entityData, ComponentConfigOffset configOffset)
            => _chunks[entityData.ChunkIndex].GetManagedComponentPage(configOffset);

        internal IComponent GetSharedComponentData(ComponentConfig config)
            => GetSharedComponentData(GetConfigOffset(config));

        internal IComponent GetSharedComponentData(ComponentConfigOffset configOffset)
            => SharedComponentDatas[configOffset.ConfigIndex].Component;

        #endregion

        #region GetGeneral

        internal IComponent GetComponentObj(EntityData entityData, ComponentConfig config)
            => _chunks[entityData.ChunkIndex].GetComponentObj(entityData.EntityIndex, GetConfigOffset(config));

        internal ref TComponent GetComponentRef<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => ref _chunks[entityData.ChunkIndex].GetComponentRef<TComponent>(entityData.EntityIndex, configOffset);

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponent<TComponent>(entityData, GetConfigOffset(config));

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => _chunks[entityData.ChunkIndex].GetComponent<TComponent>(entityData.EntityIndex, configOffset);

        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetAllComponents(ref components, startingIndex, GetConfigOffset(config));

        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
        {
            var componentIndex = startingIndex;
            for (var i = 0; i < ChunksCount; i++)
                componentIndex += _chunks[i].GetAllComponents(ref components, componentIndex, configOffset);

            return componentIndex - startingIndex;
        }

        #endregion

        #region GetManaged

        internal IComponent GetManagedComponentObj(EntityData entityData, ComponentConfig config)
            => _chunks[entityData.ChunkIndex].GetManagedComponentObj(entityData.EntityIndex, GetConfigOffset(config));

        internal ref TComponent GetManagedComponentRef<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ref _chunks[entityData.ChunkIndex].GetManagedComponentRef<TComponent>(entityData.EntityIndex, configOffset);

        internal TComponent GetManagedComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetManagedComponent<TComponent>(entityData, GetConfigOffset(config));

        internal TComponent GetManagedComponent<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => _chunks[entityData.ChunkIndex].GetManagedComponent<TComponent>(entityData.EntityIndex, configOffset);

        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetAllManagedComponents(ref components, startingIndex, GetConfigOffset(config));

        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
        {
            var componentIndex = startingIndex;
            for (var i = 0; i < ChunksCount; i++)
                componentIndex += _chunks[i].GetAllManagedComponents(ref components, componentIndex, configOffset);

            return componentIndex - startingIndex;
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

        internal void SetAllComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetAllComponents(changeVersion, srcChunkEntityIndex, entityCount, GetConfigOffset(config), component);

        internal void SetAllComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            var currentChunkIndex = srcChunkEntityIndex / DataManager.PageCapacity;
            var currentEntityIndex = srcChunkEntityIndex - (currentChunkIndex * DataManager.PageCapacity);

            // -1 first set would do at least one
            // Ex when entityCount = PageCapacity, endChunkIndex is +1 when it is wrong
            var endChunkIndex = (srcChunkEntityIndex + entityCount - 1) / DataManager.PageCapacity;
            var endEntityIndex = (srcChunkEntityIndex + entityCount) - (endChunkIndex * DataManager.PageCapacity);

            // set first chunk
            var chunk = _chunks[currentChunkIndex];
            chunk.SetComponents(changeVersion, currentEntityIndex, chunk.EntityCount - currentEntityIndex,
                configOffset, component);

            if (currentChunkIndex != endChunkIndex)
            {
                // set inbetween, set full chunks
                currentChunkIndex++;
                while (currentChunkIndex < endChunkIndex)
                {
                    chunk = _chunks[currentChunkIndex];
                    chunk.SetComponents(changeVersion, 0, chunk.EntityCount,
                        configOffset, component);
                    currentChunkIndex++;
                }

                // set last chunk
                chunk = _chunks[endChunkIndex];
                chunk.SetComponents(changeVersion, 0, endEntityIndex,
                    configOffset, component);
            }
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

        internal void SetAllManagedComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetAllManagedComponents(changeVersion, srcChunkEntityIndex, entityCount, GetConfigOffset(config), component);

        internal void SetAllManagedComponents<TComponent>(ChangeVersion changeVersion, int srcChunkEntityIndex, int entityCount,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
        {
            var currentChunkIndex = srcChunkEntityIndex / DataManager.PageCapacity;
            var currentEntityIndex = srcChunkEntityIndex - (currentChunkIndex * DataManager.PageCapacity);

            // -1 first set would do at least one
            // Ex when entityCount = PageCapacity, endChunkIndex is +1 when it is wrong
            var endChunkIndex = (srcChunkEntityIndex + entityCount - 1) / DataManager.PageCapacity;
            var endEntityIndex = (srcChunkEntityIndex + entityCount) - (endChunkIndex * DataManager.PageCapacity);

            // set first chunk
            var chunk = _chunks[currentChunkIndex];
            chunk.SetManagedComponents(changeVersion, currentEntityIndex, chunk.EntityCount - currentEntityIndex,
                configOffset, component);

            if (currentChunkIndex != endChunkIndex)
            {
                // set inbetween, set full chunks
                currentChunkIndex++;
                while (currentChunkIndex < endChunkIndex)
                {
                    chunk = _chunks[currentChunkIndex];
                    chunk.SetManagedComponents(changeVersion, 0, chunk.EntityCount,
                        configOffset, component);
                    currentChunkIndex++;
                }

                // set last chunk
                chunk = _chunks[endChunkIndex];
                chunk.SetManagedComponents(changeVersion, 0, endEntityIndex,
                    configOffset, component);
            }
        }

        #endregion

        #region SetShared

        internal void SetSharedComponent(ChangeVersion changeVersion, ComponentConfig config)
            => SetSharedComponent(changeVersion, GetConfigOffset(config));

        internal void SetSharedComponent(ChangeVersion changeVersion, ComponentConfigOffset configOffset)
            => SharedVersions[configOffset.ConfigIndex] = changeVersion;

        #endregion

        #region CopyEntity

        internal void CopyComponentsFromSameArcheTypeData(ChangeVersion changeVersion, EntityData srcEntityData, EntityData destEntityData)
            => _chunks[destEntityData.ChunkIndex].CopyComponentsFromSameArcheTypeData(changeVersion, srcEntityData.EntityIndex, destEntityData.EntityIndex);

        internal void CopyComponentsFromSameArcheTypeData(ChangeVersion changeVersion,
            int srcChunkEntityIndex, int destChunkEntityIndex, int entityCount)
        {
            var copiedCount = 0;
            while (entityCount > 0)
            {
                var srcChunkIndex = (srcChunkEntityIndex + copiedCount) / DataManager.PageCapacity;
                var srcEntityIndex = (srcChunkEntityIndex + copiedCount) - (srcChunkIndex * DataManager.PageCapacity);
                var srcChunk = _chunks[srcChunkIndex];

                var destChunkIndex = (destChunkEntityIndex + copiedCount) / DataManager.PageCapacity;
                var destEntityIndex = (destChunkEntityIndex + copiedCount) - (destChunkIndex * DataManager.PageCapacity);
                var destChunk = _chunks[destChunkIndex];

                var copyCount = Math.Min(destChunk.EntityCount, entityCount);
                destChunk.CopyComponentsFromDifferentDataChunkSameComponents(changeVersion, srcChunk,
                    srcEntityIndex, destEntityIndex, copyCount);
                entityCount -= copyCount;
                copiedCount += copyCount;
            }
        }

        internal void CopyComponentsFromDifferentArcheTypeDataSameComponents(ChangeVersion changeVersion, ArcheTypeData srcArcheTypeData,
            EntityData srcEntityData, EntityData destEntityData)
        {
            var srcChunk = srcArcheTypeData._chunks[srcEntityData.ChunkIndex];

            _chunks[destEntityData.ChunkIndex].CopyComponentsFromDifferentArcheTypeDataSameComponents(changeVersion, srcChunk,
                srcEntityData.EntityIndex, destEntityData.EntityIndex);
        }

        internal void CopyComponentsFromDifferentArcheTypeDataSameComponents(ChangeVersion changeVersion, ArcheTypeData srcArcheTypeData,
            int srcChunkEntityIndex, int destChunkEntityIndex, int entityCount)
        {
            var copiedCount = 0;
            while (entityCount > 0)
            {
                var srcChunkIndex = (srcChunkEntityIndex + copiedCount) / DataManager.PageCapacity;
                var srcEntityIndex = (srcChunkEntityIndex + copiedCount) - (srcChunkIndex * DataManager.PageCapacity);
                var srcChunk = srcArcheTypeData._chunks[srcChunkIndex];

                var destChunkIndex = (destChunkEntityIndex + copiedCount) / DataManager.PageCapacity;
                var destEntityIndex = (destChunkEntityIndex + copiedCount) - (destChunkIndex * DataManager.PageCapacity);
                var destChunk = _chunks[destChunkIndex];

                var copyCount = Math.Min(destChunk.EntityCount, entityCount);
                destChunk.CopyComponentsFromDifferentDataChunkSameComponents(changeVersion, srcChunk,
                    srcEntityIndex, destEntityIndex, copyCount);
                entityCount -= copyCount;
                copiedCount += copyCount;
            }
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
            _dataManager.DequeuePages(GeneralConfigs, ManagedConfigs, ref _chunks, 0, ChunksCount);
            _chunks = null;
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            _lastChunkIndex = 0;

            ArcheType.Dispose();
            ArcheType = new ArcheType();
            ArcheTypeIndex = new ArcheTypeIndex();
            ChunksCount = 0;
            SharedVersions = null;
            GeneralConfigs = null;
            ManagedConfigs = null;
            SharedConfigs = null;
            SharedComponentDatas = null;
        }

        private int CheckCapacityNoDataPageDequeue(int count)
        {
            var entityCount = EntityCount();
            var neededChunkLength = (entityCount + count) / DataManager.PageCapacity +
                ((entityCount + count) % DataManager.PageCapacity > 0 ? 1 : 0);

            var prevChunksLength = _chunks.Length;
            if (neededChunkLength > _chunks.Length)
                Array.Resize(ref _chunks, neededChunkLength);

            for (var i = prevChunksLength; i < neededChunkLength; i++)
            {
                _chunks[i] = new DataChunk
                {
                    ArcheTypeData = this,
                    ChunkIndex = i,
                    GeneralComponentPages = new IDataPage[GeneralConfigs.Length],
                    ManagedComponentPages = new IDataPage[ManagedConfigs.Length],
                    GeneralVersions = new ChangeVersion[GeneralConfigs.Length],
                    ManagedVersions = new ChangeVersion[ManagedConfigs.Length]
                };
            }

            return neededChunkLength;
        }

        private void CheckCapacity(int count)
        {
            var neededChunkLength = CheckCapacityNoDataPageDequeue(count);
            _dataManager.DequeuePages(GeneralConfigs, ManagedConfigs, ref _chunks, ChunksCount, neededChunkLength - ChunksCount);

            ChunksCount = neededChunkLength;
        }

        private EntityData AddEntityNoChangeVersion(Entity entity, bool clearComponents)
        {
            CheckCapacity(1);

            var lastChunk = LastChunk;
            if (lastChunk.IsFull)
            {
                _lastChunkIndex++;
                lastChunk = LastChunk;
            }

            return lastChunk.AddEntityNoChangeVersion(entity, clearComponents);
        }

        private void AddEntitiesNoChangeVersion(Entity[] entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            CheckCapacity(count);

            while (count > 0)
            {
                var lastChunk = LastChunk;
                if (lastChunk.IsFull)
                {
                    _lastChunkIndex++;
                    lastChunk = LastChunk;
                }

                var addedCount = lastChunk.AddAvailableEntitiesNoChangeVersion(entities, startingIndex, count,
                    allEntityDatas, clearComponents);
                count -= addedCount;
                startingIndex += addedCount;
            }
        }
    }
}
