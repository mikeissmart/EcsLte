using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

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

        public EcsContext Context { get; private set; }

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
        }

        #region Private

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

        private bool InternalHasEntity(Entity entity, out EntityData entityData, out ArcheTypeData archeTypeData)
        {
            if (entity.Id > 0 && entity.Id < _entitiesLength)
            {
                entityData = _entityDatas[entity.Id];
                if (entityData.ArcheTypeIndex != ArcheTypeIndex.Null)
                {
                    archeTypeData = Context.ArcheTypes.GetArcheTypeData(entityData.ArcheTypeIndex);
                    return archeTypeData.GetEntity(entityData.EntityIndex) == entity;
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

            entityData = archeTypeData.AddEntity(entity, clearComponents);
            _entityDatas[entity.Id] = entityData;

            for (var i = 0; i < archeTypeData.ArcheType.ConfigsLength; i++)
                Context.Tracking.TrackAdd(entity, archeTypeData.ArcheType.Configs[i], archeTypeData);
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

            archeTypeData.AddEntities(entities, startingIndex, count,
                _entityDatas, clearComponents);

            for (var i = 0; i < archeTypeData.ArcheType.ConfigsLength; i++)
                Context.Tracking.TrackAdds(entities, startingIndex, count,
                    archeTypeData.ArcheType.Configs[i], archeTypeData);
        }

        private void DeallocEntity(Entity entity, ArcheTypeData archeTypeData)
        {
            archeTypeData.RemoveEntity(entity, _entityDatas);
            Context.Tracking.EntityDestroyed(entity, archeTypeData);

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
                archeTypeData.GetEntities(ref _cachedInternalEntities, 0);
                for (var i = 0; i < count; i++)
                {
                    var entity = _cachedInternalEntities[i];
                    _entityDatas[entity.Id] = EntityData.Null;
                    _reusableEntities[_reusableEntitiesCount++] = entity;
                }
                _entitiesCount -= count;

                archeTypeData.RemoveAllEntities();
                Context.Tracking.ArcheTypeEntitiesDestroyed(archeTypeData);
            }
        }

        private void InternalDestroyTracker(EntityTracker tracker, ArcheTypeData archeTypeData)
        {
            var trackedEntityCount = tracker.GetArcheTypeDataEntities(archeTypeData,
                ref _cachedInternalEntities, 0);
            for (var i = 0; i < trackedEntityCount; i++)
            {
                DeallocEntity(
                    _cachedInternalEntities[i],
                    archeTypeData);
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

                copyArcheTypeData.CopyComponentsDifferentArcheTypeDataSameComponents(
                    srcArcheTypeData,
                    0,
                    prevEntityCount,
                    srcArcheTypeData.EntityCount);
            }

            return srcArcheTypeData.EntityCount;
        }

        private int InternalCopyToTracker(EntityTracker tracker, ArcheTypeData srcArcheTypeData,
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
        }

        private int InternalDuplicateArcheTypeData(ArcheTypeData archeTypeData,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (archeTypeData.EntityCount == 0)
                return 0;

            var preEntityCount = archeTypeData.EntityCount;
            Helper.ResizeRefArray(ref destEntities, destStartingIndex, preEntityCount);
            CheckAndAllocEntities(archeTypeData, false,
                ref destEntities, destStartingIndex, archeTypeData.EntityCount);

            archeTypeData.CopyComponentsSameArcheTypeData(
                0,
                preEntityCount,
                preEntityCount);

            return archeTypeData.EntityCount - preEntityCount;
        }

        private int InternalDuplicateTracker(EntityTracker tracker, ArcheTypeData archeTypeData,
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
                    archeTypeData.CopyComponentsSameArcheTypeData(
                        _entityDatas[_cachedInternalEntities[i].Id].EntityIndex,
                        preEntityCount + i,
                        1);
                }
            }

            return trackedEntityCount;
        }

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

        private void InternalAddConfigTrackingTransferEntity(Entity entity,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex? sharedDataIndex,
            out ArcheTypeData nextArcheTypeData)
        {
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (sharedDataIndex == null)
                ArcheType.AddConfig(ref _cachedArcheType, config);
            else
                ArcheType.AddConfig(ref _cachedArcheType, config, sharedDataIndex.Value);

            nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);

            Context.Tracking.TrackAdd(entity, config, nextArcheTypeData);
            Context.Tracking.TrackArcheTypeDataChange(entity,
                prevArcheTypeData, nextArcheTypeData);
        }

        private bool InternalAddConfigTrackingTransferArcheTypeData(
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex? sharedDataIndex,
            out ArcheTypeData nextArcheTypeData, out int preEntityCount)
        {
            if (prevArcheTypeData.EntityCount == 0)
            {
                nextArcheTypeData = null;
                preEntityCount = 0;
                return false;
            }

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (sharedDataIndex == null)
                ArcheType.AddConfig(ref _cachedArcheType, config);
            else
                ArcheType.AddConfig(ref _cachedArcheType, config, sharedDataIndex.Value);

            Helper.ResizeRefArray(ref _cachedInternalEntities, 0, prevArcheTypeData.EntityCount);
            prevArcheTypeData.GetEntities(ref _cachedInternalEntities, 0);
            var prevEntityCount = prevArcheTypeData.EntityCount;

            nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            preEntityCount = nextArcheTypeData.EntityCount;
            ArcheTypeData.TransferAllEntities(_entityDatas,
                prevArcheTypeData,
                nextArcheTypeData);

            Context.Tracking.TrackAdds(_cachedInternalEntities, 0, prevEntityCount,
                config, nextArcheTypeData);
            Context.Tracking.TrackAllArcheTypeDataChange(prevArcheTypeData, nextArcheTypeData);

            return true;
        }

        private bool InternalAddConfigTrackingTransferTracker(EntityTracker tracker,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex? sharedDataIndex,
            out ArcheTypeData nextArcheTypeData, out int preEntityCount)
        {
            var trackedEntityCount = tracker.GetArcheTypeDataEntities(prevArcheTypeData,
                ref _cachedInternalEntities, 0);
            if (trackedEntityCount == 0)
            {
                nextArcheTypeData = null;
                preEntityCount = 0;
                return false;
            }

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (sharedDataIndex == null)
                ArcheType.AddConfig(ref _cachedArcheType, config);
            else
                ArcheType.AddConfig(ref _cachedArcheType, config, sharedDataIndex.Value);

            nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            preEntityCount = nextArcheTypeData.EntityCount;

            for (var i = 0; i < trackedEntityCount; i++)
            {
                var entity = _cachedInternalEntities[i];

                ArcheTypeData.TransferEntity(entity, _entityDatas,
                    prevArcheTypeData, nextArcheTypeData);

                Context.Tracking.TrackAdd(entity, config, nextArcheTypeData);
                Context.Tracking.TrackArcheTypeDataChange(entity,
                    prevArcheTypeData, nextArcheTypeData);
            }

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

        private int InternalGetComponentsTracker<TComponent>(EntityTracker tracker, ArcheTypeData archeTypeData,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            var trackedEntityCount = 0;
            var config = ComponentConfig<TComponent>.Config;
            if (archeTypeData.HasConfig(config))
            {
                trackedEntityCount = tracker.GetArcheTypeDataEntities(archeTypeData,
                    ref _cachedInternalEntities, 0);
                if (trackedEntityCount > 0)
                {
                    Helper.ResizeRefArray(ref destComponents, destStartingIndex, trackedEntityCount);

                    var configOffset = archeTypeData.GetConfigOffset(config);
                    for (var i = 0; i < trackedEntityCount; i++, destStartingIndex++)
                    {
                        destComponents[destStartingIndex] = archeTypeData.GetComponent<TComponent>(
                            _entityDatas[_cachedInternalEntities[i].Id].EntityIndex,
                            configOffset);
                    }
                }
            }

            return trackedEntityCount;
        }

        private int InternalGetManagedComponentsTracker<TComponent>(EntityTracker tracker, ArcheTypeData archeTypeData,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : IManagedComponent
        {
            var trackedEntityCount = 0;
            var config = ComponentConfig<TComponent>.Config;
            if (archeTypeData.HasConfig(config))
            {
                trackedEntityCount = tracker.GetArcheTypeDataEntities(archeTypeData,
                    ref _cachedInternalEntities, 0);
                if (trackedEntityCount > 0)
                {
                    Helper.ResizeRefArray(ref destComponents, destStartingIndex, trackedEntityCount);

                    var configOffset = archeTypeData.GetConfigOffset(config);
                    for (var i = 0; i < trackedEntityCount; i++, destStartingIndex++)
                    {
                        destComponents[destStartingIndex] = archeTypeData.GetManagedComponent<TComponent>(
                            _entityDatas[_cachedInternalEntities[i].Id].EntityIndex,
                            configOffset);
                    }
                }
            }

            return trackedEntityCount;
        }

        private int InternalUpdateTrackingTracker(EntityTracker tracker,
            ArcheTypeData archeTypeData, ComponentConfig config)
        {
            if (archeTypeData.HasConfig(config))
            {
                var trackedEntityCount = tracker.GetArcheTypeDataEntities(archeTypeData,
                    ref _cachedInternalEntities, 0);
                if (trackedEntityCount > 0)
                {
                    Context.Tracking.TrackUpdates(_cachedInternalEntities, 0, trackedEntityCount,
                        config, archeTypeData);
                }

                return trackedEntityCount;
            }

            return 0;
        }

        private void InternalUpdateSharedTrackingTransferArcheTypeData(
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex sharedDataIndex)
        {
            var prevEntityCount = prevArcheTypeData.EntityCount;
            Helper.ResizeRefArray(ref _cachedInternalEntities, 0, prevEntityCount);
            prevArcheTypeData.GetEntities(ref _cachedInternalEntities, 0);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType, sharedDataIndex))
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferAllEntities(_entityDatas,
                    prevArcheTypeData, nextArcheTypeData);

                Context.Tracking.TrackAllArcheTypeDataChange(
                    prevArcheTypeData, nextArcheTypeData);

                prevArcheTypeData = nextArcheTypeData;
            }

            Context.Tracking.TrackUpdates(_cachedInternalEntities, 0, prevEntityCount,
                config, prevArcheTypeData);
        }

        private void InternalUpdateSharedTrackingTransferTracker(EntityTracker tracker,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, SharedDataIndex sharedDataIndex)
        {
            if (prevArcheTypeData.HasConfig(config))
            {
                var trackedEntityCount = tracker.GetArcheTypeDataEntities(prevArcheTypeData,
                    ref _cachedInternalEntities, 0);
                if (trackedEntityCount > 0)
                {
                    ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
                    if (ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType, sharedDataIndex))
                    {
                        var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                        for (var j = 0; j < trackedEntityCount; j++)
                        {
                            var entity = _cachedInternalEntities[j];
                            ArcheTypeData.TransferEntity(entity, _entityDatas,
                                prevArcheTypeData, nextArcheTypeData);

                            Context.Tracking.TrackArcheTypeDataChange(entity,
                                prevArcheTypeData, nextArcheTypeData);
                        }
                    }

                    Context.Tracking.TrackUpdates(_cachedInternalEntities, 0, trackedEntityCount,
                        config, prevArcheTypeData);
                }
            }
        }

        private void InternalRemoveConfigTrackingTransferEntity(Entity entity,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, bool removeShared)
        {
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (removeShared)
                ArcheType.RemoveConfig(ref _cachedArcheType, config);
            else
                ArcheType.RemoveConfigAndSharedDataIndex(ref _cachedArcheType, config);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);

            Context.Tracking.TrackRemove(entity, config, nextArcheTypeData);
            Context.Tracking.TrackArcheTypeDataChange(entity,
                prevArcheTypeData, nextArcheTypeData);
        }

        private void InternalRemoveConfigTrackingTransferArcheTypeData(
            ArcheTypeData prevArcheTypeData, ComponentConfig config, bool removeShared)
        {
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (removeShared)
                ArcheType.RemoveConfig(ref _cachedArcheType, config);
            else
                ArcheType.RemoveConfigAndSharedDataIndex(ref _cachedArcheType, config);

            Helper.ResizeRefArray(ref _cachedInternalEntities, 0, prevArcheTypeData.EntityCount);
            prevArcheTypeData.GetEntities(ref _cachedInternalEntities, 0);
            var prevEntityCount = prevArcheTypeData.EntityCount;

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferAllEntities(_entityDatas,
                prevArcheTypeData,
                nextArcheTypeData);

            Context.Tracking.TrackRemoves(_cachedInternalEntities, 0, prevEntityCount,
                config, nextArcheTypeData);
            Context.Tracking.TrackAllArcheTypeDataChange(prevArcheTypeData, nextArcheTypeData);
        }

        private void InternalRemoveConfigTrackingTransferArcheTypeData(EntityTracker tracker,
            ArcheTypeData prevArcheTypeData, ComponentConfig config, bool removeShared)
        {
            var trackedEntityCount = tracker.GetArcheTypeDataEntities(prevArcheTypeData,
                ref _cachedInternalEntities, 0);
            if (trackedEntityCount == 0)
                return;

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (removeShared)
                ArcheType.RemoveConfig(ref _cachedArcheType, config);
            else
                ArcheType.RemoveConfigAndSharedDataIndex(ref _cachedArcheType, config);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);

            for (var i = 0; i < trackedEntityCount; i++)
            {
                var entity = _cachedInternalEntities[i];

                ArcheTypeData.TransferEntity(entity, _entityDatas,
                    prevArcheTypeData, nextArcheTypeData);

                Context.Tracking.TrackRemove(entity, config, nextArcheTypeData);
                Context.Tracking.TrackArcheTypeDataChange(entity,
                    prevArcheTypeData, nextArcheTypeData);
            }
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
