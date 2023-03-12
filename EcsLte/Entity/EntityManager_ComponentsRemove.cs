using EcsLte.Utilities;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void RemoveComponents<T1, T2>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<T1, T2, T3>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);
            AssertNotHaveComponent(config3, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);
            config3.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<T1, T2, T3, T4>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);
            AssertNotHaveComponent(config3, prevArcheTypeData);
            AssertNotHaveComponent(config4, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);
            config3.Adapter.RemoveConfig(ref _cachedArcheType);
            config4.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<T1, T2, T3, T4, T5>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);
            AssertNotHaveComponent(config3, prevArcheTypeData);
            AssertNotHaveComponent(config4, prevArcheTypeData);
            AssertNotHaveComponent(config5, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);
            config3.Adapter.RemoveConfig(ref _cachedArcheType);
            config4.Adapter.RemoveConfig(ref _cachedArcheType);
            config5.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<T1, T2, T3, T4, T5, T6>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);
            AssertNotHaveComponent(config3, prevArcheTypeData);
            AssertNotHaveComponent(config4, prevArcheTypeData);
            AssertNotHaveComponent(config5, prevArcheTypeData);
            AssertNotHaveComponent(config6, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);
            config3.Adapter.RemoveConfig(ref _cachedArcheType);
            config4.Adapter.RemoveConfig(ref _cachedArcheType);
            config5.Adapter.RemoveConfig(ref _cachedArcheType);
            config6.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);
            AssertNotHaveComponent(config3, prevArcheTypeData);
            AssertNotHaveComponent(config4, prevArcheTypeData);
            AssertNotHaveComponent(config5, prevArcheTypeData);
            AssertNotHaveComponent(config6, prevArcheTypeData);
            AssertNotHaveComponent(config7, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);
            config3.Adapter.RemoveConfig(ref _cachedArcheType);
            config4.Adapter.RemoveConfig(ref _cachedArcheType);
            config5.Adapter.RemoveConfig(ref _cachedArcheType);
            config6.Adapter.RemoveConfig(ref _cachedArcheType);
            config7.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity)
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

            AssertNotHaveComponent(config1, prevArcheTypeData);
            AssertNotHaveComponent(config2, prevArcheTypeData);
            AssertNotHaveComponent(config3, prevArcheTypeData);
            AssertNotHaveComponent(config4, prevArcheTypeData);
            AssertNotHaveComponent(config5, prevArcheTypeData);
            AssertNotHaveComponent(config6, prevArcheTypeData);
            AssertNotHaveComponent(config7, prevArcheTypeData);
            AssertNotHaveComponent(config8, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);

            config1.Adapter.RemoveConfig(ref _cachedArcheType);
            config2.Adapter.RemoveConfig(ref _cachedArcheType);
            config3.Adapter.RemoveConfig(ref _cachedArcheType);
            config4.Adapter.RemoveConfig(ref _cachedArcheType);
            config5.Adapter.RemoveConfig(ref _cachedArcheType);
            config6.Adapter.RemoveConfig(ref _cachedArcheType);
            config7.Adapter.RemoveConfig(ref _cachedArcheType);
            config8.Adapter.RemoveConfig(ref _cachedArcheType);

            var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            ArcheTypeData.TransferEntity(GlobalVersion,
                entity,
                prevArcheTypeData,
                nextArcheTypeData,
                _entityDatas);
        }

        public void RemoveComponents<TComponent>(EntityArcheType archeType)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            AssertNotHaveComponent(config, prevArcheTypeData);

            if (prevArcheTypeData.EntityCount() > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);
                InternalRemoveConfigTransferArcheTypeData(prevArcheTypeData, config,
                    false);
            }
        }

        public void RemoveComponents<TComponent>(EntityFilter filter)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var prevArcheTypeData = filteredArcheTypeDatas[i];
                if (prevArcheTypeData.HasConfig(config) && prevArcheTypeData.EntityCount() > 0)
                {
                    if (!incVersion)
                    {
                        ChangeVersion.IncVersion(ref _globalVersion);
                        incVersion = true;
                    }

                    InternalRemoveConfigTransferArcheTypeData(prevArcheTypeData, config,
                        false);
                }
            }
        }

        public void RemoveManagedComponents<TComponent>(EntityArcheType archeType)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            AssertNotHaveComponent(config, prevArcheTypeData);

            if (prevArcheTypeData.EntityCount() > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);
                InternalRemoveConfigTransferArcheTypeData(prevArcheTypeData, config,
                    false);
            }
        }

        public void RemoveManagedComponents<TComponent>(EntityFilter filter)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var prevArcheTypeData = filteredArcheTypeDatas[i];
                if (prevArcheTypeData.HasConfig(config) && prevArcheTypeData.EntityCount() > 0)
                {
                    if (!incVersion)
                    {
                        ChangeVersion.IncVersion(ref _globalVersion);
                        incVersion = true;
                    }

                    InternalRemoveConfigTransferArcheTypeData(prevArcheTypeData, config,
                        false);
                }
            }
        }

        public void RemoveSharedComponents<TComponent>(EntityFilter filter)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var prevArcheTypeData = filteredArcheTypeDatas[i];
                if (prevArcheTypeData.HasConfig(config) && prevArcheTypeData.EntityCount() > 0)
                {
                    if (!incVersion)
                    {
                        ChangeVersion.IncVersion(ref _globalVersion);
                        incVersion = true;
                    }

                    InternalRemoveConfigTransferArcheTypeData(prevArcheTypeData, config,
                        false);
                }
            }
        }
    }
}
