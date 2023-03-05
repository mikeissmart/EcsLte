namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHasComponent(config, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            InternalAddConfigTransferEntity(entity, prevArcheTypeData, config,
                null,
                out var nextArcheTypeData);

            nextArcheTypeData.LastChunk.SetComponent(GlobalVersion,
                nextArcheTypeData.LastChunk.EntityCount - 1, config, component);
        }

        public void AddManagedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHasComponent(config, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            InternalAddConfigTransferEntity(entity, prevArcheTypeData, config,
                null,
                out var nextArcheTypeData);

            nextArcheTypeData.LastChunk.SetManagedComponent(GlobalVersion,
                nextArcheTypeData.LastChunk.EntityCount - 1, config, component);
        }

        public void AddSharedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHasComponent(config, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            InternalAddConfigTransferEntity(entity, prevArcheTypeData, config,
                Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component),
                out var _);
        }

        public void AddSharedComponent<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            AssertAlreadyHasComponent(config, prevArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            InternalAddConfigTransferArcheTypeData(prevArcheTypeData, config,
                Context.SharedComponentDics.GetDic<TComponent>().GetSharedDataIndex(component),
                out var _, out var _);
        }
    }
}
