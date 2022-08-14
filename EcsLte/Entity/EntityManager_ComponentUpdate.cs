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
            archeTypeData.SetComponent(entityData.EntityIndex, config, component);

            Context.Tracking.TrackUpdate(entity, config, archeTypeData);
        }

        public void UpdateManagedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, archeTypeData);
            archeTypeData.SetManagedComponent(entityData.EntityIndex, config, component);

            Context.Tracking.TrackUpdate(entity, config, archeTypeData);
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
            ArcheType.CopyToCached(prevArcheTypeData.ArcheType, ref _cachedArcheType);
            if (ArcheType.ReplaceSharedDataIndex(ref _cachedArcheType,
                Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component)))
            {
                var nextArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
                ArcheTypeData.TransferEntity(entity, _entityDatas,
                    prevArcheTypeData, nextArcheTypeData);

                Context.Tracking.TrackArcheTypeDataChange(entity,
                    prevArcheTypeData, nextArcheTypeData);
            }

            Context.Tracking.TrackUpdate(entity, config, prevArcheTypeData);
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

            InternalUpdateSharedTrackingTransferArcheTypeData(prevArcheTypeData, config,
                Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component));
        }
    }
}
