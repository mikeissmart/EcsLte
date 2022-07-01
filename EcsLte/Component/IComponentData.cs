using System;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal interface IComponentData : IComparable<IComponentData>, IEquatable<IComponentData>
    {
        ComponentConfig Config { get; }
        IComponent Component { get; }

        unsafe void CopyBlittableComponentData(byte* componentPtr);

        unsafe void CopyManagedComponentData(ArcheTypeData archeTypeData, EntityData entityData, int componentIndex, IManagedComponentPool managedPool);

        bool ComponentEquals<TComponentEqual>(TComponentEqual component) where TComponentEqual : IComponent;

        SharedComponentDataIndex GetSharedComponentDataIndex(SharedComponentIndexDictionaries sharedIndexDics);
    }

    internal class ComponentData<TComponent> : IComponentData
        where TComponent : IComponent
    {
        private readonly TComponent _component;

        public ComponentConfig Config { get; private set; }
        public IComponent Component => _component;

        internal ComponentData(TComponent component)
        {
            _component = component;
            Config = ComponentConfig<TComponent>.Config;
        }

        public unsafe void CopyBlittableComponentData(byte* componentPtr) => Marshal.StructureToPtr(_component, (IntPtr)componentPtr, false);

        public unsafe void CopyManagedComponentData(ArcheTypeData archeTypeData, EntityData entityData, int componentIndex, IManagedComponentPool managedPool) =>
            archeTypeData.SetComponentAndIndex(entityData, _component, Config, componentIndex, (ManagedComponentPool<TComponent>)managedPool);

        public bool ComponentEquals<TComponentEqual>(TComponentEqual component)
             where TComponentEqual : IComponent => Config == ComponentConfig<TComponentEqual>.Config &&
                _component.Equals(component);

        public SharedComponentDataIndex GetSharedComponentDataIndex(SharedComponentIndexDictionaries sharedIndexDics) =>
            sharedIndexDics.GetDataIndex(_component);

        public int CompareTo(IComponentData other)
            => Config.CompareTo(other.Config);

        public bool Equals(IComponentData other)
            => Config.Equals(other.Config) &&
                _component.Equals((TComponent)other.Component);
    }
}