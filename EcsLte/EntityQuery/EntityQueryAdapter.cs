namespace EcsLte
{
    internal interface IEntityQueryAdapter
    {
        void ChangeArcheTypeData(ArcheTypeData archeTypeData);
    }

    internal interface IEntityQueryAdapter<TComponent> : IEntityQueryAdapter
        where TComponent : IComponent
    {
        ref TComponent GetRef(EntityData entityData);
        TComponent GetUpdatedComponent();
    }

    internal abstract class EntityQueryAdapter<TComponent> : IEntityQueryAdapter<TComponent>
        where TComponent : IComponent
    {
        protected ArcheTypeData ArcheTypeData { get; set; }
        protected ComponentConfigOffset ConfigOffset { get; set; }

        public abstract void ChangeArcheTypeData(ArcheTypeData archeTypeDatas);
        public abstract ref TComponent GetRef(EntityData entityData);
        public virtual TComponent GetUpdatedComponent() => throw new System.NotImplementedException();
    }

    #region GeneralComponent

    internal class EntityQueryGeneralAdapter<TComponent> : EntityQueryAdapter<TComponent>
        where TComponent : unmanaged, IGeneralComponent
    {
        public override void ChangeArcheTypeData(ArcheTypeData archeTypeData)
        {
            ArcheTypeData = archeTypeData;
            ConfigOffset = archeTypeData.GetConfigOffset(ComponentConfig<TComponent>.Config);
        }

        public override ref TComponent GetRef(EntityData entityData)
            => ref ArcheTypeData.GetComponentRef<TComponent>(entityData, ConfigOffset);
    }

    #endregion

    #region ManagedComponent

    internal class EntityQueryManagedAdapter<TComponent> : EntityQueryAdapter<TComponent>
        where TComponent : IManagedComponent
    {
        public override void ChangeArcheTypeData(ArcheTypeData archeTypeData)
        {
            ArcheTypeData = archeTypeData;
            ConfigOffset = archeTypeData.GetConfigOffset(ComponentConfig<TComponent>.Config);
        }

        public override ref TComponent GetRef(EntityData entityData)
            => ref ArcheTypeData.GetManagedComponentRef<TComponent>(entityData, ConfigOffset);
    }

    #endregion

    #region SharedComponent

    internal class EntityQuerySharedAdapter<TComponent> : EntityQueryAdapter<TComponent>
        where TComponent : unmanaged, ISharedComponent
    {
        private TComponent _component;
        private TComponent _originalComponent;

        public override void ChangeArcheTypeData(ArcheTypeData archeTypeData)
        {
            ArcheTypeData = archeTypeData;
            ConfigOffset = archeTypeData.GetConfigOffset(ComponentConfig<TComponent>.Config);
            _originalComponent = archeTypeData.GetSharedComponent<TComponent>(ConfigOffset);
        }

        public override ref TComponent GetRef(EntityData entityData)
        {
            _component = _originalComponent;
            return ref _component;
        }

        public override TComponent GetUpdatedComponent() => _component;
    }

    #endregion
}
