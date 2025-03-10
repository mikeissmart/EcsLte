﻿using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal interface IComponentData : IComparable<IComponentData>, IEquatable<IComponentData>
    {
        ComponentConfig Config { get; }
    }

    internal interface IGeneralComponentData : IComponentData
    {
        IGeneralComponent Component { get; }

        unsafe void SetComponentData(ArcheTypeData archeTypeData, ChangeVersion changeVersion, EntityData entityData);
        unsafe void SetComponentDatas(ArcheTypeData archeTypeData, ChangeVersion changeVersion, int startingEntityIndex, int count);
    }

    internal interface ISharedComponentData : IComponentData
    {
        ISharedComponent Component { get; }

        bool ComponentEquals<TComponentEqual>(TComponentEqual component)
            where TComponentEqual : unmanaged, ISharedComponent;

        SharedDataIndex GetSharedComponentDataIndex(SharedComponentDictionaries sharedIndexDics);
    }

    internal interface IManagedComponentData : IComponentData
    {
        IManagedComponent Component { get; }

        void SetComponentData(ArcheTypeData archeTypeData, ChangeVersion changeVersion, EntityData entityData);
        void SetComponentDatas(ArcheTypeData archeTypeData, ChangeVersion changeVersion, int startingEntityIndex, int count);
    }

    internal abstract class ComponentData : IComponentData
    {
        public ComponentConfig Config { get; private set; }

        public ComponentData(ComponentConfig config) => Config = config;

        public int CompareTo(IComponentData other)
            => Config.CompareTo(other.Config);

        public bool Equals(IComponentData other)
            => Config.Equals(other.Config);
    }

    internal class GeneralComponentData<TComponent> : ComponentData, IGeneralComponentData
        where TComponent : unmanaged, IGeneralComponent
    {
        private readonly TComponent _component;

        public TComponent Component => _component;
        IGeneralComponent IGeneralComponentData.Component => _component;

        internal GeneralComponentData(TComponent component)
            : base(ComponentConfig<TComponent>.Config) => _component = component;

        public unsafe void SetComponentData(ArcheTypeData archeTypeData, ChangeVersion changeVersion, EntityData entityData)
            => archeTypeData.SetComponent(changeVersion, entityData, Config, _component);

        public unsafe void SetComponentDatas(ArcheTypeData archeTypeData, ChangeVersion changeVersion, int startingEntityIndex, int count)
            => archeTypeData.SetAllComponents(changeVersion, startingEntityIndex, count, Config, _component);
    }

    internal class ManagedComponentData<TComponent> : ComponentData, IManagedComponentData
        where TComponent : IManagedComponent
    {
        private readonly TComponent _component;

        public TComponent Component => _component;
        IManagedComponent IManagedComponentData.Component => _component;

        internal ManagedComponentData(TComponent component)
            : base(ComponentConfig<TComponent>.Config) => _component = component;

        public void SetComponentData(ArcheTypeData archeTypeData, ChangeVersion changeVersion, EntityData entityData)
            => archeTypeData.SetManagedComponent(changeVersion, entityData, Config, _component);

        public void SetComponentDatas(ArcheTypeData archeTypeData, ChangeVersion changeVersion, int startingEntityIndex, int count)
            => archeTypeData.SetAllManagedComponents(changeVersion, startingEntityIndex, count, Config, _component);
    }

    internal class SharedComponentData<TComponent> : ComponentData, ISharedComponentData
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly TComponent _component;
        private int _hashCode;

        public TComponent Component => _component;
        ISharedComponent ISharedComponentData.Component => _component;

        internal SharedComponentData(TComponent component)
            : base(ComponentConfig<TComponent>.Config) => _component = component;

        public bool ComponentEquals<TComponentEqual>(TComponentEqual component)
            where TComponentEqual : unmanaged, ISharedComponent
            => Config == ComponentConfig<TComponentEqual>.Config &&
                _component.GetHashCode() == component.GetHashCode() &&
                _component.Equals(component);

        public SharedDataIndex GetSharedComponentDataIndex(SharedComponentDictionaries sharedIndexDics)
            => sharedIndexDics.GetDic<TComponent>().GetSharedDataIndex(_component);

        public new bool Equals(IComponentData other)
            => Config.Equals(other.Config) &&
                _component.GetHashCode() == ((SharedComponentData<TComponent>)other).Component.GetHashCode() &&
                _component.Equals(((SharedComponentData<TComponent>)other).Component);

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(Config)
                    .AppendHashCode(_component)
                    .HashCode;
            }

            return _hashCode;
        }
    }
}