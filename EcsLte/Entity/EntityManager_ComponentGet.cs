namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, archeTypeData);

            return archeTypeData.GetComponent<TComponent>(entityData.EntityIndex, config);
        }

        public TComponent GetManagedComponent<TComponent>(Entity entity)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, archeTypeData);

            return archeTypeData.GetManagedComponent<TComponent>(entityData.EntityIndex, config);
        }

        public TComponent GetSharedComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            var config = ComponentConfig<TComponent>.Config;

            AssertNotHaveComponent(config, archeTypeData);

            return archeTypeData.GetSharedComponent<TComponent>(config);
        }

        public TComponent GetSharedComponent<TComponent>(EntityArcheType archeType)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);

            AssertNotHaveComponent(config, archeTypeData);

            return archeTypeData.GetSharedComponent<TComponent>(config);
        }
    }
}
