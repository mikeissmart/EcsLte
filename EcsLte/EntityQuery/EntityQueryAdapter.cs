using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal interface IEntityQueryAdapter
    {
        void ChangeArcheTypeData(ArcheTypeData archeTypeData);
    }

    internal interface IEntityQueryAdapter<TComponent> : IEntityQueryAdapter
        where TComponent : IComponent
    {
        ref TComponent GetRef(int entityIndex);
    }

    internal abstract class EntityQueryAdapter<TComponent> : IEntityQueryAdapter<TComponent>
        where TComponent : IComponent
    {
        protected ArcheTypeData ArcheTypeData { get; set; }
        protected ComponentConfigOffset ConfigOffset { get; set; }

        public abstract void ChangeArcheTypeData(ArcheTypeData archeTypeDatas);
        public abstract ref TComponent GetRef(int entityIndex);
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

        public unsafe override ref TComponent GetRef(int entityIndex) =>
            ref *(TComponent*)ArcheTypeData.GetComponentPtr(entityIndex, ConfigOffset);
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

        public override ref TComponent GetRef(int entityIndex) =>
            ref ArcheTypeData.GetManagedComponentRef<TComponent>(entityIndex, ConfigOffset);
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

        public unsafe override ref TComponent GetRef(int entityIndex)
        {
            _component = _originalComponent;
            return ref _component;
        }
    }

    #endregion
}
