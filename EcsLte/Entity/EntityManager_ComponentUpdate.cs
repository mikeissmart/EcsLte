namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void UpdateComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(config, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            archeTypeData.SetComponent(GlobalVersion, entityData, config, component);
        }

        public void UpdateManagedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(config, archeTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            archeTypeData.SetManagedComponent(GlobalVersion, entityData, config, component);
        }

        public void UpdateSharedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(config, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType,
                Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component)))
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(GlobalVersion,
                    entity,
                    prevArcheTypeData,
                    nextArcheTypeData,
                    _entityDatas);
            }
        }

        public void UpdateSharedComponent<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : unmanaged, ISharedComponent
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
                InternalUpdateSharedTransferArcheTypeData(prevArcheTypeData,
                    Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component));
            }
        }
    }
}
