using System;

namespace EcsLte
{
    internal interface IComponentData : IComparable<IComponentData>, IEquatable<IComponentData>
    {
        ComponentConfig Config { get; }
        IComponent Component { get; }

        unsafe void CopyComponentData(byte* componentPtr/*, int componentIndex, IManagedComponentPool[] managedComponentPools*/);
        bool ComponentEquals<TComponentEqual>(TComponentEqual component) where TComponentEqual : IComponent;
    }

    internal class ComponentData<TComponent> : IComponentData
        where TComponent : unmanaged, IComponent
    {
        private readonly TComponent _component;

        public ComponentConfig Config { get; private set; }
        public IComponent Component => _component;

        internal ComponentData(TComponent component)
        {
            _component = component;
            Config = ComponentConfig<TComponent>.Config;
        }

        public unsafe void CopyComponentData(byte* componentPtr/*, int componentIndex, IManagedComponentPool[] managedComponentPools*/) =>
            /*if (Config.IsManaged)
{
var pool = (ManagedComponentPool<TComponent>)managedComponentPools[Config.ManagedIndex];
pool.SetComponent(componentIndex, _component);
*(int*)componentPtr = componentIndex;
}
else
{
Marshal.StructureToPtr(_component, (IntPtr)componentPtr, false);
}*/
            *(TComponent*)componentPtr = _component;

        public bool ComponentEquals<TComponentEqual>(TComponentEqual component)
             where TComponentEqual : IComponent => Config == ComponentConfig<TComponentEqual>.Config &&
                _component.Equals(component);

        public int CompareTo(IComponentData other)
            => Config.CompareTo(other.Config);

        public bool Equals(IComponentData other)
            => Config.Equals(other.Config) &&
                _component.Equals((TComponent)other.Component);
    }

    internal class SharedComponentData<TComponent> : IComponentData
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly TComponent _component;

        public ComponentConfig Config { get; private set; }
        public IComponent Component => _component;

        internal SharedComponentData(TComponent component)
        {
            _component = component;
            Config = ComponentConfig<TComponent>.Config;
        }

        public unsafe void CopyComponentData(byte* componentPtr)
            => *(TComponent*)componentPtr = _component;

        public bool ComponentEquals<TComponentEqual>(TComponentEqual component)
             where TComponentEqual : IComponent => Config == ComponentConfig<TComponentEqual>.Config &&
                _component.Equals(component);

        public int CompareTo(IComponentData other)
            => Config.CompareTo(other.Config);

        public bool Equals(IComponentData other)
            => Config.Equals(other.Config) &&
                _component.Equals((TComponent)other.Component);
    }
}
