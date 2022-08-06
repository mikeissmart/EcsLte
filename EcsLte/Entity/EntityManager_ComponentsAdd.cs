using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void AddComponents<T1, T2>(Entity entity,
            T1 component1, T2 component2)
            where T1 : IComponent
            where T2 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
        }

        public void AddComponents<T1, T2, T3>(Entity entity,
            T1 component1, T2 component2, T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);
            AssertAlreadyHasComponent(config3, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);
            config3.Adapter.AddConfig(ref _cachedArcheType, component3, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);
            config3.Adapter.SetComponent(entityData.EntityIndex, component3, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config3, nextArcheTypeData);
        }

        public void AddComponents<T1, T2, T3, T4>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);
            AssertAlreadyHasComponent(config3, prevArcheTypeData);
            AssertAlreadyHasComponent(config4, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);
            config3.Adapter.AddConfig(ref _cachedArcheType, component3, Context.SharedComponentDics);
            config4.Adapter.AddConfig(ref _cachedArcheType, component4, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);
            config3.Adapter.SetComponent(entityData.EntityIndex, component3, nextArcheTypeData);
            config4.Adapter.SetComponent(entityData.EntityIndex, component4, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config3, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config4, nextArcheTypeData);
        }

        public void AddComponents<T1, T2, T3, T4, T5>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);
            AssertAlreadyHasComponent(config3, prevArcheTypeData);
            AssertAlreadyHasComponent(config4, prevArcheTypeData);
            AssertAlreadyHasComponent(config5, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);
            config3.Adapter.AddConfig(ref _cachedArcheType, component3, Context.SharedComponentDics);
            config4.Adapter.AddConfig(ref _cachedArcheType, component4, Context.SharedComponentDics);
            config5.Adapter.AddConfig(ref _cachedArcheType, component5, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);
            config3.Adapter.SetComponent(entityData.EntityIndex, component3, nextArcheTypeData);
            config4.Adapter.SetComponent(entityData.EntityIndex, component4, nextArcheTypeData);
            config5.Adapter.SetComponent(entityData.EntityIndex, component5, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config3, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config4, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config5, nextArcheTypeData);
        }

        public void AddComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);
            AssertAlreadyHasComponent(config3, prevArcheTypeData);
            AssertAlreadyHasComponent(config4, prevArcheTypeData);
            AssertAlreadyHasComponent(config5, prevArcheTypeData);
            AssertAlreadyHasComponent(config6, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);
            config3.Adapter.AddConfig(ref _cachedArcheType, component3, Context.SharedComponentDics);
            config4.Adapter.AddConfig(ref _cachedArcheType, component4, Context.SharedComponentDics);
            config5.Adapter.AddConfig(ref _cachedArcheType, component5, Context.SharedComponentDics);
            config6.Adapter.AddConfig(ref _cachedArcheType, component6, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);
            config3.Adapter.SetComponent(entityData.EntityIndex, component3, nextArcheTypeData);
            config4.Adapter.SetComponent(entityData.EntityIndex, component4, nextArcheTypeData);
            config5.Adapter.SetComponent(entityData.EntityIndex, component5, nextArcheTypeData);
            config6.Adapter.SetComponent(entityData.EntityIndex, component6, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config3, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config4, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config5, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config6, nextArcheTypeData);
        }

        public void AddComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);
            AssertAlreadyHasComponent(config3, prevArcheTypeData);
            AssertAlreadyHasComponent(config4, prevArcheTypeData);
            AssertAlreadyHasComponent(config5, prevArcheTypeData);
            AssertAlreadyHasComponent(config6, prevArcheTypeData);
            AssertAlreadyHasComponent(config7, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);
            config3.Adapter.AddConfig(ref _cachedArcheType, component3, Context.SharedComponentDics);
            config4.Adapter.AddConfig(ref _cachedArcheType, component4, Context.SharedComponentDics);
            config5.Adapter.AddConfig(ref _cachedArcheType, component5, Context.SharedComponentDics);
            config6.Adapter.AddConfig(ref _cachedArcheType, component6, Context.SharedComponentDics);
            config7.Adapter.AddConfig(ref _cachedArcheType, component7, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);
            config3.Adapter.SetComponent(entityData.EntityIndex, component3, nextArcheTypeData);
            config4.Adapter.SetComponent(entityData.EntityIndex, component4, nextArcheTypeData);
            config5.Adapter.SetComponent(entityData.EntityIndex, component5, nextArcheTypeData);
            config6.Adapter.SetComponent(entityData.EntityIndex, component6, nextArcheTypeData);
            config7.Adapter.SetComponent(entityData.EntityIndex, component7, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config3, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config4, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config5, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config6, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config7, nextArcheTypeData);
        }

        public void AddComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7, T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;
            var config8 = ComponentConfig<T8>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7,
                config8);

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertAlreadyHasComponent(config1, prevArcheTypeData);
            AssertAlreadyHasComponent(config2, prevArcheTypeData);
            AssertAlreadyHasComponent(config3, prevArcheTypeData);
            AssertAlreadyHasComponent(config4, prevArcheTypeData);
            AssertAlreadyHasComponent(config5, prevArcheTypeData);
            AssertAlreadyHasComponent(config6, prevArcheTypeData);
            AssertAlreadyHasComponent(config7, prevArcheTypeData);
            AssertAlreadyHasComponent(config8, prevArcheTypeData);

            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.AddConfig(ref _cachedArcheType, component1, Context.SharedComponentDics);
            config2.Adapter.AddConfig(ref _cachedArcheType, component2, Context.SharedComponentDics);
            config3.Adapter.AddConfig(ref _cachedArcheType, component3, Context.SharedComponentDics);
            config4.Adapter.AddConfig(ref _cachedArcheType, component4, Context.SharedComponentDics);
            config5.Adapter.AddConfig(ref _cachedArcheType, component5, Context.SharedComponentDics);
            config6.Adapter.AddConfig(ref _cachedArcheType, component6, Context.SharedComponentDics);
            config7.Adapter.AddConfig(ref _cachedArcheType, component7, Context.SharedComponentDics);
            config8.Adapter.AddConfig(ref _cachedArcheType, component8, Context.SharedComponentDics);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(entity, _entityDatas,
                prevArcheTypeData, nextArcheTypeData);
            var entityData = _entityDatas[entity.Id];

            config1.Adapter.SetComponent(entityData.EntityIndex, component1, nextArcheTypeData);
            config2.Adapter.SetComponent(entityData.EntityIndex, component2, nextArcheTypeData);
            config3.Adapter.SetComponent(entityData.EntityIndex, component3, nextArcheTypeData);
            config4.Adapter.SetComponent(entityData.EntityIndex, component4, nextArcheTypeData);
            config5.Adapter.SetComponent(entityData.EntityIndex, component5, nextArcheTypeData);
            config6.Adapter.SetComponent(entityData.EntityIndex, component6, nextArcheTypeData);
            config7.Adapter.SetComponent(entityData.EntityIndex, component7, nextArcheTypeData);
            config8.Adapter.SetComponent(entityData.EntityIndex, component8, nextArcheTypeData);

            Context.Tracking.TrackArcheTypeDataChange(entity, prevArcheTypeData, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config1, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config2, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config3, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config4, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config5, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config6, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config7, nextArcheTypeData);
            Context.Tracking.TrackAdd(entity, config8, nextArcheTypeData);
        }

        public void AddComponents<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);

            AssertAlreadyHasComponent(config, prevArcheTypeData);
            if (InternalAddConfigTrackingTransferArcheTypeData(prevArcheTypeData, config,
                null,
                out var nextArcheTypeData, out var preEntityCount))
            {
                nextArcheTypeData.SetComponents(preEntityCount, nextArcheTypeData.EntityCount,
                    config, component);
            }
        }

        public void AddComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var prevArcheTypeData = filteredArcheTypeDatas[i];
                if (!prevArcheTypeData.HasConfig(config))
                {
                    AssertAlreadyHasComponent(config, prevArcheTypeData);
                    InternalAddConfigTrackingTransferArcheTypeData(prevArcheTypeData, config,
                        null,
                        out var nextArcheTypeData, out var preEntityCount);

                    nextArcheTypeData.SetComponents(preEntityCount, nextArcheTypeData.EntityCount,
                        config, component);
                }
            }
        }

        public void AddComponents<TComponent>(EntityTracker tracker, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeDatas = tracker.CachedArcheTypeDatas;
            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                var prevArcheTypeData = archeTypeDatas[i];
                if (!prevArcheTypeData.HasConfig(config))
                {
                    if (InternalAddConfigTrackingTransferTracker(tracker, prevArcheTypeData, config,
                        null,
                        out var nextArcheTypeData, out var preEntityCount))
                    {
                        nextArcheTypeData.SetComponents(preEntityCount, nextArcheTypeData.EntityCount - preEntityCount,
                            config, component);
                    }
                }
            }
        }

        public void AddComponents<TComponent>(EntityQuery query, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                AddComponents(query.Filter, component);
            else if (query.Filter == null && query.Tracker != null)
                AddComponents(query.Tracker, component);
            else if (query.Filter != null && query.Tracker != null)
            {
                var config = ComponentConfig<TComponent>.Config;
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    var prevArcheTypeData = filteredArcheTypeDatas[i];
                    if (!prevArcheTypeData.HasConfig(config))
                    {
                        if (InternalAddConfigTrackingTransferTracker(query.Tracker, prevArcheTypeData, config,
                            null,
                            out var nextArcheTypeData, out var preEntityCount))
                        {
                            nextArcheTypeData.SetComponents(preEntityCount, nextArcheTypeData.EntityCount - preEntityCount,
                                config, component);
                        }
                    }
                }
            }
        }

        public void AddManagedComponents<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);

            AssertAlreadyHasComponent(config, prevArcheTypeData);
            if (InternalAddConfigTrackingTransferArcheTypeData(prevArcheTypeData, config,
                null,
                out var nextArcheTypeData, out var preEntityCount))
            {
                nextArcheTypeData.SetManagedComponents(preEntityCount, nextArcheTypeData.EntityCount,
                    config, component);
            }
        }

        public void AddManagedComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var prevArcheTypeData = filteredArcheTypeDatas[i];
                if (!prevArcheTypeData.HasConfig(config))
                {
                    AssertAlreadyHasComponent(config, prevArcheTypeData);
                    InternalAddConfigTrackingTransferArcheTypeData(prevArcheTypeData, config,
                        null,
                        out var nextArcheTypeData, out var preEntityCount);

                    nextArcheTypeData.SetManagedComponents(preEntityCount, nextArcheTypeData.EntityCount,
                        config, component);
                }
            }
        }

        public void AddManagedComponents<TComponent>(EntityTracker tracker, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeDatas = tracker.CachedArcheTypeDatas;
            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                var prevArcheTypeData = archeTypeDatas[i];
                if (!prevArcheTypeData.HasConfig(config))
                {
                    if (InternalAddConfigTrackingTransferTracker(tracker, prevArcheTypeData, config,
                        null,
                        out var nextArcheTypeData, out var preEntityCount))
                    {
                        nextArcheTypeData.SetManagedComponents(preEntityCount, nextArcheTypeData.EntityCount - preEntityCount,
                            config, component);
                    }
                }
            }
        }

        public void AddManagedComponents<TComponent>(EntityQuery query, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                AddManagedComponents(query.Filter, component);
            else if (query.Filter == null && query.Tracker != null)
                AddManagedComponents(query.Tracker, component);
            else if (query.Filter != null && query.Tracker != null)
            {
                var config = ComponentConfig<TComponent>.Config;
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    var prevArcheTypeData = filteredArcheTypeDatas[i];
                    if (!prevArcheTypeData.HasConfig(config))
                    {
                        if (InternalAddConfigTrackingTransferTracker(query.Tracker, prevArcheTypeData, config,
                            null,
                            out var nextArcheTypeData, out var preEntityCount))
                        {
                            nextArcheTypeData.SetManagedComponents(preEntityCount, nextArcheTypeData.EntityCount - preEntityCount,
                                config, component);
                        }
                    }
                }
            }
        }

        public void AddSharedComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var prevArcheTypeData = filteredArcheTypeDatas[i];
                if (!prevArcheTypeData.HasConfig(config))
                {
                    AssertAlreadyHasComponent(config, prevArcheTypeData);
                    InternalAddConfigTrackingTransferArcheTypeData(prevArcheTypeData, config,
                        null,
                        out var _, out var _);
                }
            }
        }

        public void AddSharedComponents<TComponent>(EntityTracker tracker, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeDatas = tracker.CachedArcheTypeDatas;
            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                var prevArcheTypeData = archeTypeDatas[i];
                if (!prevArcheTypeData.HasConfig(config))
                {
                    InternalAddConfigTrackingTransferTracker(tracker, prevArcheTypeData, config,
                        null,
                        out var _, out var _);
                }
            }
        }

        public void AddSharedComponents<TComponent>(EntityQuery query, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                AddSharedComponents(query.Filter, component);
            else if (query.Filter == null && query.Tracker != null)
                AddSharedComponents(query.Tracker, component);
            else if (query.Filter != null && query.Tracker != null)
            {
                var config = ComponentConfig<TComponent>.Config;
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    var prevArcheTypeData = filteredArcheTypeDatas[i];
                    if (!prevArcheTypeData.HasConfig(config))
                    {
                        InternalAddConfigTrackingTransferTracker(query.Tracker, prevArcheTypeData, config,
                            null,
                            out var _, out var _);
                    }
                }
            }
        }
    }
}
