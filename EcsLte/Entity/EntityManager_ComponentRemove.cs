namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void RemoveComponent(Entity entity, ComponentConfig config)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            AssertNotHaveComponent(config, prevArcheTypeData);
            InternalRemoveConfigTrackingTransferEntity(entity, prevArcheTypeData, config,
                false);
        }

        public void RemoveComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, prevArcheTypeData);
            InternalRemoveConfigTrackingTransferEntity(entity, prevArcheTypeData, config,
                false);
        }

        public void RemoveManagedComponent<TComponent>(Entity entity)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, prevArcheTypeData);
            InternalRemoveConfigTrackingTransferEntity(entity, prevArcheTypeData, config,
                false);
        }

        public void RemoveSharedComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity,
                out var _, out var prevArcheTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, prevArcheTypeData);
            InternalRemoveConfigTrackingTransferEntity(entity, prevArcheTypeData, config,
                false);
        }

        public void RemoveSharedComponent<TComponent>(EntityArcheType archeType)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var prevArcheTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);

            AssertAlreadyHasComponent(config, prevArcheTypeData);
            InternalRemoveConfigTrackingTransferArcheTypeData(prevArcheTypeData, config,
                true);
        }
    }
}
