using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        private int _entitiesCount;
        private int _entitiesLength;
        private int _nextId;
        private Entity[] _reusableEntities;
        private int _reusableEntitiesCount;
        private Entity[] _cachedInternalEntities;
        private EntityData* _entityDatas;
        private ArcheType _cachedArcheType;
        private ChangeVersion _globalVersion;

        public EcsContext Context { get; private set; }

        public ChangeVersion GlobalVersion { get => _globalVersion; }

        internal EntityManager(EcsContext context)
        {
            //_entitiesCount
            _entitiesLength = 1;
            _nextId = 1;
            _reusableEntities = new Entity[1];
            //_reusableEntitiesCount
            _cachedInternalEntities = new Entity[0];
            _entityDatas = MemoryHelper.Alloc<EntityData>(1);
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);

            Context = context;
            _globalVersion = new ChangeVersion { Version = 1 };
        }

        // todo might not need this code
        /*internal int GetArcheTypeDataChunks(EntityQuery query, ref ArcheTypeDataChunk[] chunks)
        {
            if (query.Filter != null && query.Tracker == null)
                return GetArcheTypeDataChunks(query.Filter, ref chunks);
            else if (query.Filter == null && query.Tracker != null)
                return GetArcheTypeDataChunks(query.Tracker, ref chunks);
            else if (query.Filter != null && query.Tracker != null)
            {
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                var chunkIndex = 0;
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    var archeTypeData = filteredArcheTypeDatas[i];
                    query.Tracker.GetArcheTypeDataChunks(archeTypeData, out var archeTypeChunks);
                    Helper.ResizeRefArray(ref chunks, chunkIndex, archeTypeChunks.Count);
                    archeTypeChunks.CopyTo(0, chunks, chunkIndex, archeTypeChunks.Count);
                }

                // TODO Not sure if +1 is needed
                return chunkIndex + 1;
            }

            return 0;
        }

        internal int GetArcheTypeDataChunks(EntityFilter filter, ref ArcheTypeDataChunk[] chunks)
        {
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var chunkIndex = 0;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.ChunksCount > 0)
                {
                    Helper.ResizeRefArray(ref chunks, chunkIndex, archeTypeData.ChunksCount);
                    chunkIndex += archeTypeData.GetAllChunks(ref chunks, chunkIndex);
                }
            }

            // TODO Not sure if +1 is needed
            return chunkIndex + 1;
        }

        internal int GetArcheTypeDataChunks(EntityTracker tracker, ref ArcheTypeDataChunk[] chunks)
        {
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(tracker.TrackingFilter());
            var chunkIndex = 0;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                tracker.GetArcheTypeDataChunks(archeTypeData, out var archeTypeChunks);

                Helper.ResizeRefArray(ref chunks, chunkIndex, archeTypeChunks.Count);
                archeTypeChunks.CopyTo(0, chunks, chunkIndex, archeTypeChunks.Count);

                chunkIndex += archeTypeChunks.Count;
            }

            // TODO Not sure if +1 is needed
            return chunkIndex + 1;
        }*/

        internal void InternalDestroy()
        {
            _entitiesCount = 0;
            _entitiesLength = 0;
            _nextId = 1;
            _reusableEntities = null;
            _reusableEntitiesCount = 0;
            _cachedInternalEntities = null;
            MemoryHelper.Free(_entityDatas);
            _entityDatas = null;
            _cachedArcheType.Dispose();
            _cachedArcheType = new ArcheType();
        }

        #region Private

        private bool InternalHasEntity(Entity entity, out EntityData entityData, out ArcheTypeData archeTypeData)
        {
            if (entity.Id > 0 && entity.Id < _entitiesLength)
            {
                entityData = _entityDatas[entity.Id];
                if (entityData.ArcheTypeIndex != ArcheTypeIndex.Null)
                {
                    archeTypeData = Context.ArcheTypes.GetArcheTypeData(entityData.ArcheTypeIndex);
                    return archeTypeData.GetEntity(entityData) == entity;
                }
                else
                    archeTypeData = null;
            }
            else
            {
                entityData = default;
                archeTypeData = null;
            }

            return false;
        }

        private void CheckAndAllocEntity(ArcheTypeData archeTypeData, bool clearComponents,
            out Entity entity, out EntityData entityData)
        {
            // Account for Entity.Null
            var unusedCount = _entitiesLength - ((_entitiesCount + 1) - _reusableEntitiesCount);
            if (unusedCount < 1)
            {
                var newLength = Helper.NextPow2(_entitiesLength + 1);
                Array.Resize(ref _reusableEntities, newLength);
                _entityDatas = MemoryHelper.ReallocCopy(_entityDatas, _entitiesLength, newLength);

                _entitiesLength = newLength;
            }

            if (_reusableEntitiesCount > 0)
            {
                entity = _reusableEntities[--_reusableEntitiesCount];
                entity.Version++;
            }
            else
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
            }
            _entitiesCount++;

            entityData = archeTypeData.AddEntity(GlobalVersion, entity, clearComponents);
            _entityDatas[entity.Id] = entityData;
        }

        private void CheckAndAllocEntities(ArcheTypeData archeTypeData, bool clearComponents,
            ref Entity[] entities, int startingIndex, int count)
        {
            // Account for Entity.Null
            var unusedCount = _entitiesLength - ((_entitiesCount + 1) - _reusableEntitiesCount);
            if (unusedCount < count)
            {
                var newLength = Helper.NextPow2(_entitiesLength + count);
                Array.Resize(ref _reusableEntities, newLength);
                _entityDatas = MemoryHelper.ReallocCopy(_entityDatas, _entitiesLength, newLength);

                _entitiesLength = newLength;
            }

            var entityIndex = 0;
            var destIndex = startingIndex;
            if (_reusableEntitiesCount > 0)
            {
                var reuseCount = Math.Min(_reusableEntitiesCount, count);
                for (var i = 0; i < reuseCount; i++, destIndex++)
                {
                    var entity = _reusableEntities[--_reusableEntitiesCount];
                    entity.Version++;
                    entities[destIndex] = entity;
                }
                entityIndex = reuseCount;
            }
            for (var i = entityIndex; i < count; i++, destIndex++)
            {
                var entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
                entities[destIndex] = entity;
            }
            _entitiesCount += count;

            archeTypeData.AddEntities(GlobalVersion, entities, startingIndex, count,
                _entityDatas, clearComponents);
        }

        private void DeallocEntity(Entity entity, ArcheTypeData archeTypeData)
        {
            archeTypeData.RemoveEntity(entity, _entityDatas);

            _reusableEntities[_reusableEntitiesCount++] = entity;
            _entityDatas[entity.Id] = EntityData.Null;
            _entitiesCount--;
        }

        private void DealloArcheTypeDataEntities(ArcheTypeData archeTypeData)
        {
            if (archeTypeData.EntityCount > 0)
            {
                var count = archeTypeData.EntityCount;
                Helper.ResizeRefArray(ref _cachedInternalEntities, 0, count);
                archeTypeData.GetAllEntities(ref _cachedInternalEntities, 0);
                for (var i = 0; i < count; i++)
                {
                    var entity = _cachedInternalEntities[i];
                    _entityDatas[entity.Id] = EntityData.Null;
                    _reusableEntities[_reusableEntitiesCount++] = entity;
                }
                _entitiesCount -= count;

                archeTypeData.RemoveAllEntities();
            }
        }

        private int InternalCopyToArcheTypeData(ArcheTypeData srcArcheTypeData,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (srcArcheTypeData.EntityCount > 0)
            {
                InternalCacheDiffContextArcheType(srcArcheTypeData);
                var copyArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                var prevEntityCount = copyArcheTypeData.EntityCount;

                Helper.ResizeRefArray(ref destEntities, destStartingIndex, srcArcheTypeData.EntityCount);
                CheckAndAllocEntities(copyArcheTypeData, false,
                    ref destEntities, destStartingIndex, srcArcheTypeData.EntityCount);

                copyArcheTypeData.CopyComponentsFromDifferentArcheTypeDataSameComponents(
                    GlobalVersion,
                    srcArcheTypeData,
                    0,
                    prevEntityCount,
                    srcArcheTypeData.EntityCount);
            }

            return srcArcheTypeData.EntityCount;
        }

        // TODO redo in chunk code?
        /*private int InternalCopyToTracker(EntityTracker tracker, ArcheTypeData srcArcheTypeData,
            Dictionary<ArcheTypeIndex, (ArcheTypeData, ArcheTypeData)> cachedArcheTypeDatas,
            ref Entity[] destEntities, int destStartingIndex)
        {
            var trackedEntityCount = tracker.GetArcheTypeDataEntities(srcArcheTypeData,
                ref _cachedInternalEntities, 0);
            if (trackedEntityCount > 0)
            {
                if (!cachedArcheTypeDatas.TryGetValue(srcArcheTypeData.ArcheTypeIndex, out var archeTypeDatas))
                {
                    archeTypeDatas.Item1 = srcArcheTypeData;
                    InternalCacheDiffContextArcheType(srcArcheTypeData);
                    archeTypeDatas.Item2 = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);

                    cachedArcheTypeDatas.Add(srcArcheTypeData.ArcheTypeIndex, archeTypeDatas);
                }

                var preEntityCount = archeTypeDatas.Item2.EntityCount;
                Helper.ResizeRefArray(ref destEntities, destStartingIndex, trackedEntityCount);
                CheckAndAllocEntities(archeTypeDatas.Item2, false,
                    ref destEntities, destStartingIndex, trackedEntityCount);

                for (var i = 0; i < trackedEntityCount; i++)
                {
                    archeTypeDatas.Item2.CopyComponentsDifferentArcheTypeDataSameComponents(
                        archeTypeDatas.Item1,
                        tracker.Context.Entities._entityDatas[_cachedInternalEntities[i].Id].EntityIndex,
                        preEntityCount + i,
                        1);
                }
            }

            return trackedEntityCount;
        }*/

        private int InternalDuplicateArcheTypeData(ArcheTypeData archeTypeData,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (archeTypeData.EntityCount == 0)
                return 0;

            var preEntityCount = archeTypeData.EntityCount;
            Helper.ResizeRefArray(ref destEntities, destStartingIndex, preEntityCount);
            CheckAndAllocEntities(archeTypeData, false,
                ref destEntities, destStartingIndex, archeTypeData.EntityCount);

            archeTypeData.CopyComponentsFromSameArcheTypeData(
                GlobalVersion,
                0,
                preEntityCount,
                preEntityCount);

            return archeTypeData.EntityCount - preEntityCount;
        }

        // TODO redo in chunk code?
        /*private int InternalDuplicateTracker(EntityTracker tracker, ArcheTypeData archeTypeData,
            ref Entity[] destEntities, int destStartingIndex)
        {
            var trackedEntityCount = tracker.GetArcheTypeDataEntities(archeTypeData,
                ref _cachedInternalEntities, 0);
            if (trackedEntityCount > 0)
            {
                var preEntityCount = archeTypeData.EntityCount;
                Helper.ResizeRefArray(ref destEntities, destStartingIndex, trackedEntityCount);
                CheckAndAllocEntities(archeTypeData, false,
                    ref destEntities, destStartingIndex, trackedEntityCount);

                for (var i = 0; i < trackedEntityCount; i++)
                {
                    // NOT TESTED
                    archeTypeData.CopyComponentsSameArcheTypeData(
                        GlobalVersion,
                        _entityDatas[_cachedInternalEntities[i].Id].EntityIndex,
                        preEntityCount + i,
                        1);
                }
            }

            return trackedEntityCount;
        }*/

        private void InternalCacheDiffContextArcheType(ArcheTypeData diffContextArcheTypeData)
        {
            ArcheType.CopyToCached(diffContextArcheTypeData.ArcheType, ref _cachedArcheType);
            for (var i = 0; i < diffContextArcheTypeData.ArcheType.SharedDataIndexesLength; i++)
            {
                ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType,
                    diffContextArcheTypeData.SharedComponentDatas[i]
                        .GetSharedComponentDataIndex(Context.SharedComponentDics));
            }
        }

        private void InternalAddConfig(ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex? sharedDataIndex,
            out ArcheTypeData nextArcheTypeData)
        {
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (sharedDataIndex == null)
                ArcheType.AddConfig(ref _cachedArcheType, config);
            else
                ArcheType.AddConfig(ref _cachedArcheType, config, sharedDataIndex.Value);

            nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
        }

        private void InternalAddConfigTransferEntity(Entity entity,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex? sharedDataIndex,
            out ArcheTypeData nextArcheTypeData)
        {
            InternalAddConfig(prevArcheTypeData, config, sharedDataIndex, out nextArcheTypeData);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        private bool InternalAddConfigTransferArcheTypeData(
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex? sharedDataIndex,
            out ArcheTypeData nextArcheTypeData, out int preEntityCount)
        {
            if (prevArcheTypeData.EntityCount == 0)
            {
                nextArcheTypeData = null;
                preEntityCount = 0;
                return false;
            }

            Helper.ResizeRefArray(ref _cachedInternalEntities, 0, prevArcheTypeData.EntityCount);
            prevArcheTypeData.GetAllEntities(ref _cachedInternalEntities, 0);

            InternalAddConfig(prevArcheTypeData, config, sharedDataIndex, out nextArcheTypeData);
            preEntityCount = nextArcheTypeData.EntityCount;
            ArcheTypeData.TransferAllEntities(GlobalVersion,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);

            return true;
        }

        private bool InternalArcheTypeSharedUpdate<TComponent>(ComponentConfig config, TComponent component)
            where TComponent : IComponent
        {
            if (config.IsShared)
            {
                return ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType,
                    Context.SharedComponentDics.GetDic(config).GetSharedDataIndex(component));
            }
            return false;
        }

        private int InternalGetEntitiesTracker(EntityTracker tracker, ArcheTypeData archeTypeData,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (!tracker.GetArcheTypeDataChunks(archeTypeData, out var chunks))
                return 0;

            var entityIndex = destStartingIndex;
            for (var i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                Helper.ResizeRefArray(ref destEntities, entityIndex, chunk.EntityCount);
                entityIndex += chunk.GetEntities(ref destEntities,
                    entityIndex);
            }

            return entityIndex - destStartingIndex;
        }

        private int InternalGetComponentsTracker<TComponent>(ComponentConfig config, EntityTracker tracker, ArcheTypeData archeTypeData,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            if (!archeTypeData.HasConfig(config))
                return 0;

            if (!tracker.GetArcheTypeDataChunks(archeTypeData, out var chunks))
                return 0;

            var configOffset = archeTypeData.GetConfigOffset(config);
            var componentIndex = destStartingIndex;
            for (var i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                Helper.ResizeRefArray(ref destComponents, componentIndex, chunk.EntityCount);
                componentIndex += chunk.GetAllComponents(ref destComponents,
                    componentIndex, configOffset);
            }

            return componentIndex - destStartingIndex;
        }

        private int InternalGetManagedComponentsTracker<TComponent>(ComponentConfig config, EntityTracker tracker, ArcheTypeData archeTypeData,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : IManagedComponent
        {
            if (!archeTypeData.HasConfig(config))
                return 0;

            if (!tracker.GetArcheTypeDataChunks(archeTypeData, out var chunks))
                return 0;

            var configOffset = archeTypeData.GetConfigOffset(config);
            var componentIndex = destStartingIndex;
            for (var i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                Helper.ResizeRefArray(ref destComponents, componentIndex, chunk.EntityCount);
                componentIndex += chunk.GetAllManagedComponents(ref destComponents,
                    componentIndex, configOffset);
            }

            return componentIndex - destStartingIndex;
        }

        private void InternalUpdateSharedTransferArcheTypeData(ArcheTypeData prevArcheTypeData, SharedDataIndex sharedDataIndex)
        {
            var prevEntityCount = prevArcheTypeData.EntityCount;
            Helper.ResizeRefArray(ref _cachedInternalEntities, 0, prevEntityCount);
            prevArcheTypeData.GetAllEntities(ref _cachedInternalEntities, 0);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType, sharedDataIndex))
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferAllEntities(GlobalVersion,
                    prevArcheTypeData,
                    nextArcheTypeData,
                    _entityDatas);
            }
        }

        private void InternalRemoveConfigTransferEntity(Entity entity,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, bool removeShared)
        {
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (removeShared)
                ArcheType.RemoveConfigAndSharedDataIndex(ref _cachedArcheType, config);
            else
                ArcheType.RemoveConfig(ref _cachedArcheType, config);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        private void InternalRemoveConfigTransferArcheTypeData(
            ArcheTypeData prevArcheTypeData, ComponentConfig config, bool removeShared)
        {
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (removeShared)
                ArcheType.RemoveConfigAndSharedDataIndex(ref _cachedArcheType, config);
            else
                ArcheType.RemoveConfig(ref _cachedArcheType, config);

            Helper.ResizeRefArray(ref _cachedInternalEntities, 0, prevArcheTypeData.EntityCount);
            prevArcheTypeData.GetAllEntities(ref _cachedInternalEntities, 0);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferAllEntities(GlobalVersion,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        #endregion

        #region Assert

        private void AssertNotExistEntity(Entity entity, out EntityData entityData, out ArcheTypeData archeTypeData)
        {
            if (!InternalHasEntity(entity, out entityData, out archeTypeData))
                throw new EntityNotExistException(entity);
        }

        private void AssertNotHaveComponent(ComponentConfig config, ArcheTypeData archeTypeData)
        {
            if (!archeTypeData.HasConfig(config))
                throw new ComponentNotHaveException(config.ComponentType);
        }

        private void AssertAlreadyHasComponent(ComponentConfig config, ArcheTypeData archeTypeData)
        {
            if (archeTypeData.HasConfig(config))
                throw new ComponentAlreadyHaveException(config.ComponentType);
        }

        #endregion
    }
}
