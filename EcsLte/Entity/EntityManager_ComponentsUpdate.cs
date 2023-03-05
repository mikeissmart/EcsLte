using EcsLte.Utilities;
using System.Collections.Generic;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void UpdateComponents<T1>(Entity entity,
            T1 component1)
            where T1 : IComponent
        {
            var config1 = ComponentConfigs.Instance
                .GetConfig(component1.GetType());

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            if (InternalArcheTypeSharedUpdate(config1, component1))
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
        }

        public void UpdateComponents<T1, T2>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
        }

        public void UpdateComponents<T1, T2, T3>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);
            changeArcheType |= InternalArcheTypeSharedUpdate(config3, component3);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
            config3.Adapter.SetComponent(entityData, component3, archeTypeData);
        }

        public void UpdateComponents<T1, T2, T3, T4>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);
            changeArcheType |= InternalArcheTypeSharedUpdate(config3, component3);
            changeArcheType |= InternalArcheTypeSharedUpdate(config4, component4);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
            config3.Adapter.SetComponent(entityData, component3, archeTypeData);
            config4.Adapter.SetComponent(entityData, component4, archeTypeData);
        }

        public void UpdateComponents<T1, T2, T3, T4, T5>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);
            changeArcheType |= InternalArcheTypeSharedUpdate(config3, component3);
            changeArcheType |= InternalArcheTypeSharedUpdate(config4, component4);
            changeArcheType |= InternalArcheTypeSharedUpdate(config5, component5);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
            config3.Adapter.SetComponent(entityData, component3, archeTypeData);
            config4.Adapter.SetComponent(entityData, component4, archeTypeData);
            config5.Adapter.SetComponent(entityData, component5, archeTypeData);
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);
            AssertNotHaveComponent(config6, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);
            changeArcheType |= InternalArcheTypeSharedUpdate(config3, component3);
            changeArcheType |= InternalArcheTypeSharedUpdate(config4, component4);
            changeArcheType |= InternalArcheTypeSharedUpdate(config5, component5);
            changeArcheType |= InternalArcheTypeSharedUpdate(config6, component6);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
            config3.Adapter.SetComponent(entityData, component3, archeTypeData);
            config4.Adapter.SetComponent(entityData, component4, archeTypeData);
            config5.Adapter.SetComponent(entityData, component5, archeTypeData);
            config6.Adapter.SetComponent(entityData, component6, archeTypeData);
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);
            AssertNotHaveComponent(config6, archeTypeData);
            AssertNotHaveComponent(config7, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);
            changeArcheType |= InternalArcheTypeSharedUpdate(config3, component3);
            changeArcheType |= InternalArcheTypeSharedUpdate(config4, component4);
            changeArcheType |= InternalArcheTypeSharedUpdate(config5, component5);
            changeArcheType |= InternalArcheTypeSharedUpdate(config6, component6);
            changeArcheType |= InternalArcheTypeSharedUpdate(config7, component7);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
            config3.Adapter.SetComponent(entityData, component3, archeTypeData);
            config4.Adapter.SetComponent(entityData, component4, archeTypeData);
            config5.Adapter.SetComponent(entityData, component5, archeTypeData);
            config6.Adapter.SetComponent(entityData, component6, archeTypeData);
            config7.Adapter.SetComponent(entityData, component7, archeTypeData);
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
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
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);
            Context.AssertStructualChangeAvailable();
            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);
            AssertNotHaveComponent(config6, archeTypeData);
            AssertNotHaveComponent(config7, archeTypeData);
            AssertNotHaveComponent(config8, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(archeTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            changeArcheType |= InternalArcheTypeSharedUpdate(config1, component1);
            changeArcheType |= InternalArcheTypeSharedUpdate(config2, component2);
            changeArcheType |= InternalArcheTypeSharedUpdate(config3, component3);
            changeArcheType |= InternalArcheTypeSharedUpdate(config4, component4);
            changeArcheType |= InternalArcheTypeSharedUpdate(config5, component5);
            changeArcheType |= InternalArcheTypeSharedUpdate(config6, component6);
            changeArcheType |= InternalArcheTypeSharedUpdate(config7, component7);
            changeArcheType |= InternalArcheTypeSharedUpdate(config8, component8);

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    archeTypeData,
                    nextArcheTypeData,
                    _entityDatas);

                archeTypeData = nextArcheTypeData;
                entityData = _entityDatas[entity.Id];
            }

            config1.Adapter.SetComponent(entityData, component1, archeTypeData);
            config2.Adapter.SetComponent(entityData, component2, archeTypeData);
            config3.Adapter.SetComponent(entityData, component3, archeTypeData);
            config4.Adapter.SetComponent(entityData, component4, archeTypeData);
            config5.Adapter.SetComponent(entityData, component5, archeTypeData);
            config6.Adapter.SetComponent(entityData, component6, archeTypeData);
            config7.Adapter.SetComponent(entityData, component7, archeTypeData);
            config8.Adapter.SetComponent(entityData, component8, archeTypeData);
        }

        public void UpdateComponents<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            AssertNotHaveComponent(config, archeTypeData);

            if (archeTypeData.EntityCount > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);
                archeTypeData.SetAllComponents(GlobalVersion,
                    0,
                    archeTypeData.EntityCount,
                    config,
                    component);
            }
        }

        public void UpdateComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            ChangeVersion.IncVersion(ref _globalVersion);
            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.HasConfig(config) && archeTypeData.EntityCount > 0)
                {
                    if (!incVersion)
                    {
                        ChangeVersion.IncVersion(ref _globalVersion);
                        incVersion = true;
                    }

                    archeTypeData.SetAllComponents(GlobalVersion,
                        0,
                        archeTypeData.EntityCount,
                        config,
                        component);
                }
            }
        }

        public void UpdateManagedComponents<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            AssertNotHaveComponent(config, archeTypeData);

            if (archeTypeData.EntityCount > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);
                archeTypeData.SetAllManagedComponents(GlobalVersion,
                    0,
                    archeTypeData.EntityCount,
                    config,
                    component);
            }
        }

        public void UpdateManagedComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.HasConfig(config) && archeTypeData.EntityCount > 0)
                {
                    if (!incVersion)
                    {
                        ChangeVersion.IncVersion(ref _globalVersion);
                        incVersion = true;
                    }

                    archeTypeData.SetAllManagedComponents(GlobalVersion,
                        0,
                        archeTypeData.EntityCount,
                        config,
                        component);
                }
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            ChangeVersion.IncVersion(ref _globalVersion);
            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.HasConfig(config) && archeTypeData.EntityCount > 0)
                {
                    if (!incVersion)
                    {
                        ChangeVersion.IncVersion(ref _globalVersion);
                        incVersion = true;
                    }

                    InternalUpdateSharedTransferArcheTypeData(archeTypeData,
                        Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component));
                }
            }
        }

        internal void EntityQuery_TransferNextArtcheType(in Entity[] entities, int count,
            ArcheTypeIndex archeTypeIndex, List<(IComponent, ComponentConfig)> components)
        {
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeTypeIndex);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            var changeArcheType = false;
            for (var i = 0; i < components.Count; i++)
            {
                var item = components[i];
                changeArcheType |= ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType,
                    Context.SharedComponentDics.GetDic(item.Item2).GetSharedDataIndex(item.Item1));
            }

            if (changeArcheType)
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);

                for (var i = 0; i < count; i++)
                {
                    ArcheTypeData.TransferEntity(GlobalVersion,
                        entities[i],
                        prevArcheTypeData,
                        nextArcheTypeData,
                        _entityDatas);
                }
            }
        }
    }
}
